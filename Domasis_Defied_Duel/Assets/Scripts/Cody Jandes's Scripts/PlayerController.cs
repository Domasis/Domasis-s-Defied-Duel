using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.PlayerLoop;


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
    [SerializeField] [UnityEngine.Range(1,10)] int HP; //DONT CHANGE


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

    [SerializeField] GameObject muzzleFlash;

    //Added in audio just in case we cant figure out how to use singleton

    [Header("-----Audio-----")]

    [SerializeField] AudioSource aud; //source

    [SerializeField] AudioClip[] audJump; //the array of sounds

    [SerializeField] [UnityEngine.Range(0, 1)] float audJumpVolume; //volume we want it to play at -- added range to keep normalized between 0 and 1

    [SerializeField] AudioClip[] audHurt;

    [SerializeField] [UnityEngine.Range(0, 1)] float audHurtVolume;

    [SerializeField] AudioClip[] audFootsteps;

    [SerializeField] [UnityEngine.Range(0, 1)] float audFootstepVolume;

    [SerializeField] AudioClip audReload;

    [SerializeField][UnityEngine.Range(0, 1)] float audReloadVolume;

    [Header("-----Armor Stats-----")]
    // Player Armor Level (damage resistance the player has)
    [SerializeField][UnityEngine.Range(0, 100)] int armorLevel; // DONT CHANGE for starting armor
    [SerializeField] int maxArmor = 100; // Maximum armor level



    // Method to show the players current armor percentage
    public float GetArmorPercentage()
    {
        return (float)armorLevel / maxArmor;
    }

    // func to add armor to the player
    public void AddArmor(int armorAmount)
    {
        // Adds the armor to the current armor level
        armorLevel += armorAmount;

        // clamps armor level so it does not exceed the maximum val
        armorLevel = Mathf.Clamp(armorLevel, 0, maxArmor);

        // Debugging to check if armor is applied
        //Debug.Log("Armor Applied: " + armorAmount + ". New Armor Level: " + armorLevel);

        // Optionally, log when armor reaches maximum
        //if (armorLevel == maxArmor)
        {
           // Debug.Log("Armor is at maximum!"); // Log when armor reaches the maximum value
        }

        // Optionally, play an armor pickup sound here if needed
    }

    
     //Vector3 to move 
    Vector3 movePlayer;

    //Gravity and keeping track of jump
    Vector3 playerVelocity;

    //Is or is not sprinting
    bool isSprinting;

    //Is or is not shooting
    bool isShooting;

    //Is or is not playing footsteps
    bool isPlayingSteps;

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

    [SerializeField] AnimateCamera cameraAnim;

    // YF - Public property that makes health accessible by external classes.
    public int Health { get => HP; set => HP = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOriginal = Health;

        spawnPlayer();

        //updatePlayerUI(); took this out as we now call in spawn player
        
    }

    // Update is called once per frame
    void Update()
    {

        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.red);

        if(!GameManager.instance.isPaused)
        {
            movement();
            selectGun();
            reload();
        }

        sprint();
        
    }

    public void spawnPlayer()
    {
        controller.enabled = false; //disbale controller
        transform.position = GameManager.instance.GetPlayerSpawnPoint().transform.position;
        controller.enabled = true;

        Health = HPOriginal;
        updatePlayerUI();
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
        if(Input.GetButton("Fire1") && gunList.Count > 0 && gunList[selectedGun].ammoCurrent > 0 && !isShooting)
        {
            StartCoroutine(shoot()); //how to call ienumerator or coroutine
        }

        if (controller.isGrounded && movePlayer.magnitude > 0.3f && !isPlayingSteps)
        {
            StartCoroutine(playFootsteps());
        }
    }

    void jump()
    {
        //if jump pressed and jump count is less than max
        if(Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++; //increment jump
            playerVelocity.y = jumpSpeed; //how fast we can jump and move on y axis

            //audio lines
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVolume); //random clip from array, from 0 to the length of array, volume we want it to play at
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

    //Create timer for footsteps
    IEnumerator playFootsteps()
    { 
        isPlayingSteps = true;

        aud.PlayOneShot(audFootsteps[Random.Range(0, audFootsteps.Length)], audFootstepVolume);

        if (!isSprinting)
        {
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            yield return new WaitForSeconds(0.25f);
        }

        isPlayingSteps = false;

    }


    //iEnumerators are timer
    IEnumerator shoot()
    {
        isShooting = true;
        gunList[selectedGun].ammoCurrent--;

        //Update UI ammo
        GameManager.instance.updateAmmoCounttt(gunList[selectedGun].ammoCurrent);

        //Gun shot
        aud.PlayOneShot(gunList[selectedGun].shootSound[Random.Range(0, gunList[selectedGun].shootSound.Length)], gunList[selectedGun].shootVolume);

        //Muzzle flash
        StartCoroutine(flashMuzzle());

        //what the raycast collided with
        RaycastHit hit;

        //Raycast from camera position, to the forward direction, optional hit to return info, how far it goes
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDistance, ~ignoreMask))
        {
            //Whenever we hit something, it will tell us what we hit
            //Debug.Log(hit.collider.name);

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

        //Add hit effect for gun
        Instantiate(gunList[selectedGun].hitEffect, hit.point, Quaternion.identity);

        //Wait for shoot to finish 
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    IEnumerator flashMuzzle()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        muzzleFlash.SetActive(false);
    }

    public void TakeSomeDamage(int amount)
    {
        // Need an instance of the model for the purposes of the camera shake.
        Renderer model = GetComponent<Renderer>();

        // If armor is above 0 absorb the damage
        if (armorLevel > 0)
        {
            // how much damage will be absorbed by the armor
            int damageAbsorbed = Mathf.Min(armorLevel, amount); // Armor absorbs damage up to its current level
            armorLevel -= damageAbsorbed; // Decrease the armor by the absorbed damage

            // minus the absorbed damage from the amount
            amount -= damageAbsorbed;
        }

        // After armor reaches 0, apply the remaining damage to players health
        if (amount > 0)
        {
            Health -= amount;
        }

        // Play hurt sound
        aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVolume);

        // Update the UI with the current health and armor val
        updatePlayerUI();

        // If health is 0 or less, trigger player death
        if (Health <= 0)
        {
            GameManager.instance.youLose();
        }

        cameraAnim.StartCoroutine(cameraAnim.ShakeCamera(Camera.main, model));

        // Flash damage screen effect (optional)
        StartCoroutine(flashDamage());
    

    }

    public void updatePlayerUI()
    {
        // Update the health bar based on current health
        GameManager.instance.playerHPBar.fillAmount = (float)Health / HPOriginal;

        // Update the armor bar based on current armor level
        GameManager.instance.playerArmorBar.fillAmount = (float)armorLevel / maxArmor;
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

        GameManager.instance.updateAmmoCounttt(gunList[selectedGun].ammoCurrent);
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

        GameManager.instance.updateAmmoCounttt(gunList[selectedGun].ammoCurrent);
    }

    void reload()
    {
        if(Input.GetButtonDown("Reload") && gunList.Count > 0)
        {
            //relaod to refill the gun 
            gunList[selectedGun].ammoCurrent = gunList[selectedGun].ammoMax;
            GameManager.instance.updateAmmoCounttt(gunList[selectedGun].ammoCurrent);

            aud.PlayOneShot(audReload, audReloadVolume);
        }
    }

    public void Drink(AudioClip drinkAud)
    {
        aud.PlayOneShot(drinkAud, 1f);
    }

    public void DoorOpen(AudioClip doorAud)
    {
        aud.PlayOneShot(doorAud, 1f);
    }
}
