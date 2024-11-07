using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//Sheldon Sharp

public class GameManager1 : MonoBehaviour
{
    public static GameManager1 instance; // Singleton Class

    [SerializeField] GameObject menuActive, menuPause, menuWin, menuLose;
    [SerializeField] TMP_Text enemyCountText;
    public GameObject player;
    public GameObject playerDamageScreen;
    public PlayerController playerScript;
    public Image playerHPBar;

    private bool isPaused;
    private float timeScaleOrig;
    private int enemyCount; // Keeps track of  enemy count

    public bool IsPaused
    {
        get { return isPaused; }
        set { isPaused = value; }
    }

    public float TimeScaleOrig
    {
        get { return timeScaleOrig; }
        set { timeScaleOrig = value; }
    }

    public int EnemyCount
    {
        get { return enemyCount; }
        set
        {
            enemyCount = value;
            enemyCountText.text = enemyCount.ToString("F0");
        }
    }

    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");

        playerScript = player.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                StatePause();
                menuActive = menuPause;
                menuActive.SetActive(IsPaused);
            }
            else if (menuActive == menuPause)
            {
                StateUnpause();
            }
        }
    }

    public void StatePause()
    {
        IsPaused = !IsPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void StateUnpause()
    {
        IsPaused = !IsPaused;
        Time.timeScale = TimeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    // Method to update the enemy count when enemies are defeated or spawned
    public void UpdateGameGoal(int amount)
    {
        EnemyCount += amount;
        Debug.Log("Current Enemy Count: " + EnemyCount); // Log the current enemy count for debugging

        if (EnemyCount <= 0)
        {
            StatePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }

    public void YouLose()
    {
        StatePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
}
