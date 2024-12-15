using UnityEngine;

public class SingletonAudioManager : MonoBehaviour
{
    public static SingletonAudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;  // for background music
    [SerializeField] private AudioSource soundSource;  // for general sounds

    [Header("Audio Settings")]
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.5f;  // music volume
    [SerializeField, Range(0f, 1f)] private float soundVolume = 1f;    // sound volume

    [Header("Specific Sound Clips")]
    [SerializeField] private AudioClip[] damageSounds;
    [SerializeField] private AudioClip[] hurtSounds;
    [SerializeField] private AudioClip[] shootSounds;
    [SerializeField] private AudioClip[] footstepSounds;

    // Properties with Getters and Setters
    public float MusicVolume
    {
        get => musicVolume;
        set
        {
            musicVolume = Mathf.Clamp(value, 0f, 1f);
            if (musicSource != null)
                musicSource.volume = musicVolume;
        }
    }

    public float SoundVolume
    {
        get => soundVolume;
        set
        {
            soundVolume = Mathf.Clamp(value, 0f, 1f);
            if (soundSource != null)
                soundSource.volume = soundVolume;
        }
    }

    // Ensure there's only one instance of the audio manager
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // Ensure only one instance exists
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);  // Persist through scene changes
    }

    // Play background music 
    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.loop = true;
            musicSource.Play();
            musicSource.volume = musicVolume;
        }
    }

    // Stop background music
    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    // Play oneShot sound
    public void PlaySound(AudioClip soundClip)
    {
        if (soundClip != null)
        {
            soundSource.PlayOneShot(soundClip, soundVolume);
        }
    }

    // Play a sound at a specific 3D position
    public void PlaySoundAtPosition(AudioClip soundClip, Vector3 position)
    {
        if (soundClip != null)
        {
            AudioSource.PlayClipAtPoint(soundClip, position, soundVolume);
        }
    }

    // Play damage sounds
    public void PlayDamageSound()
    {
        if (damageSounds.Length > 0)
        {
            AudioClip selectedSound = damageSounds[Random.Range(0, damageSounds.Length)];
            PlaySound(selectedSound);
        }
    }

    // Play hurt sounds 
    public void PlayHurtSound()
    {
        if (hurtSounds.Length > 0)
        {
            AudioClip selectedSound = hurtSounds[Random.Range(0, hurtSounds.Length)];
            PlaySound(selectedSound);
        }
    }

    // Play shoot sounds
    public void PlayShootSound()
    {
        if (shootSounds.Length > 0)
        {
            AudioClip selectedSound = shootSounds[Random.Range(0, shootSounds.Length)];
            PlaySound(selectedSound);
        }
    }

    // Play footstep sounds 
    public void PlayFootstepSound()
    {
        if (footstepSounds.Length > 0)
        {
            AudioClip selectedSound = footstepSounds[Random.Range(0, footstepSounds.Length)];
            PlaySound(selectedSound);
        }
    }
}