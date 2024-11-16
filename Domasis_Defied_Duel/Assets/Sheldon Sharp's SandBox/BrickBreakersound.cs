using System.Collections;
using UnityEngine;

// SHELDON SHARP

public class BrickBreaker : MonoBehaviour
{
    [SerializeField] private Renderer model;
    [SerializeField] private ParticleSystem breakEffect;

    // Health value for the brick
    [SerializeField] private int health = 1;  // Default health for the brick (can be set in inspector)
    private Color originalColor;
    private Color hitColor = Color.red;

    private GameManager gameManager;

    // Serialized field to assign damage sound in the Inspector
    [SerializeField] private AudioClip damageSound;  // Assign the damage sound in the inspector
    private AudioSource audioSource;  // To play the sound

    void Start()
    {
        originalColor = model.material.color;
       
        audioSource = GetComponent<AudioSource>();  // Get the AudioSource component on this object
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            // Call function to handle the brick being damaged
            TakeDamage(1);

            // Play the damage sound
            PlayDamageSound();

            // Play hit flash effect
            StartCoroutine(FlashHit());

            // Check for win condition in the game
           

            // Destroy the brick if health is 0 or below
            if (health <= 0)
            {
                // Play break effect when the brick is destroyed
                if (breakEffect != null)
                {
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                }

                // Play the damage sound just before destruction
               // PlayDamageSound();

                // Destroy the brick after a short delay to allow the sound to play
                Destroy(gameObject, 0.2f);  // You can adjust this delay based on the sound duration
            }
        }
    }

    private void TakeDamage(int amount)
    {
        health -= amount;  // Decrease the brick's health

        // If health is 0 or below, destroy the brick
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator FlashHit()
    {
        model.material.color = hitColor;  // Change to hit color
        yield return new WaitForSeconds(0.1f);
        model.material.color = originalColor;  // Reset to original color
    }

    // Function to play the damage sound
    private void PlayDamageSound()
    {
        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);  // Play the damage sound once
        }
    }
}
