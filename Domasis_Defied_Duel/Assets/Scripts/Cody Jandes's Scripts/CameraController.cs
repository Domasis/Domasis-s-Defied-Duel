using UnityEngine;

public class CameraController : MonoBehaviour
{
    //CODY JANDES CREATED THIS SCRIPT 

    //Look sensitivity
    [SerializeField] int sensitivity;

    //Lovking the vertical min and max
    [SerializeField] int lockVertMin, lockVertMax;

    //Look inversion
    [SerializeField] bool invertY;

    //remember where we are on camera x
    float rotateX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Do not show cursor
        Cursor.visible = false;

        //Locks when outside of game window
        Cursor.lockState = CursorLockMode.Locked;
        
    }

    // Update is called once per frame
    void Update()
    {
        //Get Update
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime; //mouse Y
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime; //mouse X

        if (invertY)
        {
            rotateX += mouseY;
        }
        else
        {
            rotateX -= mouseY;
        }
        //Clamp x rotation of camera 
        rotateX = Mathf.Clamp(rotateX, lockVertMin, lockVertMax);

        //Rotate the camera on X-axis
        transform.localRotation = Quaternion.Euler(rotateX, 0, 0);

        //Rotate player on y-Axis (this is where player turns)
        transform.parent.Rotate(Vector3.up * mouseX);
    }

    //PAUSED AT 
}
