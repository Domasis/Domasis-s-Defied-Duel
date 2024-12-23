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

    // Player Armor bar (new)
    public Image playerArmorBar;  // Armor bar UI element

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
    private bool isMissionPanelActive = false;

    public float TimeScaleOriginal { get => timeScaleOriginal; set => timeScaleOriginal = value; }

    [SerializeField] private AudioSource winMusicSource;
    [SerializeField] private AudioClip winMusicClip;
    [SerializeField] private AudioSource backgroundMusicSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // Ensure singleton instance
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Save the original time scale
        TimeScaleOriginal = Time.timeScale;

        // Find/initialize the player
        player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerScript = player.GetComponent<PlayerController>();
            playerScript.enabled = false; // Disable player controls at first
        }

        // Set the player spawn
        var spawnPoint = GameObject.FindWithTag("Player Spawn Position");
        if (spawnPoint != null)
        {
            SetPlayerSpawnPoint(spawnPoint);
        }

        // Initialize level
        InitializeLevel();

        if (winMusicClip != null)
        {
            winMusicSource.clip = winMusicClip;
        }
    }

    private void InitializeLevel()
    {
        missionPanel.SetActive(true);
        isMissionPanelActive = true;

        LockPlayerControls(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            if (Input.GetButtonDown("Cancel"))
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

        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (Input.GetKeyDown(KeyCode.Escape)||Input.GetButtonDown("Cancel"))
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

        if (isMissionPanelActive && Input.GetKeyDown(KeyCode.Return))
        {
            StartMission();
        }

        // Update the Armor bar
        UpdateArmorBar();
    }

    public void StartMission()
    {
        missionPanel.SetActive(false);
        isMissionPanelActive = false;

        LockPlayerControls(false);
    }

    public void LockPlayerControls(bool isLocked)
    {
        Cursor.visible = isLocked;
        Cursor.lockState = isLocked ? CursorLockMode.None : CursorLockMode.Locked;

        if (instance.player != null)
        {
            instance.player.GetComponent<PlayerController>().enabled = !isLocked;
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
        if (backgroundMusicSource != null && backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Stop();
        }
        // Pause and pull win menu
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);

        PlayWinMusic();
    }

    private void PlayWinMusic()
    {
        if (winMusicSource != null && winMusicClip != null)
        {
            winMusicSource.Stop();
            winMusicSource.Play();
        }
    }

    // Update the armor bar UI based on the player's armor percentage
    private void UpdateArmorBar()
    {
        if (playerScript != null)
        {
            float armorPercentage = playerScript.GetArmorPercentage(); // Get the armor percentage from the PlayerController
            playerArmorBar.fillAmount = armorPercentage;  // Update armor bar
        }
    }
}
