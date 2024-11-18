using System.Collections;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class IterativeEnemyController : MonoBehaviour, TakesDamage, IHearSounds
{
    // Editor exposed variable that stores the enemy HP.
    [SerializeField] int hp;

    // Editor exposed variable that stores the model of the enemy being hit. Needed to flash damage.
    [SerializeField] Renderer model;

    // Editor exposed Navigational Mesh Agent that allows our enemy to intelligently path towards the player.
    [SerializeField] NavMeshAgent agent;

    // Editor exposed Transform that tracks the position from which bullets are fired from the model.
    [SerializeField] Transform shootPos;

    // Editor exposed Transform that tracks the head of our enemy, important for rotating the model.
    [SerializeField] Transform headPos;

    // Editor exposed int that tracks the speed at which our enemy rotates towards the player.
    [SerializeField] int faceTargetSpeed;

    // Editor exposed float that tracks the AI's rate of fire.
    [SerializeField] float shootRate;

    // Editor exposed GameObject that contains our bullet, which our AI will fire.
    [SerializeField] GameObject bullet;

    // Editor exposed variable that stores the maximum angle at which the AI can "see" our player.
    [SerializeField] int enemyViewAngle;

    // Float that stores the timer on which enemies will roam.
    [SerializeField] float roamTimer;

    // Color instance that represents the original model color of our AI.
    Color origColor;

    // Color instance that represents the color that our character will flash when hit.
    Color dmgColor;

    // Boolean that tracks when the enemy is currently firing.
    bool isShooting;

    // Boolean that tracks when the player is in range of the enemy.
    bool playerInRange;

    // Vector3 that stores the player's position in world space.
    Vector3 playerDir;

    // Float that tracks the angle from the AI to the player.
    float angleToPlayer;

    // Vector 3 that tracks the original position that the object spawned in at.
    Vector3 origLocation;

    // Boolean that tracks whether the AI is currently investing a sound.
    bool iHeardSomething;

    // Boolean that tracks whether the AI was recently hit.
    bool someoneHitMe;

    // Boolean that tracks whether the AI is currently on high alert and seeking the player directly.
    bool isOnHighAlert;

    // Integer that tracks the stopping distance of the AI.
    float origStoppingDistance;

    // Vector3 that tracks where the sound generating object is located.
    Vector3 heardSoundLocation;

    // Integer that tracks the range at which the AI will roam.
    [SerializeField] int roamDistance;

    // Vector3 that specifies the minimum radius that the AI will roam to.
    [SerializeField] Vector3 minRoamDist;

    // Animator reference in our code. Required to access modifiers to animations.
    [SerializeField] Animator enemyAnimator;

    // Integer that tracks the rate at which animations transition between states.
    [SerializeField] int animTransitionSpeed;

    // Boolean that tracks whether the AI is roaming.
    bool isRoaming;

    // Temporary Coroutine variable. This allows us to end the specific instance of Roam() called in the event that our AI needs to stop roaming to respond to other things.
    Coroutine tempRoam;

    // Properties for the above data members and their respective getters and setters.
    public int Hp { get => hp; set => hp = value; }

    public Renderer Model { get => model; set => model = value; }

    public NavMeshAgent Agent { get => agent; set => agent = value; }

    public Transform ShootPos { get => shootPos; set => shootPos = value; }

    public Transform HeadPos { get => headPos; set => headPos = value; }

    public int FaceTargetSpeed { get => faceTargetSpeed; set => faceTargetSpeed = value; }

    public float ShootRate { get => shootRate; set => shootRate = value; }

    public GameObject Bullet { get => bullet; set => bullet = value; }

    public Color OrigColor { get => origColor; set => origColor = value; }

    public Color DmgColor { get => dmgColor; set => dmgColor = value; }

    public bool IsShooting { get => isShooting; set => isShooting = value; }

    public bool PlayerInRange { get => playerInRange; set => playerInRange = value; }

    public Vector3 PlayerDir { get => playerDir; set => playerDir = value; }

    public int EnemyViewAngle { get => enemyViewAngle; set => enemyViewAngle = value; }

    public float AngleToPlayer { get => angleToPlayer; set => angleToPlayer = value; }

    public Vector3 OrigLocation { get => origLocation; set => origLocation = value; }

    public bool IHeardSomething { get => iHeardSomething; set => iHeardSomething = value; }

    public bool IsOnHighAlert { get => isOnHighAlert; set => isOnHighAlert = value; }

    public float OrigStoppingDistance { get => origStoppingDistance; set => origStoppingDistance = value; }

    public bool IsRoaming { get => isRoaming; set => isRoaming = value; }

    public float RoamTimer { get => roamTimer; set => roamTimer = value; }

    public Vector3 MinRoamDist { get => minRoamDist; set => minRoamDist = value; }

    public int RoamDistance { get => roamDistance; set => roamDistance = value; }

    public bool SomeoneHitMe { get => someoneHitMe; set => someoneHitMe = value; }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // We set origColor to the model's color.
        origColor = Model.material.color;

        // We then update the EnemyCount of our PlayerLocator accordingly.
        GameManager.instance.updateGameGoal(1);

        // We set OrigLocation to the original location the object spawned in at.
        OrigLocation = transform.position;

        OrigStoppingDistance = agent.stoppingDistance;

    }

    // Update is called once per frame
    void Update()
    {

        UpdateAnim();

        if (!isOnHighAlert)
        {
            if (IsRoaming)
            {
                if (IHeardSomething)
                {
                    StopCoroutine(tempRoam);
                    IsRoaming = false;
                    StartCoroutine(InvestigateSound());
                }
                else if (SomeoneHitMe)
                {
                    StopCoroutine(tempRoam);
                    IsRoaming = false;
                }
            }
            // If our player is in range of the AI, and the AI can see the player:
            else if (playerInRange && !CanSeePlayer())
            {
                if (!isRoaming && !IHeardSomething && agent.remainingDistance < 0.05f)
                {
                    tempRoam = StartCoroutine(Roam());
                }

            }
            else if (!PlayerInRange)
            {
                if (!isRoaming && !IHeardSomething && agent.remainingDistance < 0.05f)
                {
                    tempRoam = StartCoroutine(Roam());
                }
            }
        }
        else
        {
            AttackThePlayer();
        }
            
    }

    bool CanSeePlayer()
    {
        // We initialize a local bool that will be returned at the end of the function call.
        bool playerIsVisible = false;

        // We set our playerDir to the Player's transform position, subtracted by the headPos position to get the player's actual location.
        playerDir = GameManager.instance.player.transform.position - headPos.position;

        // We set our angle to the player to be the angle between the player's vector3 and the model's forward transform.
        AngleToPlayer = Vector3.Angle(playerDir, transform.forward);

        // We then initialize a RaycastHit which will store the object it hits.

        // If our raycast hits something:
        if (Physics.Raycast(headPos.position, playerDir, out RaycastHit hit) && hit.collider.CompareTag("Player") && AngleToPlayer <= EnemyViewAngle)
        {
            // We attack the player.
            AttackThePlayer();

            // We then also set playerIsVisible to true.
            playerIsVisible = true;
        }

        agent.stoppingDistance = playerIsVisible? OrigStoppingDistance : 0;

        // Finally, regardless of the flow of execution, we return our boolean here.
        return playerIsVisible;
    }

    // Private method called when the player enters the sphere collider. Sets our PlayerInRange to true for our AttackThePlayer method.
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            PlayerInRange = true;
        }

    }

    // Private method called whenever the player exits the AI's sphere collider. Sets our PlayerInRange to false for our AttackThePlayer method.
    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            PlayerInRange = false;

            agent.stoppingDistance = 0;
        }

    }


    // Required interface method that must be implemented as a part of the IDamage interface. Adjusts enemyAI HP, and destroys it if it falls to or below 0.
    public void TakeSomeDamage(int amt)
    {
        hp -= amt;

        StartCoroutine(FlashRed());

        agent.SetDestination(GameManager.instance.player.transform.position);

        if (hp <= 0)
        {
            GameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);

        }

    }

    // IEnumerator called in a coroutine, flashes the enemyAI's model red.
    IEnumerator FlashRed()
    {
        model.material.color = DmgColor;
        yield return new WaitForSeconds(0.2f);
        model.material.color = OrigColor;

        while (agent.pathPending)
        {
            SomeoneHitMe = true;
            yield return null;
        }
    }

    // IEnumerator called in a coroutine, enables the enemyAI to shoot at the player.
    IEnumerator Shoot()
    {
        IsShooting = true;

        enemyAnimator.SetTrigger("Shoot");

        yield return new WaitForSeconds(ShootRate);

        IsShooting = false;
    }

    public void CreateBullet()
    {
        if (IsShooting)
        {
            Instantiate(bullet, shootPos.position, transform.rotation);
        }
    }

    // Method that rotates the enemyAI towards the player. Called when the NavMeshAgent detects that it is within stopping distance.
    void FaceTarget()
    {
        // We get a quaternion from the LookRotation of the playerDir.
        Quaternion rot = Quaternion.LookRotation(playerDir);

        // We then rotate the enemy AI using a lerp, which lerps from the model's current rotation to the rot, in deltaTime.
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, FaceTargetSpeed * Time.deltaTime);
    }

    // Method that is called in Update. Locates the player and fires at them if the AI is not shooting them already.
    void AttackThePlayer()
    {
        // We then set the NavMeshAgent's destination to the player's transform position.
        agent.SetDestination(GameManager.instance.player.transform.position);

        // If the agent is within stopping distance:
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            // The AI will continuously turn towards the player in order to shoot them.
            FaceTarget();
        }

        // Finally, if the AI isn't shooting, it will call the Shoot() coroutine.
        if (!IsShooting)
        {
            StartCoroutine(Shoot());
        }
    }

    public void ReactToSound(Vector3 invokingLocation)
    {
        IHeardSomething = true;
        heardSoundLocation = invokingLocation;

    }

    IEnumerator Roam()
    {

        IsRoaming = true;

        yield return new WaitForSeconds(RoamTimer);

        agent.stoppingDistance = 0;

        Vector3 randomDist = MinRoamDist + (Random.insideUnitSphere * RoamDistance);

        randomDist += OrigLocation;

        NavMeshHit hit;

        NavMesh.SamplePosition(randomDist, out hit, RoamDistance, 1);

        agent.SetDestination(hit.position);

        isRoaming = false;

        tempRoam = null;

    }

    IEnumerator InvestigateSound()
    {
        agent.stoppingDistance = 0;

        agent.SetDestination(heardSoundLocation);

        while (agent.pathPending)
        {
            yield return null;
        }

        yield return new WaitForSeconds(RoamTimer);

        IHeardSomething = false;
    }

    void UpdateAnim()
    {
        // We want to know the absolute value of our character's normalized speed for the purposes of our animator, so we get the normalized magnitude of our velocity.
        float normAgentSpeed = agent.velocity.normalized.magnitude;

        // We also want to know what the current speed of our animation is.
        float currAnimSpeed = enemyAnimator.GetFloat("Speed");

        // Now that we have that animation, we now update our Animator's speed float with that agent speed, allowing us to accurately blend the animations.
        enemyAnimator.SetFloat("Speed", Mathf.Lerp(currAnimSpeed, normAgentSpeed, Time.deltaTime * animTransitionSpeed));
    }
}
