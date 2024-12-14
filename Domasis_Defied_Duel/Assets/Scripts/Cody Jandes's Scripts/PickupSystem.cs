using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class PickupSystem : MonoBehaviour
{
    enum PickupType { Gun, Health, Keycard, Armor } 

    [SerializeField] PickupType type; 

    [SerializeField] GunStats gun;

    // YF - Health gain added.
    [SerializeField][Range(0, 3)] int healthGain;

    // YF - Armor gain added
    [SerializeField][Range(0, 100)] int armorGain; 

    [SerializeField] AudioClip drinkSound;

    void Start()
    {
        if (type == PickupType.Gun)
        {
            gun.ammoCurrent = gun.ammoMax;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (type)
            {
                case PickupType.Gun:
                    GameManager.instance.playerScript.getGunStats(gun);
                    Destroy(gameObject);
                    break;

                case PickupType.Health:
                    GameManager.instance.playerScript.Health += healthGain;
                    GameManager.instance.playerScript.updatePlayerUI();
                    GameManager.instance.playerScript.Drink(drinkSound);
                    Destroy(gameObject);
                    break;

                case PickupType.Armor:
                    GameManager.instance.playerScript.AddArmor(armorGain); 
                    Destroy(gameObject);
                    break;

                  
            }
        }
    }
}
