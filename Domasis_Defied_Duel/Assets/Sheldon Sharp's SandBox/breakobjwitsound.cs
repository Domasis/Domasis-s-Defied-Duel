using System.Collections;
using UnityEngine;

public class DestructibleObstacle1 : MonoBehaviour, TakesDamage
{
    [SerializeField] int hp;
    [SerializeField] Renderer model;
    [SerializeField] Color dmgColor;

    // Serialized fields for damage sound 
    [SerializeField] private AudioClip damageSound;  // The sound played on damage
    private AudioSource audioSource;  // AudioSource to play the sound
   

    // Properties
    public int HP { get => hp; set => hp = value; }
    public Color OrigColor { get; private set; }
    public Color DmgColor { get => dmgColor; set => dmgColor = value; }
    public Renderer Model { get => model; set => model = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Store the original color of the model
        OrigColor = Model.material.color;
        audioSource = GetComponent<AudioSource>();  // Get the AudioSource component attached to the object
    }

    // Function that handles damage to the obstacle
    public void TakeSomeDamage(int amt)
    {
        HP -= amt;  // Decrease the HP by the damage amount

        // Flash the damage effect
        StartCoroutine(FlashDmg());

        // Play the damage sound
        PlayDamageSound();

        // Check if the obstacle is destroyed
        if (HP <= 0)
        {
          

     // Play the destruction sound (same as damage sound)
            PlayDamageSound();

            // Destroy the object after a short delay
            Destroy(gameObject, 0.2f);  // Delay gives time for sound/effect to play
        }
    }

    
    IEnumerator FlashDmg()
    {
        
        Model.material.color = DmgColor;

      
        yield return new WaitForSeconds(0.2f);

        
        Model.material.color = OrigColor;
    }

    // Function to play the damage sound
    private void PlayDamageSound()
    {
        if (audioSource != null && damageSound != null)
        {
            audioSource.Play();  // Play the damage sound once
        }
    }
}
