using System.Collections;
using UnityEngine;

public class DestructibleObstacle1 : MonoBehaviour, TakesDamage
{
    [SerializeField] private int hp;
    [SerializeField] private Renderer model;
    [SerializeField] private Color dmgColor;
    [SerializeField] private AudioClip damageSound;  // The sound played on damage
    [SerializeField][Range(0, 1)] private float damageSoundVolume = 0.5f; // Volume of the damage sound
    private AudioSource audioSource;

    // Properties
    public int HP
    {
        get => hp;
        set => hp = value;
    }

    public Renderer Model
    {
        get => model;
        set => model = value;
    }

    public Color OrigColor { get; private set; }

    public Color DmgColor
    {
        get => dmgColor;
        set => dmgColor = value;
    }

    public AudioClip DamageSound
    {
        get => damageSound;
        set => damageSound = value;
    }

    public float DamageSoundVolume
    {
        get => damageSoundVolume;
        set => damageSoundVolume = Mathf.Clamp01(value); // Ensures the volume stays between 0 and 1
    }

    public AudioSource AudioSourceComponent
    {
        get => audioSource;
        private set => audioSource = value;
    }

    void Start()
    {
        // Store the original color of the model
        OrigColor = Model.material.color;

        // Add and configure AudioSource component if it doesn't already exist
        AudioSourceComponent = gameObject.GetComponent<AudioSource>();
        if (AudioSourceComponent == null)
        {
            AudioSourceComponent = gameObject.AddComponent<AudioSource>();
        }
        AudioSourceComponent.playOnAwake = false;
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
        if (AudioSourceComponent != null && DamageSound != null)
        {
            AudioSourceComponent.PlayOneShot(DamageSound, DamageSoundVolume);  // Play the damage sound once
        }
    }
}
