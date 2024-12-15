using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class HelicopterAudioController : MonoBehaviour
{
    [SerializeField] AudioSource bladeAudio;

    [SerializeField] AudioClip bladeClip;

    bool isPlaying;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPlaying)
        {
            StartCoroutine(playBladeAudio());
        }
    }

    IEnumerator playBladeAudio()
    {
        isPlaying = true;

        bladeAudio.PlayOneShot(bladeClip, 2f);

        yield return new WaitForSeconds(22f);

        isPlaying = false;
    }
}
