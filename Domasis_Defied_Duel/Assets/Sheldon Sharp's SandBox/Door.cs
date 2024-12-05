using UnityEngine;

public class Door : MonoBehaviour
{
    GameObject theDoor;
    private bool isDoorOpen = false; 
    private bool canInteract = true; 

    private Animation doorAnimation;

    void Start()
    {
       
        theDoor = GameObject.FindWithTag("SF_Door");
        doorAnimation = theDoor.GetComponent<Animation>();

        doorAnimation.Stop();
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
