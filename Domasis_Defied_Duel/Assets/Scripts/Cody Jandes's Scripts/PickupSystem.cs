using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class PickupSystem : MonoBehaviour
{

    enum pickupType { gun, Health, Keycard} //enum options

    [SerializeField] pickupType type; //this will give us which type we are getting

    [SerializeField] GunStats gun;

    // YF - Health gain added.
    [SerializeField] [Range(0, 3)] int healthGain;

    [SerializeField] AudioClip drinkSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(type == pickupType.gun)
        {
            gun.ammoCurrent = gun.ammoMax;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && type == pickupType.gun)
        {
            GameManager.instance.playerScript.getGunStats(gun);
            Destroy(gameObject);
        }

        // If it's a health pickup, grabbing it will cause the player to heal for a pre-determined amount.
        if(other.CompareTag("Player") && type == pickupType.Health)
        {
            GameManager.instance.playerScript.Health += healthGain;
            GameManager.instance.playerScript.updatePlayerUI();
            GameManager.instance.playerScript.Drink(drinkSound);
            Destroy(gameObject);
        }
       
    }
}

