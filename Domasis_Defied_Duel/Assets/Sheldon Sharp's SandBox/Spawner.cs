using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private int numberToSpawn;
    [SerializeField] private int timeBetweenSpawns;
    [SerializeField] private Transform[] spawnPos;

    private int spawnCount;
    private bool startSpawning;
    private bool isSpawning;

  
    public GameObject ObjectToSpawn
    {
        get => objectToSpawn;
        set => objectToSpawn = value;
    }

    public int NumberToSpawn
    {
        get => numberToSpawn;
        set => numberToSpawn = value;
    }

    public int TimeBetweenSpawns
    {
        get => timeBetweenSpawns;
        set => timeBetweenSpawns = value;
    }

    public Transform[] SpawnPositions
    {
        get => spawnPos;
        set => spawnPos = value;
    }

    public int SpawnCount
    {
        get => spawnCount;
        private set => spawnCount = value;
    }

    public bool StartSpawning
    {
        get => startSpawning;
        set => startSpawning = value;
    }

    public bool IsSpawning
    {
        get => isSpawning;
        private set => isSpawning = value;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.instance.updateGameGoal(NumberToSpawn);
    }


    void Update()
    {
        if (StartSpawning && SpawnCount < NumberToSpawn && !IsSpawning)
        {
            StartCoroutine(Spawn());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartSpawning = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartSpawning = false;
        }
    }

    IEnumerator Spawn()
    {
        IsSpawning = true;

        yield return new WaitForSeconds(TimeBetweenSpawns);

        int spawnInt = Random.Range(0, SpawnPositions.Length);

        Instantiate(ObjectToSpawn, SpawnPositions[spawnInt].position, SpawnPositions[spawnInt].rotation);

        SpawnCount++;

        IsSpawning = false;
    }
}
