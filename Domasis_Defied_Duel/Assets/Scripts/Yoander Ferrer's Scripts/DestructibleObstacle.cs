using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class DestructibleObstacle : MonoBehaviour, TakesDamage, MakesSound
{

    [SerializeField] int hp;

    [SerializeField] Renderer model;

    Color origColor;

    [SerializeField] Color dmgColor;

    AudioSource dmgAudio;

    SphereCollider soundSphere;

    [SerializeField] float sphereMaxRadius;

    float originalSphereRadius;

    [SerializeField] float sphereExpansionRate;

    public int HP { get => hp; set => hp = value; }
    public Color OrigColor { get => origColor; set => origColor = value; }
    public Color DmgColor { get => dmgColor; set => dmgColor = value; }
    public Renderer Model { get => model; set => model = value; }
    public AudioSource SoundClip { get => dmgAudio; set => dmgAudio = value; }
    public SphereCollider SoundSphere { get => soundSphere; set => soundSphere = value; }
    public float SphereMaxRadius { get => sphereMaxRadius; set => sphereMaxRadius = value; }
    public float OriginalSphereRadius { get => originalSphereRadius; set => originalSphereRadius = Mathf.Clamp(value, 0.01f, 15.0f); }
    public float SphereExpansionRate { get => sphereExpansionRate; set => sphereExpansionRate = value; }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        origColor = Model.material.color;

        SoundSphere = GetComponent<SphereCollider>();

        SoundClip = GetComponent<AudioSource>();

        OriginalSphereRadius = SoundSphere.radius;

    }

    public void TakeSomeDamage(int amt)
    {

        HP -= amt;

        StartCoroutine(FlashDmg());

        StartCoroutine(MakeSomeNoise());

        if (HP <= 0)
        {
            Destroy(gameObject);
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return; 
        }

        IHearSounds hearSounds = other.gameObject.GetComponent<IHearSounds>();

        if (hearSounds != null)
        {

            Debug.Log("An object with IHearSounds was detected!");

        }
        
        

    }

    IEnumerator FlashDmg()
    {

        Model.material.color = DmgColor;

        yield return new WaitForSeconds(0.2f);

        Model.material.color = origColor;

    }

    public IEnumerator MakeSomeNoise()
    {

        SoundClip.Play();

        Collider[] hitObjects = Physics.OverlapSphere(transform.position, sphereMaxRadius, 7);

        foreach (Collider collider in hitObjects)
        {
            Debug.Log(collider.name);

            IHearSounds hearSounds = collider.gameObject.GetComponent<IHearSounds>();

            hearSounds?.InvestigateSound(transform.position);

        }

        yield return new WaitForSeconds(0.5f);

    }
}
