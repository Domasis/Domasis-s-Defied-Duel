using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Transform))]
public class LiveActor : MonoBehaviour
{
    // Renderer component required to render object in world space.
    Renderer model;

    // Sphere collider, required for interactions between the player and other objects.
    SphereCollider actorCollider;

    [SerializeField] Transform headPos;

    [Header("Player Facing Values")]

    // Integer that stores the speed at which the liveActor will turn to rotate towards the player.
    [SerializeField] [Range(2, 15)] int faceTargetSpeed;

    [SerializeField] [Range(75, 160)] float enemyViewConeAngle;

    float angleToPlayer;

    Vector3 playerDir;

    [Header("Enemy Health")]
    // Integer that stores the object's health.
    [SerializeField] [Range(0, 25)] int hp;

    // Boolean that stores whether the player is in range.
    bool playerInRange;



    // Getters and Setters for member properties.
    public Renderer Model { get => model; set => model = value; }
    public SphereCollider ActorCollider { get => actorCollider; set => actorCollider = value; }
    public int FaceTargetSpeed { get => faceTargetSpeed; set => faceTargetSpeed = value; }
    public Vector3 PlayerDir { get => playerDir; set => playerDir = value; }
    public int HP { get => hp; set => hp = value; }
    public float AngleToPlayer { get => angleToPlayer; set => angleToPlayer = value; }
    public float EnemyViewConeAngle { get => enemyViewConeAngle; set => enemyViewConeAngle = value; }
    public bool PlayerInRange { get => playerInRange; set => playerInRange = value; }
    public Transform HeadPos { get => headPos; set => headPos = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        Model = GetComponent<Renderer>();

        ActorCollider = GetComponent<SphereCollider>();

        if (!ActorCollider.isTrigger)
        {
            ActorCollider.isTrigger = true;
        }
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInRange = false;
        }
    }

    // Public method that handles GameObject rotation towards the player.
    public void FacePlayer()
    {
        // We get a quaternion from the LookRotation of the playerDir.
        Quaternion rot = Quaternion.LookRotation(PlayerDir);

        // We then rotate the enemy AI using a lerp, which lerps from the model's current rotation to the rot, in deltaTime.
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, FaceTargetSpeed * Time.deltaTime);
    }

    public bool CanSeePlayer()
    {
        bool playerIsVisible = false;

        PlayerDir = GameManager.instance.player.transform.position - HeadPos.position;

        AngleToPlayer = Vector3.Angle(PlayerDir, transform.forward);

        if (Physics.Raycast(HeadPos.position, PlayerDir, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Player") && AngleToPlayer <= EnemyViewConeAngle)
            {
                playerIsVisible = true;
            }

        }

        return playerIsVisible;
    }
}
