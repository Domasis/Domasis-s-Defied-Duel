
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
public class OffensiveEnemyController : LiveActor, TakesDamage, IHearSounds
{
    // IEnemyState derived class instances that contain the logic for each state the enemy is in.
    private static OnAttackState attackState = new();
    private static OnRoamState roamState = new();
    private static OnInvestigateState investigateState = new();

    [Header("Enemy Offensive Stats and State")]

    // For debug purposes, stores the currentState so that it can be clearly seen in the editor.
    [SerializeField] private string currentStateName;

    // IEnemyState instance variable that stores our current state.
    private IEnemyState currentState;

    [SerializeField] Transform shootPos;

    // Editor exposed float that tracks the AI's rate of fire.
    [SerializeField][Range(0.6f, 2.0f)] float shootRate;

    // Editor exposed GameObject that contains our bullet, which our AI will fire.
    [SerializeField] GameObject bullet;

    [Header("Animation and Motion")]
    // Editor exposed Navigational Mesh Agent that allows our enemy to intelligently path and roam.
    [SerializeField] NavMeshAgent agent;

    // Animator reference in our code. Required to access modifiers to animations.
    [SerializeField] Animator animator;

    // Color instance that represents the color that our character will flash when hit.
    Color dmgColor;

    // Int that tracks the rate at which our animations will transition between.
    [SerializeField] int animTransitionSpeed;

    [Header("Roam and Investigation Attributes")]

    // Float that tracks the distance that our AI can roam to.
    [SerializeField] float roamDistance;

    // Integer that tracks the rate at which our AI will roam from location to location.
    [SerializeField] int roamTimer;

    // Vector3 that stores the minimum distance that the AI can roam to.
    [SerializeField] Vector3 minRoamDist;

    // Int that stores the maximum range that our Agent's roam priority can be set to (exclusive).
    [SerializeField] [Range(10, 100)] int roamPrioMax;

    float originalStoppingDistance;

    // Vector3 that stores the AI's location on spawn.
    Vector3 origLocation;

    Vector3 investigationLoc;

    [Header("----- On Death Investigation Radius -----")]

    // Sets a range around the object's transform position that enemies can roam within.
    [SerializeField][Range(1, 4)] int investigationRadius;

    // Group of booleans that track the various states, statuses, and actions being taken by our AI.
    bool isRoaming, isInvestigating, isAttacking, wasHit, heardSomething, isShooting;

    public bool IsRoaming { get => isRoaming; set => isRoaming = value; }
    public bool IsInvestigating { get => isInvestigating; set => isInvestigating = value; }
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
    public float RoamDistance { get => roamDistance; set => roamDistance = value; }
    public int RoamTimer { get => roamTimer; set => roamTimer = value; }
    public Vector3 MinRoamDist { get => minRoamDist; set => minRoamDist = value; }
    public NavMeshAgent Agent { get => agent; set => agent = value; }
    public Animator Animator { get => animator; set => animator = value; }
    public string CurrentStateName { get => currentStateName; set => currentStateName = value; }
    public IEnemyState CurrentState { get => currentState; set => currentState = value; }
    public bool WasHit { get => wasHit; set => wasHit = value; }
    public bool HeardSomething { get => heardSomething; set => heardSomething = value; }
    public static OnAttackState AttackState { get => attackState; set => attackState = value; }
    public static OnRoamState RoamState { get => roamState; set => roamState = value; }
    public static OnInvestigateState InvestigateState { get => investigateState; set => investigateState = value; }
    public Vector3 OrigLocation { get => origLocation; set => origLocation = value; }
    public int RoamPrioMax { get => roamPrioMax; set => roamPrioMax = value; }
    public Color DmgColor { get => dmgColor; set => dmgColor = value; }
    public int InvestigationRadius { get => investigationRadius; set => investigationRadius = value; }
    public Vector3 InvestigationLoc { get => investigationLoc; set => investigationLoc = value; }
    public int AnimTransitionSpeed { get => animTransitionSpeed; set => animTransitionSpeed = value; }
    public float ShootRate { get => shootRate; set => shootRate = value; }
    public GameObject Bullet { get => bullet; set => bullet = value; }
    public bool IsShooting { get => isShooting; set => isShooting = value; }
    public float OriginalStoppingDistance { get => originalStoppingDistance; set => originalStoppingDistance = value; }




    // OnEnable is called once before the first execution of Start after the MonoBehaviour is created, as well as whenever a GameObject is enabled.
    void OnEnable()
    {
        CurrentState = RoamState;
    }

