using UnityEngine;

[CreateAssetMenu]

public class GunStats : ScriptableObject
{
    //Scriptable game object (data holders) (changed in scene, changed in project)
    public GameObject gunModel;
    public int shootDamage;
    public int shootDistance;
    public float shootRate;
    public int ammoCurrent, ammoMax;

    public ParticleSystem hitEffect; //wherever we shoot, add particle effect
    public AudioClip[] shootSound;
    public float shootVolume;

}
