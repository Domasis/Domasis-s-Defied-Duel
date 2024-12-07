using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sheldonplayercontroller : MonoBehaviour, TakesDamage
{
    [Header("-----Components-----")]
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] CharacterController controller;

    [Header("-----Health Stats-----")]
    [SerializeField][UnityEngine.Range(1, 10)] int HP;
    [SerializeField][UnityEngine.Range(1, 10)] int armor;

    [Header("-----Movement-----")]
    [SerializeField][UnityEngine.Range(1, 10)] int speed;
    [SerializeField][UnityEngine.Range(1, 5)] int sprintMod;
    [SerializeField][UnityEngine.Range(1, 3)] int jumpMax;
    [SerializeField][UnityEngine.Range(5, 15)] int jumpSpeed;
    [SerializeField][UnityEngine.Range(25, 50)] int gravity;

    [Header("-----Combat-----")]
    [SerializeField] List<GunStats> gunList = new List<GunStats>();
    [SerializeField][UnityEngine.Range(0, 50)] int shootDamage;
    [SerializeField][UnityEngine.Range(0, 1)] float shootRate;
    [SerializeField][UnityEngine.Range(1, 1000)] int shootDistance;
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject muzzleFlash;

    [Header("-----Audio-----")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audJump;
    [SerializeField][UnityEngine.Range(0, 1)] float audJumpVolume;
    [SerializeField] AudioClip[] audHurt;
    [SerializeField][UnityEngine.Range(0, 1)] float audHurtVolume;
    [SerializeField] AudioClip[] audFootsteps;
    [SerializeField][UnityEngine.Range(0, 1)] float audFootstepVolume;

    Vector3 movePlayer;
    Vector3 playerVelocity;
    bool isSprinting;
    bool isShooting;
    bool isPlayingSteps;
    int jumpCount;
    int HPOriginal;
    int armorOriginal;
    int selectedGun;

    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;

    void Start()
    {
        HPOriginal = HP;
        armorOriginal = armor;
        spawnPlayer();
    }

    void Update()
    {
        if (!sheldonGameManager.instance.isPaused)
        {
            movement();
            selectGun();
            reload();
        }
        sprint();
    }

    public void spawnPlayer()
    {
        controller.enabled = false;
        transform.position = sheldonGameManager.instance.GetPlayerSpawnPoint().transform.position;
        controller.enabled = true;

        HP = HPOriginal;
        armor = armorOriginal;
        updatePlayerUI();
    }

    void movement()
    {
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVelocity = Vector3.zero;
        }

        movePlayer = (transform.forward * Input.GetAxis("Vertical")) +
                     (transform.right * Input.GetAxis("Horizontal"));
        controller.Move(movePlayer * speed * Time.deltaTime);
        jump();
        controller.Move(playerVelocity * Time.deltaTime);
        playerVelocity.y -= gravity * Time.deltaTime;

        if (Input.GetButton("Fire1") && gunList.Count > 0 && gunList[selectedGun].ammoCurrent > 0 && !isShooting)
        {
            StartCoroutine(shoot());
        }

        if (controller.isGrounded && movePlayer.magnitude > 0.3f && !isPlayingSteps)
        {
            StartCoroutine(playFootsteps());
        }
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVelocity.y = jumpSpeed;
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVolume);
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

    IEnumerator playFootsteps()
    {
        isPlayingSteps = true;
        aud.PlayOneShot(audFootsteps[Random.Range(0, audFootsteps.Length)], audFootstepVolume);
        yield return new WaitForSeconds(isSprinting ? 0.25f : 0.5f);
        isPlayingSteps = false;
    }

    IEnumerator shoot()
    {
        isShooting = true;
        gunList[selectedGun].ammoCurrent--;
        sheldonGameManager.instance.updateAmmoCounttt(gunList[selectedGun].ammoCurrent);
        aud.PlayOneShot(gunList[selectedGun].shootSound[Random.Range(0, gunList[selectedGun].shootSound.Length)], gunList[selectedGun].shootVolume);
        StartCoroutine(flashMuzzle());

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDistance, ~ignoreMask))
        {
            TakesDamage damage = hit.collider.GetComponent<TakesDamage>();
            if (damage != null)
            {
                damage.TakeSomeDamage(shootDamage);
            }
        }

        Instantiate(gunList[selectedGun].hitEffect, hit.point, Quaternion.identity);
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
        if (armor > 0)
        {
            int remainingDamage = amount - armor;
            armor = Mathf.Max(armor - amount, 0);

            if (remainingDamage > 0)
                HP -= remainingDamage;
        }
        else
        {
            HP -= amount;
        }

        aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVolume);
        updatePlayerUI();

        if (HP <= 0)
        {
            sheldonGameManager.instance.youLose();
        }
    }

    public void updatePlayerUI()
    {
        sheldonGameManager.instance.playerHPBar.fillAmount = (float)HP / HPOriginal;
        sheldonGameManager.instance.playerArmorBar.fillAmount = (float)armor / armorOriginal; // Add UI update for armor
    }

    public void PickUpArmor(int armorAmount)
    {
        armor = Mathf.Min(armor + armorAmount, armorOriginal);
        updatePlayerUI();
    }

    public void getGunStats(GunStats gun)
    {
        gunList.Add(gun);
        selectedGun = gunList.Count - 1;
        shootDamage = gun.shootDamage;
        shootDistance = gun.shootDistance;
        shootRate = gun.shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;

        sheldonGameManager.instance.updateAmmoCounttt(gunList[selectedGun].ammoCurrent);
    }

    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count - 1)
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

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].gunModel.GetComponent<MeshRenderer>().sharedMaterial;

        sheldonGameManager.instance.updateAmmoCounttt(gunList[selectedGun].ammoCurrent);
    }

    void reload()
    {
        if (Input.GetButtonDown("Reload") && gunList.Count > 0)
        {
            gunList[selectedGun].ammoCurrent = gunList[selectedGun].ammoMax;
            sheldonGameManager.instance.updateAmmoCounttt(gunList[selectedGun].ammoCurrent);
        }
    }
}
