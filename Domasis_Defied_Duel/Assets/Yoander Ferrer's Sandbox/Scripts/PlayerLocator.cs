using UnityEngine;

public class PlayerLocator : MonoBehaviour
{

    private static PlayerLocator instance;

    private GameObject player;

    private PlayerController playerController;

    int EnemyCount;

    public static PlayerLocator Instance { get => instance; set => instance = value; }
    public GameObject Player { get => player; set => player = value; }
    public PlayerController PlayerController { get => playerController; set => playerController = value; }
    public int EnemyCount1 { get => EnemyCount; set => EnemyCount = value; }

    // Awake is called prior to Start, which is called prior to Update.
    void Awake()
    {
        Instance = this;

        Player = GameObject.FindWithTag("Player");

        PlayerController = Player.GetComponent<PlayerController>();
    }

    public void UpdateEnemyCount(int count)
    {
        EnemyCount += count;
    }
}
