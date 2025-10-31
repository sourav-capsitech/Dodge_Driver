using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Required for List

public class SpawnManager : MonoBehaviour
{
    [Header("Prefab References")]
    public GameObject carPrefab;
    public GameObject coinPrefab;

    [Header("Spawn Settings")]
    public Transform spawnPoint; 
    
    // Lane positions based on a 10.8 unit wide road centered at X=0
    [Header("Lane Positions (Calculated from 10.8 width)")]
    // Left Player Lanes (Left Half: X=-5.4 to X=0)
    public float lane1_FarLeft = -4.05f;    // Outer lane of left player
    public float lane2_NearCenter = -1.35f; // Inner lane of left player
    // Right Player Lanes (Right Half: X=0 to X=5.4)
    public float lane3_NearCenter = 1.35f;  // Inner lane of right player
    public float lane4_FarRight = 4.05f;    // Outer lane of right player

    // List to hold all four lane X coordinates for random selection
    private List<float> allLanes; 

    [Header("Car Spawn Configuration (Obstacles)")]
    public float carInitialInterval = 1.5f;     // Starting time between car spawns
    public float carMaxInterval = 3.5f;         // Maximum time between car spawns (max 'rest' time)
    public float carIntervalIncreaseRate = 0.05f; // How much the delay increases each spawn

    [Header("Coin Spawn Configuration (Collectibles)")]
    public float coinInitialInterval = 3.0f;    // Starting time between coin spawns
    public float coinMaxInterval = 6.0f;        // Maximum time between coin spawns
    public float coinIntervalIncreaseRate = 0.1f; // How much the delay increases each spawn
    
    // Private variables to track current spawn rates
    private float currentCarInterval;
    private float currentCoinInterval;

    private void Start()
    {
        // Populate the lane list
        allLanes = new List<float> { lane1_FarLeft, lane2_NearCenter, lane3_NearCenter, lane4_FarRight };

        currentCarInterval = carInitialInterval;
        currentCoinInterval = coinInitialInterval;

        // Start separate coroutines for cars and coins
        StartCoroutine(CarSpawnRoutine());
        StartCoroutine(CoinSpawnRoutine());
    }

    /// <summary>
    /// Coroutine to handle continuous car (obstacle) spawning.
    /// The interval increases over time, providing easier stretches.
    /// </summary>
    private IEnumerator CarSpawnRoutine()
    {
        while (true)
        {
            // Wait for the current interval
            yield return new WaitForSeconds(currentCarInterval);

            // Spawn the car
            SpawnCar();

            // Gradually INCREASE the interval (easier sections), clamping at the maximum
            currentCarInterval = Mathf.Min(carMaxInterval, currentCarInterval + carIntervalIncreaseRate);
        }
    }

    /// <summary>
    /// Coroutine to handle continuous coin (collectible) spawning.
    /// The interval increases over time.
    /// </summary>
    private IEnumerator CoinSpawnRoutine()
    {
        while (true)
        {
            // Wait for the current interval
            yield return new WaitForSeconds(currentCoinInterval);

            // Spawn the coin
            SpawnCoin();
            
            // Gradually INCREASE the interval, clamping at the maximum
            currentCoinInterval = Mathf.Min(coinMaxInterval, currentCoinInterval + coinIntervalIncreaseRate);
        }
    }

    private void SpawnCar()
    {
        // 1. Get a random lane X position
        float randomLaneX = GetRandomLanePosition();
        
        // 2. Create the spawn position using the random X and the shared spawnPoint Y/Z
        Vector3 spawnPos = new Vector3(randomLaneX, spawnPoint.position.y, spawnPoint.position.z);

        // 3. Instantiate the car at the new randomized position
        GameObject car = Instantiate(carPrefab, spawnPos, spawnPoint.rotation);
        
        // Start monitoring the car for destruction
        StartCoroutine(DestroyObjectAfterZ(car, 25f));
    }

    private void SpawnCoin()
    {
        // 1. Get a random lane X position
        float randomLaneX = GetRandomLanePosition();
        
        // 2. Create the spawn position using the random X and the shared spawnPoint Y/Z
        Vector3 spawnPos = new Vector3(randomLaneX, spawnPoint.position.y, spawnPoint.position.z);

        // 3. Instantiate the coin at the new randomized position
        GameObject coin = Instantiate(coinPrefab, spawnPos, spawnPoint.rotation);
        
        // Start monitoring the coin for destruction
        StartCoroutine(DestroyObjectAfterZ(coin, 22f));
    }
    
    /// <summary>
    /// Selects a random X coordinate from the four available lanes.
    /// </summary>
    private float GetRandomLanePosition()
    {
        int randomIndex = Random.Range(0, allLanes.Count);
        return allLanes[randomIndex];
    }

    /// <summary>
    /// Generic coroutine to destroy an object when it moves past a certain Z position.
    /// </summary>
    /// <param name="obj">The GameObject to monitor.</param>
    /// <param name="zThreshold">The Z position past which the object should be destroyed.</param>
    private IEnumerator DestroyObjectAfterZ(GameObject obj, float zThreshold)
    {
        while (obj != null)
        {
            if (obj.transform.position.z > zThreshold)
            {
                Destroy(obj);
                yield break;
            }
            yield return null;
        }
    }
}
