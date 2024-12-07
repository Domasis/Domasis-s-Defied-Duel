using UnityEngine;

public class ArmorPickup : MonoBehaviour
{
    // Enum to define the type of pickup
    enum PickupType { Gun, Health, Armor, DamageBoost }

    [SerializeField] PickupType type; // Type of pickup (armor, health, gun, etc.)
    [SerializeField] int armorAmount; // Amount of armor this pickup provides

    [SerializeField] GunStats gun; // If the pickup is a gun, this is the gun info

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
            // Get the player controller script (sheldonplayercontroller) attached to the player
            sheldonplayercontroller player = other.GetComponent<sheldonplayercontroller>();

            if (player != null)
            {
                // Handle different pickup types
                switch (type)
                {
                    case PickupType.Gun:
                        GameManager.instance.playerScript.getGunStats(gun);
                        break;

                    case PickupType.Armor:
                        player.PickUpArmor(armorAmount); // Call the method to add armor
                        break;

                    // Add other cases for Health, DamageBoost, etc. if needed
                    case PickupType.Health:
                        // Example: Add health pickup logic here if needed
                        break;

                    case PickupType.DamageBoost:
                        // Example: Add damage boost pickup logic here if needed
                        break;
                }

                // Destroy the pickup after being collected
                Destroy(gameObject);
            }
        }
    }
}
