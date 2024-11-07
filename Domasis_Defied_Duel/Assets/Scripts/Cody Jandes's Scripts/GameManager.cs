using UnityEngine;
using UnityEngine.UI;
using TMPro;

//CODY JANDES CREATED THIS SCRIPT 
public class GameManager : MonoBehaviour
{
    //Singleton
    public static GameManager instance;

    //Close down whichever menu is open
    [SerializeField] GameObject menuActive;

    //Pause menu , win/lose menu
    [SerializeField] GameObject menuPause, menuWin, menuLose;

    //Count for enemy 
    [SerializeField] TMP_Text enemyCountText;

    //Control Health bar
    public Image playerHPBar;

    //Take damage screen
    public GameObject playerDamageScreen;

    //Player call
    public GameObject player;

    //Player script
    public PlayerController playerScript;

    //use getters and setters normally
    public bool isPaused;

    float timeScaleOriginal; //use getter and setter here

    //Keep track of enemy count
    int enemyCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //single instance of singleton 
        instance = this;
        timeScaleOriginal = Time.timeScale; //use getter and setter here
        player = GameObject.FindWithTag("Player"); //allows us to find player
        playerScript = player.GetComponent<PlayerController>(); //pull player controller after located

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

        menuActive.SetActive(false); //deactivate menu
        menuActive = null; //unassign the active menu
    }

    public void updateGameGoal (int amount)
    {
        enemyCount += amount;
        enemyCountText.text = enemyCount.ToString("F0");

        if (enemyCount <= 0)
        {
            //Pause and pull win menu
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
}
