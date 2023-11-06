using UnityEngine;

public class RandomPrefabSpawner : MonoBehaviour
{
    public GameObject[] prefabs; // Array to hold the prefabs you want to spawn
    public float cooldown = 1f; // Cooldown period between spawns in seconds
    public int maxSpawnCount = 10; // Maximum number of objects to spawn before stopping

    private float timer = 0f;
    private int spawnCount = 0;

    void Update()
    {
        // Check if the spawn count has reached the maximum limit
        if (spawnCount >= maxSpawnCount)
        {
            return; // Stop spawning if the limit is reached
        }

        timer -= Time.deltaTime;

        // Check if the cooldown period has passed
        if (timer <= 0f)
        {
            // Spawn a random prefab from the list at the spawner's position
            SpawnRandomPrefab(transform.position);

            // Reset the timer based on the cooldown period
            timer = cooldown;

            // Increment the spawn count
            spawnCount++;
        }
    }

    void SpawnRandomPrefab(Vector3 spawnPosition)
    {
        // Check if there are any prefabs to spawn
        if (prefabs.Length > 0)
        {
            // Choose a random index within the prefabs array
            int randomIndex = Random.Range(0, prefabs.Length);

            // Instantiate the randomly selected prefab at the specified position
            GameObject prefabInstance = Instantiate(prefabs[randomIndex], spawnPosition, Quaternion.identity);

            // Optional: You can parent the instantiated prefab to the spawner for organization
            prefabInstance.transform.parent = transform;
        }
        else
        {
            Debug.LogError("No prefabs assigned to the spawner!");
        }
    }
}
