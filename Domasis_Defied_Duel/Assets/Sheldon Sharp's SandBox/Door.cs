using UnityEngine;

public class Door : MonoBehaviour
{
    private Animation doorAnimation; 
    //private bool isDoorOpen = false; 
    private bool canInteract = true;

    [SerializeField] AudioClip doorSounds;

    void Start()
    {
        
        doorAnimation = GetComponent<Animation>();

        
        if (doorAnimation == null)
        {
           // Debug.LogError("No Animation component found on " + gameObject.name);
        }
        else
        {
           
            doorAnimation.Stop();
        }
    }

    void OnTriggerStay(Collider obj)
    {
        
        if (obj.CompareTag("Player") && Input.GetKeyDown(KeyCode.E) && canInteract)
        {
            GameManager.instance.GetInteractPopup().SetActive(true);
            doorAnimation.Play("open");
            GameManager.instance.playerScript.DoorOpen(doorSounds);
            //isDoorOpen = true; 
            canInteract = false; 
        }
    }

    void OnTriggerExit(Collider obj)
    {
        GameManager.instance.GetInteractPopup().SetActive(false);
        if (obj.CompareTag("Player"))
        {
            
        }
    }
}
