using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject[] clientPrefabs; // Array to hold multiple client prefabs
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval;
    [SerializeField] private int maxClients = 5;
    [SerializeField] private Transform[] targetPoint;
    [SerializeField] private AudioSource audioSource; // Reference to the AudioSource component
    [SerializeField] private AudioClip spawnSound; // Sound to play when a client is spawned
    public int currentClients = 0;
    public List<Transform> avaliableSpawnPoints;
    void Start()
    {
        avaliableSpawnPoints = new List<Transform>(targetPoint);
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnClient();
            spawnInterval = Random.Range(5f, 10f);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnClient()
    {
        if (currentClients < maxClients)
        {
            if (avaliableSpawnPoints.Count == 0)
                return;

            // Randomly select a client prefab
            int prefabIndex = Random.Range(0, clientPrefabs.Length);
            GameObject client = Instantiate(clientPrefabs[prefabIndex], spawnPoint.position, Quaternion.identity);

            client.GetComponent<ClientMovement>().SetSpawnPoint(spawnPoint);
            client.GetComponent<ClientMovement>().SetSpawner(this);

            int randomIndex = Random.Range(0, avaliableSpawnPoints.Count);
            Transform chosenTarget = avaliableSpawnPoints[randomIndex];

            client.GetComponent<ClientMovement>().SetTarget(chosenTarget);
            avaliableSpawnPoints.RemoveAt(randomIndex);

            currentClients++;

            // Play the spawn sound
            if (audioSource != null && spawnSound != null)
            {
                audioSource.PlayOneShot(spawnSound);
            }
        }
    }

    public void FreeSpawnPoint(Transform spawnPoint)
    {
        avaliableSpawnPoints.Add(spawnPoint);
        currentClients--;
    }
}
