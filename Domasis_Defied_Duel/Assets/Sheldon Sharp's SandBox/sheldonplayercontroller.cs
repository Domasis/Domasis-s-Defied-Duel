using System.Collections;
using UnityEngine;

// CODY JANDES CREATED THIS SCRIPT 

// Small modification made by Yoander Ferrer to support projectile shooting.

public class SheldonPlayerController : MonoBehaviour, TakesDamage
{
    // CODY JANDES CREATED THIS SCRIPT 

    //ignore player mask 
    [SerializeField] LayerMask ignoreMask;

    //Initiale character controller into script
    [SerializeField] CharacterController controller;

    //Player HP
    [SerializeField] int HP;

    //Player speed
    [SerializeField] int speed;

    //Sprint Mod
    [SerializeField] int sprintMod;

    //Jump counter (max times you can jump
    [SerializeField] int jumpMax;

    //How fast we can jump
    [SerializeField] int jumpSpeed;

    //Setting gravity value
    [SerializeField] int gravity;

    //Shoot damage amount
    [SerializeField] int shootDamage;

    //Rate of fire
    [SerializeField] float shootRate;

    [SerializeField] int shootDistance;

    //Vector3 to move 
    Vector3 movePlayer;

    //Gravity and keeping track of jump
    Vector3 playerVelocity;

    //Is or is not sprinting
    bool isSprinting;

    //Is or is not shooting
    bool isShooting;

    //Jump counter
    int jumpCount;

    //CHANGE THIS TO GETTER
    int HPOriginal;

    // Editor exposed variable that stores the bullet that the player will fire. - Yoander
    [SerializeField] GameObject bullet;

    // Editor exposed variable that stores the position the bullet will fire from. - Yoander
    [SerializeField] Transform shootPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOriginal = HP;
        // Removed the updatePlayerUI method call
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.red);
        movement();
        sprint();
    }

    void movement()
    {
        if (controller.isGrounded)
        {
            //Make jump count to zero so we can jump again
            jumpCount = 0;
            //Zero out vector3 to avoid negative numbers, reset number
            playerVelocity = Vector3.zero;
        }

        // Move player with preset axes and focus controls on where player is looking and moving
        movePlayer = (transform.forward * Input.GetAxis("Vertical")) +
                     (transform.right * Input.GetAxis("Horizontal"));

        // Use the move player and the player controller to move the player and the inputs created
        controller.Move(movePlayer * speed * Time.deltaTime);

        // Jump command
        jump();

        // Computations to make character jump 
        controller.Move(playerVelocity * Time.deltaTime);
        playerVelocity.y -= gravity * Time.deltaTime; // Handle gravity into jump pulling down

        // If we left button click and we are not shooting
        if (Input.GetButton("Fire1") && !isShooting)
        {
            StartCoroutine(shoot()); // How to call IEnumerator or coroutine
        }
    }

    void jump()
    {
        // If jump pressed and jump count is less than max
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++; // Increment jump
            playerVelocity.y = jumpSpeed; // How fast we can jump and move on y axis
        }
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    // iEnumerators are timer
    IEnumerator shoot()
    {
        isShooting = true;

        // Instantiate bullet at shoot position
        Instantiate(bullet, shootPos.position, Camera.main.transform.rotation);

        // Wait for shoot to finish 
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void TakeSomeDamage(int amount)
    {
        HP -= amount;
        // Removed the updatePlayerUI call
        StartCoroutine(flashDamage());

        // I am Dead
        if (HP <= 0)
        {
            GameManager.instance.youLose();
        }
    }

    IEnumerator flashDamage()
    {
        GameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(.1f);
        GameManager.instance.playerDamageScreen.SetActive(false);
    }
}
