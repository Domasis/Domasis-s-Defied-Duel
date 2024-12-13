using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(SphereCollider))]
public class Objective : MonoBehaviour, IAlert
{

    //CODY JANDES CREATED THIS SCRIPT, edits added by Yoander Ferrer

    [SerializeField] Renderer model;

    [SerializeField] Slider hackProgressSlider;

    [SerializeField] Canvas hackProgressCanvas;

    [SerializeField] TMP_Text warningText;

    float hackProgress = 0;

    [SerializeField] [Range(1, 100)] float maxHackLimit;

    float timeBetweenAlerts = 5f;

    float timer;

    int faceTargetSpeed = 8;

    [SerializeField] [Range(1, 4)] int investigationRadius;

    [SerializeField] Vector3 minRoamDist;

    Vector3 playerDir;

    Color colorOriginal;

    //Get and Set originalColor
    public Color GetOriginalColor()
    {
        return colorOriginal;
    }
    public void SetOriginalColor(Color newColor)
    {
        colorOriginal = newColor;
    }
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetOriginalColor(model.material.color);
        GameManager.instance.updateSecondaryGameGoal(1);

        hackProgressSlider.maxValue = maxHackLimit;

        hackProgressSlider.value = hackProgress;

        hackProgressCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        playerDir = GameManager.instance.player.transform.position - transform.position;
        HackBarFacePlayer();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            GameManager.instance.GetInteractPopup().SetActive(true);

            if (Input.GetButton("Interact"))
            {

                hackProgress += Time.deltaTime;

                timer += Time.deltaTime;

                warningText.text = ($"Enemies will be warned in {(timeBetweenAlerts - timer):F2}!");
                
                if (hackProgressSlider != null && hackProgressCanvas != null)
                {
                    hackProgressCanvas.enabled = true;
                    hackProgressSlider.value = hackProgress;
                }

            }

            if (timer >= timeBetweenAlerts)
            {

                AlertEnemies();

                timer = 0;

            }

            if (hackProgress >= maxHackLimit)
            {

                GameManager.instance.updateSecondaryGameGoal(-1);
                Destroy(gameObject);

            }

            if (Input.GetButtonUp("Interact"))
            {
                if (hackProgressCanvas.enabled)
                { hackProgressCanvas.enabled = false; }
            }
            
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.GetInteractPopup().SetActive(false);

            if (hackProgressCanvas.enabled)
            {
                hackProgressCanvas.enabled = false;
            }
            timer = 0;
        }
    }

    private void OnDestroy()
    {
        GameManager.instance.GetInteractPopup().SetActive(false);

        if (GameManager.instance.GetObjectiveCount() > 0)
        {
            AlertEnemies();
        }
    }

    public void AlertEnemies()
    {
        /* We need to find all of the enemies in this object's range. Because this is an enemy, we want it to notify enemies in a much larger radius.
   To do this, we create a Collider array that is populated from an OverlapSphere.*/
        Collider[] hitObjects = Physics.OverlapSphere(transform.position, 15 * investigationRadius);

        // For each collider in the array we've generated:
        foreach (Collider collider in hitObjects)
        {
            if (collider.GetComponent<EnemySpawner>() != null)
            {
                collider.GetComponent<EnemySpawner>().StartSpawning = true;
            }
            // We check for an IHearSounds component.
            IHearSounds heardSomething = collider.GetComponent<IHearSounds>();

            /* If the component exists, we call its ReactToSound method, passing in a random location around the object that was destroyed.
            By adding a minimum roam distance, we limit the likelihood that all of the enemies try to path to the same location. */
            heardSomething?.React(((Random.insideUnitSphere * investigationRadius) + minRoamDist * 2) + transform.position);
        }
    }

    private void HackBarFacePlayer()
    {
        // We get a quaternion from the LookRotation of the playerDir.
        Quaternion rot = Quaternion.LookRotation(playerDir);

        // We then rotate the enemy AI using a lerp, which lerps from the model's current rotation to the rot, in deltaTime.
        hackProgressCanvas.transform.rotation = Quaternion.Lerp(hackProgressCanvas.transform.rotation, rot, faceTargetSpeed * 2 * Time.deltaTime);
    }
}
