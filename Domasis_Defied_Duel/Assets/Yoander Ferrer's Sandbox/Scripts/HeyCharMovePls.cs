using System.Collections;
using UnityEngine;
public class HeyCharMovePls : MonoBehaviour, GetHitSon
{
    // Editor exposed variable that stores the Player's HP.
    [SerializeField] int HP;

    // Editor exposed CharacterController that is required for player motion.
    [SerializeField] CharacterController playerController;

    // Editor exposed variable that tracks the player's speed.
    [SerializeField] float speed;

    // Editor exposed variable that stores the modifier to the player's speed when they are sprinting!
    [SerializeField] float sprintMod;

    // Editor exposed variable that tracks the max number of jumps that the player can make.
    [SerializeField] int maxJumps;

    // Editor exposed variable that tracks the speed at which the player jumps.
    [SerializeField] int jumpSpeed;

    // Editor exposed variable that tracks the force of gravity on the player.
    [SerializeField] int gravity;

    // Editor exposed variable that stores the damage our shots will deal.
    [SerializeField] int shootDmg;

    // Editor exposed variable that stores the rate at which we fire.
    [SerializeField] float fireRate;

    // Editor exposed variable that tracks how far we can shoot.
    [SerializeField] int shootDist;

    // Editor exposed variable that tracks the Layer that we will ignore.
    [SerializeField] LayerMask ignoreMask;

    // Vector3 that stores the direction in which we are moving.
    Vector3 moveDir;

    // Vector3 that stores the velocity of our jump.
    Vector3 jumpVelocity;

    // Boolean that tracks whether our character is currently sprinting.
    bool isSprinting;

    // Boolean that tracks whether our character is currently shooting.
    bool isShooting;

    // Integer that tracks our current number of jumps.
    int jumpCount;

    public int HP1 { get => HP; set => HP = value; }

    public CharacterController PlayerController { get => playerController; set => playerController = value; }

    public float Speed { get => speed; set => speed = value; }

    public float SprintMod { get => sprintMod; set => sprintMod = value; }

    public int MaxJumps { get => maxJumps; set => maxJumps = value; }

    public int JumpSpeed { get => jumpSpeed; set => jumpSpeed = value; }

    public int Gravity { get => gravity; set => gravity = value; }

    public int ShootDmg { get => shootDmg; set => shootDmg = value; }

    public float FireRate { get => fireRate; set => fireRate = value; }

    public int ShootDist { get => shootDist; set => shootDist = value; }

    public LayerMask IgnoreMask { get => ignoreMask; set => ignoreMask = value; }

    public Vector3 MoveDir { get => moveDir; set => moveDir = value; }

    public Vector3 JumpVelocity { get => jumpVelocity; set => jumpVelocity = value; }

    public bool IsSprinting { get => isSprinting; set => isSprinting = value; }

    public bool IsShooting { get => isShooting; set => isShooting = value; }

    public int JumpCount { get => jumpCount; set => jumpCount = value; }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveChar();
        Sprint();
    }

    public void GetThwacked(int amt)
    {
        HP -= amt;

        if (HP <= 0)
        {
            Debug.Break();
        }
    }

    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            Speed *= SprintMod;
        }

    }

    void Jump()
    {

    }

    void MoveChar()
    {
        if (PlayerController.isGrounded)
        {
            JumpCount = 0;
            JumpVelocity = Vector3.zero;
        }

        MoveDir = (transform.forward * Input.GetAxis("Vertical")) + 
                  (transform.right * Input.GetAxis("Horizontal"));

        PlayerController.Move(MoveDir * Speed * Time.deltaTime);

        Jump();


    }

    IEnumerator Shoot()
    {
        yield return new WaitForSeconds(FireRate);
    }
}
