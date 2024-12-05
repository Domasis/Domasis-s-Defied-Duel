using UnityEngine;

public class Door : MonoBehaviour
{
    GameObject theDoor;
    private bool isPlayerNear = false; // Track if the player is near the door
    [SerializeField] private KeyCode interactionKey = KeyCode.E; // Key to interact with the door

    void Start()
    {
        theDoor = GameObject.FindWithTag("SF_Door");
    }

    void Update()
    {
        // Check if the player is near and presses the interaction key
        if (isPlayerNear && Input.GetKeyDown(interactionKey))
        {
            if (theDoor.GetComponent<Animation>().IsPlaying("open"))
            {
                theDoor.GetComponent<Animation>().Play("close");
            }
            else
            {
                theDoor.GetComponent<Animation>().Play("open");
            }
        }
    }

    void OnTriggerEnter(Collider obj)
    {
        if (obj.CompareTag("Player")) // Ensure only the player can interact
        {
            isPlayerNear = true; // Player is in range
        }
    }

    void OnTriggerExit(Collider obj)
    {
        if (obj.CompareTag("Player")) // Ensure only the player can interact
        {
            isPlayerNear = false; // Player is out of range
        }
    }
}
