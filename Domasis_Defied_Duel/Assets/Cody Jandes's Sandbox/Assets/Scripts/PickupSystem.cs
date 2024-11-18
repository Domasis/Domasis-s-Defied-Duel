using UnityEngine;

public class PickupSystem : MonoBehaviour
{

    enum pickupType { gun, Health, DamageBoost} //enum options

    [SerializeField] pickupType type; //this will give us which type we are getting

    [SerializeField] GunStats gun;

     //pauseeeee

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
        if(other.CompareTag("Player"))
        {
            GameManager.instance.playerScript.getGunStats(gun);
            Destroy(gameObject);
        }
    }
}

