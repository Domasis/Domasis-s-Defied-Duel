using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // Editor exposed field that tracks the specific gameObject to spawn.
    [SerializeField] GameObject objectToSpawn;

    // Editor exposed field that tracks the number of enemies this spawner instance will spawn.
    [SerializeField] [Range(3, 10)] int numberToSpawn;

    // Editor exposed variable that tracks the delay between enemy spawns. Set to 0 for instant spawns.
    [SerializeField] [Range(0, 3)] float spawnDelay;

    // Stores all of the locations that enemies can spawn from.
    [SerializeField] Transform[] spawnPos;

    // Number of enemies spawned thus far.
    int spawnCount;

    // Boolean that tracks whether the spawner should start spawning enemies.
    bool startSpawning;

    // Bool that tracks if the spawner is actively spawning an enemy.
    bool isSpawning;

    public GameObject ObjectToSpawn { get => objectToSpawn; set => objectToSpawn = value; }
    public int NumberToSpawn { get => numberToSpawn; set => numberToSpawn = value; }
    public float SpawnDelay { get => spawnDelay; set => spawnDelay = value; }
    public Transform[] SpawnPos { get => spawnPos; set => spawnPos = value; }
    public int SpawnCount { get => spawnCount; set => spawnCount = value; }
    public bool StartSpawning { get => startSpawning; set => startSpawning = value; }
    public bool IsSpawning { get => isSpawning; set => isSpawning = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if (StartSpawning && spawnCount < numberToSpawn && !isSpawning)
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

    IEnumerator Spawn()
    {
        IsSpawning = true;

        yield return new WaitForSeconds(SpawnDelay);

        int spawnLoc = Random.Range(0, spawnPos.Length);

        Instantiate(ObjectToSpawn, SpawnPos[spawnLoc].position, SpawnPos[spawnLoc].rotation);

        SpawnCount++;

        isSpawning = false;
    }
}
