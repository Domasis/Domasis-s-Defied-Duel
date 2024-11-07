using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, TakesDamage
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
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // We set origColor to the model's color.
        origColor = Model.material.color;

        // We then update the EnemyCount of our PlayerLocator accordingly.
        GameManager.instance.updateGameGoal(1);

    }

    // Update is called once per frame
    void Update()
    {

        AttackThePlayer();

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
        }

    }


    // Required interface method that must be implemented as a part of the IDamage interface. Adjusts enemyAI HP, and destroys it if it falls to or below 0.
    public void TakeSomeDamage(int amt)
    {

        hp -= amt;

        StartCoroutine(FlashRed());

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
    }

    // IEnumerator called in a coroutine, enables the enemyAI to shoot at the player.
    IEnumerator Shoot()
    {
        IsShooting = true;

        Instantiate(bullet, shootPos.position, transform.rotation);

        yield return new WaitForSeconds(ShootRate);

        IsShooting = false;
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
        // If our player is in range of the enemy's sphere collider:
        if (playerInRange)
        {
            // We set our playerDir to the Player's transform position, subtracted by the headPos position to get the player's actual location.
            playerDir = GameManager.instance.player.transform.position - headPos.position;

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
    }
}
