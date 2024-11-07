using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Singleton
    public static GameManager instance;

    //Close down whichever menu is open
    [SerializeField] GameObject menuActive;

    //Pause menu 
    [SerializeField] GameObject menuPause;

    //use getters and setters normally
    public bool isPaused;

    float timeScaleOriginal; //use getter and setter here

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //single instance of singleton 
        instance = this;
        timeScaleOriginal = Time.timeScale; //use getter and setter here
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();

                menuActive = menuPause;

                menuActive.SetActive(true);
            }

            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
        }
    }

    public void statePause()
    {
        isPaused=!isPaused; //toggle bool
        Time.timeScale = 0; //stops everything but game
        Cursor.visible = true; //see cursor when paused
        Cursor.lockState = CursorLockMode.Confined; //stuck in window with cursor
    }

    public void stateUnpause()
    {
        isPaused = !isPaused; //toggle bool
        Time.timeScale = timeScaleOriginal; //save variable to control this
        Cursor.visible = false; //don't see cursor when paused
        Cursor.lockState = CursorLockMode.Locked; //relock cursor

        menuActive.SetActive(false);
        menuActive = null; //unassign the active menu
    }
}
