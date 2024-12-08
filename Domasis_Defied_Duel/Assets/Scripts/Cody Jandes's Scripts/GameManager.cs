using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

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

    //Count for objective
    [SerializeField] TMP_Text objectiveCountText;

    //Checkpoint popup
    [SerializeField] GameObject checkpointPopup;

    //Interactive object popup
    [SerializeField] GameObject interactPopup;

    //Alertable Enemies
    [SerializeField] GameObject[] alertableEnemies;

    //Track objective
    int objectiveCount; 

    public int GetObjectiveCount()
    {
        return objectiveCount;
    }
    public void SetObjectiveCount(int newCount)
    {
        objectiveCount += newCount;
    }
    

    public GameObject GetCheckpointPopup()
    {
        return checkpointPopup;
    }

    public void SetCheckpointPopup(GameObject thisCheckpointPopup)
    {
        checkpointPopup = thisCheckpointPopup;
    }

    public GameObject GetInteractPopup()
    {
        return interactPopup;
    }

    public void SetInteractPopup(GameObject thisInteractPopup)
    {
        interactPopup = thisInteractPopup;
    }

    //Control Health bar
    public Image playerHPBar;

    //Take damage screen
    public GameObject playerDamageScreen;

    //Player call
    public GameObject player;

    //Player script
    public PlayerController playerScript;

    //Player spawn point (use getter and setter) POTENTIAL REMOVE/////////////////////////////////////////////////////////
    GameObject playerSpawnPosition;

    public GameObject GetPlayerSpawnPoint()
    {
        return playerSpawnPosition;
    }

    public void SetPlayerSpawnPoint(GameObject newPlayerSpawn)
    {
        playerSpawnPosition = newPlayerSpawn;
    }

    //use getters and setters normally
    public bool isPaused;

    float timeScaleOriginal; //use getter and setter here

    //Keep track of enemy count
    int enemyCount;

    //Moved ammo count to game manager so moved text with it
    [SerializeField] TMP_Text ammoCountText;

    public float TimeScaleOriginal { get => timeScaleOriginal; set => timeScaleOriginal = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //single instance of singleton 
        instance = this;
        TimeScaleOriginal = Time.timeScale; //use getter and setter here
        player = GameObject.FindWithTag("Player"); //allows us to find player
        playerScript = player.GetComponent<PlayerController>(); //pull player controller after located
        SetPlayerSpawnPoint(GameObject.FindWithTag("Player Spawn Position")); //POTENTIAL REMOVE///////////////////////////////////////////////////////////////////////////////////

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel") || Input.GetButtonDown("Menu"))
        {
            // To ensure that the game does not attempt to pause while a tooltip is open (as it is already paused), we check here to make sure that the tooltip isn't visible, as well as if our menu isn't active. - Yoander
            if (menuActive == null && InteractiveTooltipManager.instance.TipCanvas.enabled == false)
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
        Time.timeScale = TimeScaleOriginal; //save variable to control this
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
            youWin();
        }
    }

    public void updateSecondaryGameGoal(int amount)
    {
        objectiveCount += amount;
        objectiveCountText.text = objectiveCount.ToString("F0");

        if (objectiveCount <= 0)
        {
            youWin();
        }
    }

    //Moved ammo counter to game manager
    public void updateAmmoCounttt(int ammoCurr)
    {
        if (ammoCountText != null)
        {
            ammoCountText.text = ammoCurr.ToString("F0");
        }
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void youWin()
    {
        //Pause and pull win menu
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }
}
