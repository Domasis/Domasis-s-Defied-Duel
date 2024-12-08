using UnityEngine;
using UnityEngine.Audio;

public class MenuFeedbackSoundsController : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip hoverSound;
    [SerializeField] AudioClip clickSound;
    [SerializeField] AudioMixer audioMixer;

    public void PlayHoverSound()
    {
        PlaySound(hoverSound);
    }

    public void PlayClickSound()
    {
        PlaySound(clickSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null || audioSource == null)
        
            return;
        audioSource.PlayOneShot(clip);
    }
}
