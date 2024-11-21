using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
// Enemy Controller fully created and managed by Yoander Ferrer.

public class EnemyController : MonoBehaviour, TakesDamage, IHearSounds, IAlert
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
    [SerializeField] [Range(3, 10)] float roamDistance;

    // Int that tracks how long the AI will wait before roaming again.
    [SerializeField] [Range(0, 5)] int roamTimer;

    // Vector3 that ensures that the AI travels at least a certain distance.
    [SerializeField] Vector3 minRoamDist;

    [Header("----- Animation Settings -----")]

    // Int that tracks the rate at which our animations will transition between.
    [SerializeField] int animTransitionSpeed;

    // Animator reference in our code. Required to access modifiers to animations.
    [SerializeField] Animator enemyAnimator;

    [Header("----- On Death Investigation Radius -----")]

    // Sets a range around the object's transform position that enemies can roam within.

    [SerializeField] [Range(1, 3)] int investigationRadius;

    // Color instance that represents the original model color of our AI.
    Color origColor;

    // Color instance that represents the color that our character will flash when hit.
    Color dmgColor;

    // Boolean that tracks when the enemy is currently firing.
    bool isShooting;

    // Boolean that tracks when the player is in range of the enemy.
    bool playerInRange;

    // Bool that stores whether our player is currently roaming.
    bool isRoaming;

    // Boolean that tracks whether the AI is currently investigating a sound.
    bool iHeardSomething;

    // Boolean that tracks whether is reacting to taking player damage.
    bool someoneHitMe;

    // Boolean that tracks whether the enemy is on high alert, overriding their normal functionality.
    bool onHighAlert;

    // Float that tracks the angle from the AI to the player.
    float angleToPlayer;

    // Int that tracks the current stopping distance of our AI.
    float origStoppingDistance;

    // Vector3 that stores the player's position in world space.
    Vector3 playerDir;

    // Vector3 that stores the AI's location on spawn.
    Vector3 origLocation;

    // Vector3 that stores the location from which sound was heard.
    Vector3 heardSoundLocation;

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

    public bool IHeardSomething { get => iHeardSomething; set => iHeardSomething = value; }

    public bool SomeoneHitMe { get => someoneHitMe; set => someoneHitMe = value; }

    public bool OnHighAlert { get => onHighAlert; set => onHighAlert = value; }

    public Vector3 HeardSoundLocation { get => heardSoundLocation; set => heardSoundLocation = value; }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // We set origColor to the model's color.
        origColor = Model.material.color;

        // We get the AI's starting position, so that we can always reference it while roaming.
        OrigLocation = transform.position;

        // We set the original stopping distance to the editor exposed stopping distance inside of our NavMeshAgent component.
        OrigStoppingDistance = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        // Every frame, we want to update the AI's animation.
        UpdateAnim();

        // We then call our patrol logic, which handles roaming, investigating, and attacking the player, should the AI spot them.
        PatrolTheArea();


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
        // If the object that enter our collider is a player:
        if (other.CompareTag("Player"))
        {
            // We set our bool to true.
            PlayerInRange = true;
        }

    }

    // Private method called whenever the player exits the AI's sphere collider. Sets our PlayerInRange to false for our AttackThePlayer method.
    private void OnTriggerExit(Collider other)
    {
        // If the object that exited the collision was a player:
        if (other.CompareTag("Player"))
        {
            // We set our bool to false.
            PlayerInRange = false;

            // We then also set the stopping distance back to 0 to ensure that the enemy does not get hung up pathing to a location they won't reach.
            agent.stoppingDistance = 0;
        }

    }

    // Method called every frame. Handles the AI's game logic.
    void PatrolTheArea()
    {
        // If the enemy is not on high alert, we want them to investigate the area normally.
        if (!OnHighAlert)
        {
            // If this bool is true, we know that they're currently in the Roam() coroutine.
            if (IsRoaming)
            {
                // If they hear something that makes sound (handled by a sphere check), or they got shot by the player.
                if (IHeardSomething || SomeoneHitMe)
                {
                    // They need to stop roaming immediately.
                    StopCoroutine(TempRoam);
                    // By setting this variable to false, they can start roaming again, otherwise they'll get stuck where they are without returning to their location.
                    IsRoaming = false;

                }

            }

            // If our player is in range of the AI, and the AI can see the player:
            else if (PlayerInRange && !CanSeePlayer())
            {
                // If they're not already roaming and their distance is less than 0.05m:
                if (!isRoaming && agent.remainingDistance < 0.05f)
                {
                    // We set our coroutine variable to this instance of our Roam() coroutine.
                    TempRoam = StartCoroutine(Roam());

                }
            }
            // If the AI can't see the player and they're not in range, they still want to roam.
            else if (!PlayerInRange)
            {
                // Same protective check, if they're not already roaming, and their remaining distance to their destination is less than 0.05m:
                if (!isRoaming && agent.remainingDistance < 0.05f)
                {
                    // We set our coroutine variable to this instance of our Roam() coroutine.
                    TempRoam = StartCoroutine(Roam());

                }

            }
        }
        // If they are on high alert, the AI needs to ignore all previous protocols and immediately beeline to the player, as they are carrying a super important object.
        else
        {
            AttackThePlayer();
        }
    }

    // Required interface method that must be implemented as a part of the IDamage interface. Adjusts enemyAI HP, and destroys it if it falls to or below 0.
    public void TakeSomeDamage(int amt)
    {
        // We reduce the enemy's health by the damage amount passed in.
        HP -= amt;

        // We then start the coroutine responsible for flashing the enemy's model.
        StartCoroutine(FlashRed());

        // We then set the AI's location to the last location where the player shot the AI from.
        agent.SetDestination(GameManager.instance.player.transform.position);

        // If the enemy has no HP left:
        if (HP <= 0)
        {
            // The enemy will alert all nearby enemies in range, which will cause them to investigate the area.
            AlertEnemies();
            
            // It will then decrement the game goal.
            GameManager.instance.updateGameGoal(-1);

            // Finally, we destroy the game object.
            Destroy(gameObject);

        }    

    }

    // IEnumerator called in a coroutine, flashes the enemyAI's model red.
    IEnumerator FlashRed()
    {
        model.material.color = DmgColor;
        yield return new WaitForSeconds(0.2f);
        model.material.color = OrigColor;

        if (agent.pathPending)
        {
            while (agent.pathPending)
            {
                SomeoneHitMe = true;
                yield return null;
            }
        }
    }

    // IEnumerator called in a coroutine, enables the enemyAI to shoot at the player.
    IEnumerator Shoot()
    {
        // To prevent entering this coroutine prematurely, we set IsShooting to true, which locks it out in Update.
        IsShooting = true;

        // We set our animator's shoot trigger, which causes the enemy to play the shoot animation.
        enemyAnimator.SetTrigger("Shoot");

        // We then call CreateBullet(), which creates a bullet that flies from its spawn location.
        CreateBullet();

        // We then wait for a duration equal to our firerate.
        yield return new WaitForSeconds(ShootRate);

        // We then set IsShooting to false, which allows Update to call the coroutine again.
        IsShooting = false;
    }

    public void CreateBullet()
    {
        // If we're not shooting, we don't want to fire a bullet, so:
        if (IsShooting)
        {
            // We create our bullet with our forward velocity.
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
        // We set our roaming bool to true, both to notify our Patrol logic, as well as to ensure we don't roam before the coroutine finishes.
        isRoaming = true;

        // We wait the specified roam timer (this gives the impression that the AI is scanning the area and looking around).
        yield return new WaitForSeconds(RoamTimer);

        // We then set our AI's stopping distance to 0, which will allow them to reach their destination.
        agent.stoppingDistance = 0;

        /* We generate a ramdom location in a unit sphere, multiplied by our roam distance,
        clamping its size to our minimum roam distance to ensure our AI isn't just moving super small increments occasionally.*/
        Vector3 randomLoc = MinRoamDist + (Random.insideUnitSphere * RoamDistance);

        // We add our original location to our random location, to ensure the AI is roaming around where it spawned.
        randomLoc += OrigLocation;

        // We then sample this position with our NavMesh, making sure that it's a valid position to roam to. If it is, our NavMeshHit will update with that location.
        NavMesh.SamplePosition(randomLoc, out NavMeshHit hit, RoamDistance, 1);

        // Finally, we set our destination to that location.
        agent.SetDestination(hit.position);

        // We then set our roaming bool to false, allowing us to roam again.
        isRoaming = false;

        // We also set our coroutine variable to null, as we don't want to access invalid memory in subsequent calls if we need to stop our coroutine.
        TempRoam = null;

    }
    
    public void ReactToSound(Vector3 invokingLocation)
    {
        // We want them to only investigate a sound when they weren't just hit, or if they didn't already hear something.
        if (!IHeardSomething && !SomeoneHitMe)
        {
            // We update the sound location accordingly.
            HeardSoundLocation = invokingLocation;
            // We then call our investigation coroutine, which is the only way our IHeardSomething bool can be turned off.
            StartCoroutine(InvestigateSound());

        }

    }

    IEnumerator InvestigateSound()
    {
        // To ensure we don't investigate multiple sounds at once, we set this to true.
        IHeardSomething = true;

        // Like with any investigation, we set our stoppingDistance to 0 to ensure our AI doesn't get caught up on trying to reach their destination.
        agent.stoppingDistance = 0;
        // We then set our destination to our determined location.
        agent.SetDestination(HeardSoundLocation);
        // We want this coroutine to last while we're still investigating our sound, so this will allow us to stay in the coroutine until we arrive.
        while (agent.remainingDistance <= 0.25f)
        {
            yield return null;
        }

        // Finally, we wait for the time specified by our RoamTimer. This results in double the waiting period compared to roaming normally. This is deliberate.
        yield return new WaitForSeconds(RoamTimer);

        IHeardSomething = false;


    }

    // Method that handles updating the animation of the AI.
    void UpdateAnim()
    {
        // We want to know the absolute value of our character's normalized speed for the purposes of our animator, so we get the normalized magnitude of our velocity.
        float normAgentSpeed = agent.velocity.normalized.magnitude;

        // We also want to know what the current speed of our animation is.
        float currAnimSpeed = EnemyAnimator.GetFloat("Speed");

        // Now that we have that animation, we now update our Animator's speed float with that agent speed, allowing us to accurately blend the animations.
        EnemyAnimator.SetFloat("Speed", Mathf.Lerp(currAnimSpeed, normAgentSpeed, Time.deltaTime * AnimTransitionSpeed));
    }

    // Public Interface method that notifies enemies to this object's location.
    public void AlertEnemies()
    {
        /* We need to find all of the enemies in this object's range. Because this is an enemy, we want it to notify enemies in a much larger radius.
           To do this, we create a Collider array that is populated from an OverlapSphere.*/
        Collider[] hitObjects = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius * 2, 7);

        // For each collider in the array we've generated:
        foreach (Collider collider in hitObjects)
        {
            // We check for an IHearSounds component.
            IHearSounds heardSomething = collider.GetComponent<IHearSounds>();

            /* If the component exists, we call its ReactToSound method, passing in a random location around the object that was destroyed.
            By adding a minimum roam distance, we limit the likelihood that all of the enemies try to path to the same location. */
            heardSomething?.ReactToSound(((Random.insideUnitSphere * investigationRadius) + minRoamDist * 2) + transform.position);
        }
    }
}