    new

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();

        if (Agent == null)
        {
            Agent = GetComponent<NavMeshAgent>();
        }

        if (Animator == null)
        {
            Animator = GetComponent<Animator>();
        }

        // If all agents have the same priority, they won't ignore each other when needed, so we randomize it to a modifiable value.
        Agent.avoidancePriority = Random.Range(0, RoamPrioMax);

        OriginalStoppingDistance = Agent.stoppingDistance;

        // Finally, we get the object's spawn location as its original location, so that our roam knows what point to roam around from.
        OrigLocation = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnim();
        ManageState();
    }

    public void TakeSomeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(FlashRed());
        InvestigationLoc = GameManager.instance.player.transform.position;
        WasHit = true;
        
    }

    public void React(Vector3 invokingLocation)
    {
        if (!IsInvestigating)
        {
            HeardSomething = true;
            InvestigationLoc = invokingLocation;

        }
    }

    // IEnumerator method that allows our AI to roam around its spawn point.
    public IEnumerator Roam()
    {
        // We set IsRoaming to true to prevent concurrent calls breaking our AI's ability to roam.
        IsRoaming = true;

        // We then pause execution of our method for a set amount of time.
        yield return new WaitForSeconds(RoamTimer);

        // We set our Agent's stopping distance to 0 to ensure it can reach its destination.
        Agent.stoppingDistance = 0;

        // We sample a random location within a minimum roam distance by generatng a random point inside of a unit sphere, multiplied by its allotted roam distance.
        Vector3 randomLoc = MinRoamDist + (Random.insideUnitSphere * RoamDistance);

        // We add this random location to our enemy's Original Location to ensure it's always roaming around the same area.
        randomLoc += OrigLocation;

        // We then sample the position generated by our previous calculations. The NavMeshHit generated will be populated with valid position data if the sampled location is valid.
        NavMesh.SamplePosition(randomLoc, out NavMeshHit hit, RoamDistance, 1);

        // We then set our destination for our Agent to this new hit position. If the position wasn't valid, this will simply not set a destination.
        Agent.SetDestination(hit.position);

        while (agent.remainingDistance <= 0.25f)
        {
            yield return null;
        }

        IsRoaming = false;
    }

    IEnumerator FlashRed()
    {
        Model.material.color = DmgColor;

        yield return new WaitForSeconds(0.2f);

        Model.material.color = OrigColor;

        if (agent.pathPending)
        {
            while (agent.pathPending)
            {
                WasHit = true;
                yield return null;
            }
        }
    }

    void ManageState()
    {
        CurrentState = CurrentState.HandleState(this);
        CurrentStateName = CurrentState.ToString();
    }

    // Method that handles updating the AI's animation.
    void UpdateAnim()
    {
        // We want to know the absolute value of our character's normalized speed for the purposes of our animator, so we get the normalized magnitude of our velocity.
        float normAgentSpeed = agent.velocity.normalized.magnitude;

        // We also want to know what the current speed of our animation is.
        float currAnimSpeed = Animator.GetFloat("Speed");

        // Now that we have that animation, we now update our Animator's speed float with that agent speed, allowing us to accurately blend the animations.
        Animator.SetFloat("Speed", Mathf.Lerp(currAnimSpeed, normAgentSpeed, Time.deltaTime * AnimTransitionSpeed));
    }

    public IEnumerator Investigate()
    {
        // To ensure we don't investigate multiple things at once, we set both investigation bools to true.
        IsInvestigating = true;

        // Like with any investigation, we set our stoppingDistance to 0 to ensure our AI doesn't get caught up on trying to reach their destination.
        agent.stoppingDistance = 0;
        // We then set our destination to our determined location.
        agent.SetDestination(InvestigationLoc);
        // We want this coroutine to last while we're still investigating our sound, so this will allow us to stay in the coroutine until we arrive.
        while (agent.remainingDistance <= 0.25f)
        {
            yield return null;
        }

        // Finally, we wait for the time specified by our RoamTimer. This results in double the waiting period compared to roaming normally. This is deliberate, as investigations should take longer.
        yield return new WaitForSeconds(RoamTimer);

        IsInvestigating = WasHit = HeardSomething = false;

    }

    public IEnumerator Shoot()
    {
        // To prevent entering this coroutine prematurely, we set IsShooting to true, which locks it out in Update.
        IsShooting = true;

        // We set our animator's shoot trigger, which causes the enemy to play the shoot animation.
        Animator.SetTrigger("Shoot");

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
}
