using System.Collections;
using UnityEngine;

public class DestructibleObstacle1 : MonoBehaviour, TakesDamage, IAlert
{
    [SerializeField] private int hp;
    [SerializeField] private Renderer model;
    [SerializeField] private Color dmgColor;
    [SerializeField] private AudioClip damageSound;  // The sound played on damage
    [SerializeField][Range(0, 1)] private float damageSoundVolume = 0.5f; // Volume of the damage sound
    [SerializeField] [Range(15, 60)] int alertRadius;
    [SerializeField][Range(1, 3)] int investigationRange;
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

        AlertEnemies();

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

    // AlertEnemies implementation done by Yoander Ferrer.
    // Public interface method that alerts nearby enemies to this object's location, causing them to investigate.
    public void AlertEnemies()
    {
        // We need to get the location of every enemy in range. To do this, we populate an array of colliders using an OverlapSphere call.
        Collider[] hitObjects = Physics.OverlapSphere(transform.position, alertRadius, 7);

        // We then iterate through each collider:
        foreach (Collider hitObject in hitObjects)
        {
            // We attempt to get an IHearSounds component from that collider.
            IHearSounds heardSomething = hitObject.GetComponent<IHearSounds>();

            // If that collider is valid (the ? operator is the same as saying if (obj != null)), we call its ReactToSound method.
            heardSomething?.React((Random.insideUnitSphere * investigationRange) + transform.position);

        }
    }
}
