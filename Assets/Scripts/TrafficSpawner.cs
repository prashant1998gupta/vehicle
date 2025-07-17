using System.Collections.Generic;
using UnityEngine;

public class TrafficSpawner : MonoBehaviour
{
    public GameObject[] carPrefabs;
    public Transform player;
    public float spawnDistance = 50f;
    public float spawnInterval = 1.5f;
    public float laneOffset = 2f;
    public int numberOfLanes = 4;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnTrafficCar();
            timer = 0f;
        }
    }

    void SpawnTrafficCar()
    {
        // Pick a random lane
        int lane = Random.Range(0, numberOfLanes);
        float xPos = (lane - 1) * laneOffset; // -1, 0, 1 for 3 lanes

        Vector3 spawnPos = new Vector3(xPos, 1, player.position.z + spawnDistance);

        GameObject car = Instantiate(carPrefabs[Random.Range(0, carPrefabs.Length)], spawnPos, Quaternion.identity);
        car.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
