using UnityEngine;

public class Door : MonoBehaviour
{
    private Animation doorAnimation; 
    private bool isDoorOpen = false; 
    private bool canInteract = true; 

    void Start()
    {
        
        doorAnimation = GetComponent<Animation>();

        
        if (doorAnimation == null)
        {
            Debug.LogError("No Animation component found on " + gameObject.name);
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
           
            doorAnimation.Play("open");
            isDoorOpen = true; 
            canInteract = false; 
        }
    }

    void OnTriggerExit(Collider obj)
    {
       
        if (obj.CompareTag("Player"))
        {
            
        }
    }
}
