using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Initiale character controller into script
    [SerializeField] CharacterController controller;

    //Player speed
    [SerializeField] int speed;

    //Sprint Mod
    [SerializeField] int sprintMod;

    //Vector3 to move 
    Vector3 movePlayer;

    bool isSprinting;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movement();
        sprint();
        
    }

    void movement()
    {
        //Move player using preset axes
        //KEEP THIS CODE FOR REFERENCE
        //movePlayer = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        //Move player with preset axes and focus controls on where player is looking and moving
        movePlayer = (transform.forward * Input.GetAxis("Vertical")) +
                     (transform.right * Input.GetAxis("Horizontal"));
        //Use the move player and the player controller to move the player and the inputs created
        controller.Move(movePlayer * speed * Time.deltaTime);
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
       
 
}
