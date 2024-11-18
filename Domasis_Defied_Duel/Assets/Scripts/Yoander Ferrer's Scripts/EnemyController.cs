using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
// Enemy Controller fully created and managed by Yoander Ferrer.

public class EnemyController : MonoBehaviour, TakesDamage
{
    [Header("----- Enemy Models and Transforms -----")]

    // Editor exposed variable that stores the model of the enemy being hit. Needed to flash damage.
    [SerializeField] Renderer model;

    // Editor exposed Navigational Mesh Agent that allows our enemy to intelligently path towards the player.
    [SerializeField] NavMeshAgent agent;

    // Editor exposed Transform that tracks the position from which bullets are fired from the model.
    [SerializeField] Transform shootPos;

    // Editor exposed Transform that tracks the head of our enemy, important for rotating the model.
    [SerializeField] Transform headPos;

    [Header("----- AI Enemy Stats -----")]
    // Editor exposed variable that stores the enemy HP.
    [SerializeField] [Range(3, 20)] int hp;

    // Editor exposed float that tracks the AI's rate of fire.
    [SerializeField] [Range(0.6f, 2.0f)] float shootRate;

    // Editor exposed variable that stores the maximum angle at which the AI can "see" our player.
    [SerializeField][Range(75, 160)] int enemyViewAngle;

    // Editor exposed GameObject that contains our bullet, which our AI will fire.
    [SerializeField] GameObject bullet;

    [Header("----- AI Roam Settings -----")]

    // Editor exposed int that tracks the speed at which our enemy rotates towards the player.
    [SerializeField][Range(2, 15)] int faceTargetSpeed;

    // Float that tracks the distance that our AI can roam to.
    [SerializeField] float roamDistance;

    // Int that tracks how long the AI will wait before roaming again.
    [SerializeField] int roamTimer;

    // Vector3 that ensures that the AI travels at least a certain distance.
    [SerializeField] Vector3 minRoamDist;

    [Header("----- Animation Settings -----")]

    // Int that tracks the rate at which our animations will transition between.
    [SerializeField] int animTransitionSpeed;

    // Animator reference in our code. Required to access modifiers to animations.
    [SerializeField] Animator enemyAnimator;

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

    // Vector3 that stores the AI's location on spawn.
    Vector3 origLocation;

    // Float that tracks the angle from the AI to the player.
    float angleToPlayer;

    // Bool that stores whether our player is currently roaming.
    bool isRoaming;

    // Int that tracks the current stopping distance of our AI.
    float origStoppingDistance;

    // Coroutine variable that stores our coroutine for our Roam logic while it is active, so that it can be shut down when other conditions are met.
    Coroutine tempRoam;

    // Properties for the above data members and their respective getters and setters.
    public int HP { get => hp; set => hp = value; }

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

    public bool IsRoaming { get => isRoaming; set => isRoaming = value; }

    public float OrigStoppingDistance { get => origStoppingDistance; set => origStoppingDistance = value; }

    public float RoamDistance { get => roamDistance; set => roamDistance = value; }

    public int RoamTimer { get => roamTimer; set => roamTimer = value; }

    public Vector3 OrigLocation { get => origLocation; set => origLocation = value; }
    public Vector3 MinRoamDist { get => minRoamDist; set => minRoamDist = value; }

    public Coroutine TempRoam { get => tempRoam; set => tempRoam = value; }

    public Animator EnemyAnimator { get => enemyAnimator; set => enemyAnimator = value; }

    public int AnimTransitionSpeed { get => animTransitionSpeed; set => animTransitionSpeed = value; }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // We set origColor to the model's color.
        origColor = Model.material.color;

        // We then update the EnemyCount of our PlayerLocator accordingly.
        GameManager.instance.updateGameGoal(1);

        // We get the AI's starting position, so that we can always reference it while roaming.
        OrigLocation = transform.position;

        OrigStoppingDistance = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnim();

        // If our player is in range of the AI, and the AI can see the player:
        if (PlayerInRange && !CanSeePlayer())
        {
            if (!isRoaming && agent.remainingDistance < 0.05f)
            {

                StartCoroutine(Roam());

            }
        }
        else if (!PlayerInRange)
        {
            if (!isRoaming && agent.remainingDistance < 0.05f)
            {

                StartCoroutine(Roam());

            }
        }


    }

    bool CanSeePlayer()
    {
        // We initialize a local bool that will be returned at the end of the function call.
        bool playerIsVisible = false;

        // We set our playerDir to the Player's transform position, subtracted by the headPos position to get the player's actual location.
        PlayerDir = GameManager.instance.player.transform.position - headPos.position;

        // We set our angle to the player to be the angle between the player's vector3 and the model's forward transform.
        AngleToPlayer = Vector3.Angle(playerDir, transform.forward);

        // We then initialize a RaycastHit which will store the object it hits.
        RaycastHit hit;

        // If our raycast hits something:
        if (Physics.Raycast(headPos.position, PlayerDir, out hit))
        {
            // If the object it hit was a player, and the player is within the AI's view angle:
            if (hit.collider.CompareTag("Player") && AngleToPlayer <= EnemyViewAngle)
            {
                // We attack the player.
                AttackThePlayer();

                // We then also set playerIsVisible to true.
                playerIsVisible = true;
            }
            
        }

        agent.stoppingDistance = playerIsVisible ? OrigStoppingDistance : 0;

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

        HP -= amt;

        StartCoroutine(FlashRed());

        agent.SetDestination(GameManager.instance.player.transform.position);

        if (HP <= 0)
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

    IEnumerator Roam()
    {
        
        isRoaming = true;

        yield return new WaitForSeconds(RoamTimer);

        agent.stoppingDistance = 0;

        Vector3 randomLoc = MinRoamDist + (Random.insideUnitSphere * RoamDistance);

        randomLoc += OrigLocation;

        NavMesh.SamplePosition(randomLoc, out NavMeshHit hit, RoamDistance, 1);

        agent.SetDestination(hit.position);

        isRoaming = false;

        TempRoam = null;

    }

    void UpdateAnim()
    {
        // We want to know the absolute value of our character's normalized speed for the purposes of our animator, so we get the normalized magnitude of our velocity.
        float normAgentSpeed = agent.velocity.normalized.magnitude;

        // We also want to know what the current speed of our animation is.
        float currAnimSpeed = EnemyAnimator.GetFloat("Speed");

        // Now that we have that animation, we now update our Animator's speed float with that agent speed, allowing us to accurately blend the animations.
        EnemyAnimator.SetFloat("Speed", Mathf.Lerp(currAnimSpeed, normAgentSpeed, Time.deltaTime * AnimTransitionSpeed));
    }
}
