using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//CODY JANDES CREATED THIS SCRIPT 

// Small modification made by Yoander Ferrer to support projectile shooting.

public class PlayerController : MonoBehaviour, TakesDamage
{
    //CODY JANDES CREATED THIS SCRIPT 

    [Header("-----Components-----")]
    //ignore player mask 
    [SerializeField] LayerMask ignoreMask;

    //Initiale character controller into script
    [SerializeField] CharacterController controller;

    [Header("-----Health Stats-----")]
    //Player HP
    [SerializeField] [UnityEngine.Range(1,10)] int HP;


    [Header("-----Movement-----")]
    //Player speed
    [SerializeField][UnityEngine.Range(1, 10)] int speed;

    //Sprint Mod
    [SerializeField][UnityEngine.Range(1, 5)] int sprintMod;

    //Jump counter (max times you can jump
    [SerializeField][UnityEngine.Range(1, 3)] int jumpMax;

    //How fast we can jump
    [SerializeField][UnityEngine.Range(5, 15)] int jumpSpeed;

    //Setting gravity value
    [SerializeField][UnityEngine.Range(25, 50)] int gravity;


    [Header("-----Combat-----")]

    [SerializeField] List<GunStats> gunList = new List<GunStats>();
    //Shoot damage amount
    [SerializeField][UnityEngine.Range(0, 50)] int shootDamage;

    //Rate of fire
    [SerializeField][UnityEngine.Range(0, 1)] float shootRate;

    [SerializeField][UnityEngine.Range(1, 1000)] int shootDistance;

    [SerializeField] GameObject gunModel;

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

    //Int to keep track of where we are in list
    int selectedGun;

    // Editor exposed variable that stores the bullet that the player will fire. - Yoander
    [SerializeField] GameObject bullet;

    // Editor exposed variable that stores the postion the bullet will fire from. - Yoander
    [SerializeField] Transform shootPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOriginal = HP;
        updatePlayerUI();
        
    }

    // Update is called once per frame
    void Update()
    {

        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.red);

        if(!GameManager.instance.isPaused)
        {
            movement();
            selectGun();
        }

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
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDistance, ~ignoreMask))
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

        // Instead of using a raycast, this allows us to fire a projectile right at where our camera is aimed, based on a determined shootPos! - Yoander
        //REMOVED BULLET SO THAT WE CAN USE EFFECTS FROM LECTURE 6

        //Instantiate(bullet, shootPos.position, Camera.main.transform.rotation);

        //Wait for shoot to finish 
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void TakeSomeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();
        StartCoroutine(flashDamage());


        //I am Dead
        if (HP <= 0)
        {
            GameManager.instance.youLose();
        }
    }

    public void updatePlayerUI()
    {
        GameManager.instance.playerHPBar.fillAmount = (float)HP / HPOriginal;
    }

    IEnumerator flashDamage()
    {
        GameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(.1f);
        GameManager.instance.playerDamageScreen.SetActive(false);
    }

    public void getGunStats(GunStats gun)
    {

        //Add gun to list
        gunList.Add(gun);

        //when we pick up a gun its now not at the end
        selectedGun = gunList.Count - 1;

        //Transfer stats to player (Reciever)
        shootDamage = gun.shootDamage;
        shootDistance = gun.shootDistance;
        shootRate = gun.shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh; //set the model on the player 
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial; //rendered passed next
    }

    void selectGun()
    {
        //use scrollwheel to flip through guns
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count -1)
        {
            selectedGun++;
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
        {
            selectedGun--;
            changeGun();
        }
    }

    void changeGun()
    {
        shootDamage = gunList[selectedGun].shootDamage;
        shootDistance = gunList[selectedGun].shootDistance;
        shootRate = gunList[selectedGun].shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].gunModel.GetComponent<MeshFilter>().sharedMesh; //set the model on the player 
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].gunModel.GetComponent<MeshRenderer>().sharedMaterial; //rendered passed next
    }
}
