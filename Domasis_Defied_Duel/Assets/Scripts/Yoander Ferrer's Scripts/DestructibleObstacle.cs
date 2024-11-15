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

    [SerializeField] float sphereMaxRadius;

    [SerializeField] int investigationRange;

    public int HP { get => hp; set => hp = value; }

    public Color OrigColor { get => origColor; set => origColor = value; }

    public Color DmgColor { get => dmgColor; set => dmgColor = value; }

    public Renderer Model { get => model; set => model = value; }

    public AudioSource SoundClip { get => dmgAudio; set => dmgAudio = value; }

    public float SphereMaxRadius { get => sphereMaxRadius; set => sphereMaxRadius = value; }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        origColor = Model.material.color;

        SoundClip = GetComponent<AudioSource>();

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

            hearSounds?.ReactToSound((Random.insideUnitSphere * investigationRange) + transform.position);

        }

        yield return new WaitForSeconds(0.5f);

    }
}
