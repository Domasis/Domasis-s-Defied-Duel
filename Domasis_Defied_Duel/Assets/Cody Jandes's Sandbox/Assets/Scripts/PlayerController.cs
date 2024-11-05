using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //CODY JANDES CREATED THIS SCRIPT 

    //Initiale character controller into script
    [SerializeField] CharacterController controller;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
        if(controller.isGrounded)
        {
            //Make jump count to zero so we can jump again
            jumpCount = 0;
            //Zero out vector3 to avoid negative numbers, reset numebr
            playerVelocity = Vector3.zero;
        }
        //Move player using preset axes
        //KEEP THIS CODE FOR REFERENCE
        //movePlayer = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        //Move player with preset axes and focus controls on where player is looking and moving
        movePlayer = (transform.forward * Input.GetAxis("Vertical")) +
                     (transform.right * Input.GetAxis("Horizontal"));
        //Use the move player and the player controller to move the player and the inputs created
        controller.Move(movePlayer * speed * Time.deltaTime);

        //Jump command
        jump();

        //Computations to make character jump 
        controller.Move(playerVelocity * Time.deltaTime);
        playerVelocity.y -= gravity * Time.deltaTime; //handle gravity into jump pulling down

        //if we left button click and we are not shooting
        if(Input.GetButton("Fire1") && !isShooting)
        {
            StartCoroutine(shoot()); //how to call ienumerator or coroutine
        }
    }

    void jump()
    {
        //if jump pressed and jump count is less than max
        if(Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++; //increment jump
            playerVelocity.y = jumpSpeed; //how fast we can jump and move on y axis
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

    //iEnumerators are timer
    IEnumerator shoot()
    {
        isShooting = true;

        //what the raycast collided with
        RaycastHit hit;

        //Raycast from camera position, to the forward direction, optional hit to return info, how far it goes
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDistance))
        {
            //Whenever we hit something, it will tell us what we hit
            Debug.Log(hit.collider.name);

            //temp variable to return if it can take damage
            TakesDamage damage = hit.collider.GetComponent<TakesDamage>();

            //if it does return it can take damage, apply damage
            if (damage != null)
            {
                damage.TakeSomeDamage(shootDamage);
            }
        }

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
       
 
}
