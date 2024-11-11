using UnityEngine;

public class HeyCamMovePls : MonoBehaviour
{
    // Editor exposed variable that controls the camera's sensitivity.
    [SerializeField] int sens;

    // Editor exposed variables that list the minimum and maximum ranges for our vertical camera rotation.
    [SerializeField] float lockVertMin, lockVertMax;

    // Editor exposed variable that tracks whether our Y axis should be inverted.
    [SerializeField] bool invertY;

    [SerializeField] AudioListener listener;

    // Variable that stores the amount that our camera should be rotated by on the x-axis.
    float rotX;

    float rotY;


    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

        // Get the input
        float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;

        // If our Y axis is inverted:
        if (invertY)
        {
            // we add the value of MouseY to rotX.
            rotX += mouseY;

        }
        else
        {
            // Otherwise, we subtract the value of mouseY to rotX.
            rotX -= mouseY;

        }

        rotY += mouseX;

        // We then clamp the value of our camera's x rotation to our lockVertMin and lockVertMax, to ensure the camera does not rotate too far.
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        // Rotate the camera on the x-axis. We use localRotation, as normal Rotation is based in world space, and we need to rotate in local space.
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        // Rotate the player on the y-axis. We use mouseX because we want the character to rotate based on the mouse's x position.
        transform.parent.Rotate(Vector3.up * mouseX);
    }



}
