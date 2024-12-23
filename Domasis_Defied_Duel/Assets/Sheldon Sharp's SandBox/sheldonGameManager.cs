using UnityEngine;
using UnityEngine.UI;
using TMPro;

//CODY JANDES CREATED THIS SCRIPT 
public class sheldonGameManager : MonoBehaviour
{
    //Singleton
    public static sheldonGameManager instance;

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

    //Control Health bar
    public Image playerHPBar;

    //Control Armor bar (new)
    public Image playerArmorBar;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //single instance of singleton 
        instance = this;
        timeScaleOriginal = Time.timeScale; //use getter and setter here
        player = GameObject.FindWithTag("Player"); //allows us to find player
        playerScript = player.GetComponent<PlayerController>(); //pull player controller after located
        SetPlayerSpawnPoint(GameObject.FindWithTag("Player Spawn Position")); //POTENTIAL REMOVE///////////////////////////////////////////////////////////////////////////////////
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
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
        isPaused = !isPaused; //toggle bool
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

    public void updateGameGoal(int amount)
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

    // Update the player armor UI bar (new method)
    public void updateArmorUI(int armorCurrent, int armorMax)
    {
        if (playerArmorBar != null)
        {
            playerArmorBar.fillAmount = (float)armorCurrent / armorMax;
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
