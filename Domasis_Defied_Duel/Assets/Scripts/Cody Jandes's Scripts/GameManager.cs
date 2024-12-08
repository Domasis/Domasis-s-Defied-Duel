using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager instance;

    // Close down whichever menu is open
    [SerializeField] GameObject menuActive;

    // Pause menu , win/lose menu
    [SerializeField] GameObject menuPause, menuWin, menuLose;

    // Count for enemy 
    [SerializeField] TMP_Text enemyCountText;

    // Count for objective
    [SerializeField] TMP_Text objectiveCountText;

    // Checkpoint popup
    [SerializeField] GameObject checkpointPopup;

    // Interactive object popup
    [SerializeField] GameObject interactPopup;

    // Alertable Enemies
    [SerializeField] GameObject[] alertableEnemies;

    // Track objective
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

    // Control Health bar
    public Image playerHPBar;

    // Take damage screen
    public GameObject playerDamageScreen;

    // Player call
    public GameObject player;

    // Player script
    public PlayerController playerScript;

    // Player spawn point (use getter and setter) POTENTIAL REMOVE/////////////////////////////////////////////////////////
    GameObject playerSpawnPosition;

    public GameObject GetPlayerSpawnPoint()
    {
        return playerSpawnPosition;
    }

    public void SetPlayerSpawnPoint(GameObject newPlayerSpawn)
    {
        playerSpawnPosition = newPlayerSpawn;
    }

    // Use getters and setters normally
    public bool isPaused;

    float timeScaleOriginal; // Use getter and setter here

    // Keep track of enemy count
    int enemyCount;

    // Moved ammo count to game manager so moved text with it
    [SerializeField] TMP_Text ammoCountText;

    [Header("Mission UI")]
    public GameObject missionPanel;

    public float TimeScaleOriginal { get => timeScaleOriginal; set => timeScaleOriginal = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // Single instance of singleton 
        instance = this;
        TimeScaleOriginal = Time.timeScale; // Use getter and setter here
        player = GameObject.FindWithTag("Player"); // Allows us to find player
        playerScript = player.GetComponent<PlayerController>(); // Pull player controller after located
        SetPlayerSpawnPoint(GameObject.FindWithTag("Player Spawn Position")); // POTENTIAL REMOVE///////////////////////////////////////////////////////////////////////////////////
    }

    private void InitilaizeLevel()
    {
        missionPanel.SetActive(true);
      
    }
    public void StartMission()
    {
        missionPanel.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("Menu"))
        {
            // To ensure that the game does not attempt to pause while a tooltip is open (as it is already paused), we check here to make sure that the tooltip isn't visible, as well as if our menu isn't active. - Yoander
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
        isPaused = !isPaused; // Toggle bool
        Time.timeScale = 0; // Stops everything but game
        Cursor.visible = true; // See cursor when paused
        Cursor.lockState = CursorLockMode.Confined; // Stuck in window with cursor
    }

    public void stateUnpause()
    {
        isPaused = !isPaused; // Toggle bool
        Time.timeScale = TimeScaleOriginal; // Save variable to control this
        Cursor.visible = false; // Don't see cursor when paused
        Cursor.lockState = CursorLockMode.Locked; // Relock cursor

        menuActive.SetActive(false); // Deactivate menu
        menuActive = null; // Unassign the active menu
    }

    public void updateGameGoal(int amount)
    {
        enemyCount += amount;
        enemyCountText.text = enemyCount.ToString("F0");

        //if (enemyCount <= 0)
        //{
        //    youWin();
        //}
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

    // Moved ammo counter to game manager
    public void updateAmmoCounttt(int ammoCurr)
    {
        if (ammoCountText != null)
        {
            ammoCountText.text = ammoCurr.ToString("F0");
        }
    }

    public void youLose()
    {
        GetInteractPopup().SetActive(false);
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void youWin()
    {
        GetInteractPopup().SetActive(false);
        // Pause and pull win menu
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }
}

