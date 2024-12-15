using UnityEngine;

public class ArmorPickup : MonoBehaviour
{
    // Amount of armor to apply when the pickup is collected
    //[SerializeField] private int armorAmount = 20;

    // Method to handle the pickup interaction
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player collides with the pickup
        if (other.CompareTag("Player"))
        {
            // Apply the armor to the player through the GameManager
            if (GameManager.instance != null)
            {
                //GameManager.instance.ApplyArmor(armorAmount);
            }
            else
            {
               // Debug.LogWarning("GameManager instance not found. Armor pickup failed.");
            }

            // Optionally, destroy the pickup after being collected
            Destroy(gameObject);
        }
    }
}
