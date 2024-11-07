using System.Collections;
using UnityEngine;
public class HeyCharMovePls : MonoBehaviour, TakesDamage
{
    // Values in Unity can't just be made private by declaring them private. To do this, we use [SerializeField].

    [SerializeField] int HP;

    // Editor exposed CharacterController class instance.
    [SerializeField] CharacterController controller;

    // Editor exposed variable that tracks the player's speed.
    [SerializeField] float speed;

    // Editor exposed variable that stores the modifier to the player's speed when they are sprinting.
    [SerializeField] float sprintMod;

    // Editor exposed variable that stores the maximum number of jumps that the player can make.
    [SerializeField] int jumpMax;

    // Editor exposed variable that stores the player's jump speed.
    [SerializeField] int jumpSpeed;

    // Editor exposed variable that stores the force of gravity on the player. Without this, we'd float off into space!
    [SerializeField] int gravity;

    // Editor exposed variable that stores the damage dealt by a projectile.
    [SerializeField] int shootDmg;

    // Editor exposed variable that stores the rate of fire.
    [SerializeField] float fireRate;

    // Editor exposed variable that stores the distance that the projectiles fired from this character can travel.
    [SerializeField] int shootDist;

    // Editor exposed variable that stores the mask that the raycast will ignore.
    [SerializeField] LayerMask ignoreMask;

    // Editor exposed variable that stores the bullet that the player will fire.
    [SerializeField] GameObject bullet;

    // Editor exposed variable that stores the postion the bullet will fire from.
    [SerializeField] Transform shootPos;

    // Because our character needs to move in 3D space, we need a 3 dimensional vector. Luckily, Unity has a class for this, Vector3. Vector2 is the class for 2D vectors.
    Vector3 moveDir;

    // In order to jump, we need a Vector3 that can be used to store the magnitude(direction) of the player's jump!
    Vector3 jumpVelocity;

    // Variable that tracks if the player is sprinting.
    bool isSprinting;

    bool isShooting;

    // Variable that tracks the number of times the player has jumped.
    int jumpCount;


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
        // To keep our code clean and improve modularity, we move the logic for movement to the necessary helper methods.
        MoveChar();
        Sprint();

    }

    // Helper method that handles whenever the character is sprinting.
    void Sprint()
    {
        // Input.GetButtonDown checks if a button is being held, while Input.ButtonDown merely checks if the button was pressed.
        if (Input.GetButtonDown("Sprint"))
        {
            // If the sprint button is being held, we multiply our speed by our sprintMod, and set isSprinting to true.
            speed *= sprintMod;
            isSprinting = true;

        }

        // Input.GetButtonUp works like GetButtonDown, but in reverse, checking if the button was released.
        else if (Input.GetButtonUp("Sprint"))
        {
            // If the button was released, we reset our speed back to its original value, and set isSprinting to false.
            speed /= sprintMod;
            isSprinting = false;

        }
    }

    // Helper method that handles whenever the player wants to jump
    void Jump()
    {
        // If our player is currently attempting to jump by holding the jump button, and the jump count is not at its maximum:
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            // We increment the jump count and increase our character's y-velocity, causing our character to ascend, the first part of a jump!
            jumpCount++;
            jumpVelocity.y = jumpSpeed;

        }

    }

    // Helper method that handles character movement.
    void MoveChar()
    {
        // If our character is on the ground, we reset the jump count and our y-velocity, so that they can jump without any issues.
        if (controller.isGrounded)
        {
            jumpCount = 0;
            jumpVelocity = Vector3.zero;
        }
        /* We initialize our moveDir, passing in two different Input.GetAxis instances, one Horizontal, and one Vertical. 
                 * We pass in 0 for Y, as we will handle jumping elsewhere.
                 * Note: GetAxis takes a string for the axis, and it is CASE SENSITIVE.*/
        //moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        moveDir = (transform.forward * Input.GetAxis("Vertical")) +
                  (transform.right * Input.GetAxis("Horizontal"));

        /* To ensure that our characters move at a constant rate, we multiply our Vector3 by our speed value, multiplied by delta time.
         * Delta time ensures that our character's movement is not dependent on device framerate, and that they will move at a constant rate.
        
        transform.position += moveDir * speed * Time.deltaTime;

        The problem is that changing transforms like this ignores collision.
        */

        // To solve this, we use the CharacterController's position method, which takes in a Vector3 argument. We multiply this by our speed and deltaTime to get our movement.
        controller.Move(moveDir * speed * Time.deltaTime);

        // We then call our jump method, which will check if we're currently attempting to jump.
        Jump();

        // After we jump, we lower our character's y-velocity through the use of our gravity. If we don't zero this out though like we do at the start, we can end up with MASSIVE negative y-velocity, eventually disabling our ability to jump.
        controller.Move(jumpVelocity * Time.deltaTime);
        jumpVelocity.y -= gravity * Time.deltaTime;

        // Finally, if we're attempting to shoot, we call Shoot() as a coroutine, which is a way to call methods whose execution can be paused, like IEnumerators.
        if (Input.GetButtonDown("Fire1") && !isShooting)
        {
            // Coroutines are REQUIRED to call IEnumerator methods, as just calling the method itself will not work properly.
            StartCoroutine(Shoot());
        }
    }

    // IEnumerator methods are Interface Enumerator methods, the execution of which can be paused and continued as needed.
    IEnumerator Shoot()
    {
        // We set isShooting to true, as we are shooting.
        isShooting = true;

        //// We create a RaycastHit class instance hit, which will get the data we need when the shot collides.
        //RaycastHit hit;

        //// Physics.Raycast has a few components in its constructor: (The origin point, the point to rotate around, the RayCastHit instance, the length of the Raycast, and a LayerMask to ignore)
        //// If our Raycast collides with an object in range:
        //if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreMask))
        //{
        //    Debug.Log(hit.collider.name);
        //    // We get an IDamage instance from the object the raycast collided with.
        //    TakesDamage dmg = hit.collider.GetComponent<TakesDamage>();

        //    // If we were successful in getting that IDamage instance:
        //    dmg?.TakeSomeDamage(shootDmg);
        //}

        Instantiate(bullet, shootPos.position, Camera.main.transform.rotation);

        yield return new WaitForSeconds(fireRate);
        isShooting = false;
    }



    public void TakeSomeDamage(int amount)
    {
        HP -= amount;

        StartCoroutine(FlashDmg());

        // If our HP is at or below 0, we're dead!
        if (HP <= 0)
        {
            GameManager.instance.youLose();
        }
    }

    IEnumerator FlashDmg()
    {
        GameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.playerDamageScreen.SetActive(false);
    }

}
