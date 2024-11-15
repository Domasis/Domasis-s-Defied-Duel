using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public interface MakesSound
{

    AudioSource SoundClip { get; set; }

    IEnumerator MakeSomeNoise();

    float SphereMaxRadius { get; set; }
    
}
