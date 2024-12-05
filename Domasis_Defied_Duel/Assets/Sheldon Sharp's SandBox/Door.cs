using UnityEngine;

public class Door : MonoBehaviour
{
    GameObject theDoor;

    void Start()
    {
        theDoor = GameObject.FindWithTag("SF_Door");
    }

    void OnTriggerStay(Collider obj)
    {
        // Check if the player is in the trigger and presses the "E" key
        if (obj.CompareTag("Player") && Input.GetKeyDown(KeyCode.E)) // Use the "E" key for interaction
        {
            // Ensure we check if the door animation is playing and toggle between open and close
            var doorAnimation = theDoor.GetComponent<Animation>();

            // Debug: Print out the current animation playing
            Debug.Log("Current Animation: " + doorAnimation.clip.name);

            // If the "open" animation is already playing, play "close", otherwise play "open"
            if (doorAnimation.IsPlaying("open"))
            {
                Debug.Log("Switching to Close animation");
                doorAnimation.CrossFade("close", 0.2f); // Smoothly transition to "close"
            }
            else
            {
                Debug.Log("Switching to Open animation");
                doorAnimation.CrossFade("open", 0.2f); // Smoothly transition to "open"
            }
        }
    }
}
