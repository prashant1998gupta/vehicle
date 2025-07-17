using System.Collections.Generic;
using UnityEngine;

public class TrafficSpawner : MonoBehaviour
{
    [Header("Car Settings")]
    public GameObject[] carPrefabs;
    public int poolSize = 20;
    public float carSpeedMin = 6f;
    public float carSpeedMax = 12f;

    [Header("Spawn Settings")]
    public Transform player;
    public float spawnDistance = 50f;
    public float despawnDistance = 30f;
    public float spawnInterval = 1f;
    public int numberOfLanes = 4;
    public float laneOffset = 2.5f;

    private float timer;
    private Queue<GameObject> carPool = new Queue<GameObject>();
    private List<GameObject> activeCars = new List<GameObject>();

    void Start()
    {
        // Initialize object pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject car = Instantiate(carPrefabs[Random.Range(0, carPrefabs.Length)]);
            car.SetActive(false);
            carPool.Enqueue(car);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnTrafficCar();
            timer = 0f;
        }

        // Despawn cars behind the player
        for (int i = activeCars.Count - 1; i >= 0; i--)
        {
            if (player.position.z - activeCars[i].transform.position.z > despawnDistance)
            {
                RecycleCar(activeCars[i]);
                activeCars.RemoveAt(i);
            }
        }
    }

    void SpawnTrafficCar()
    {
        if (carPool.Count == 0) return;

        int lane = Random.Range(0, numberOfLanes);
        float xPos = (lane - (numberOfLanes / 2)) * laneOffset;

        Vector3 spawnPos = new Vector3(xPos, 1f, player.position.z + spawnDistance);
        GameObject car = carPool.Dequeue();

        car.transform.position = spawnPos;
        car.transform.rotation = Quaternion.identity;
        car.SetActive(true);

        float speed = Random.Range(carSpeedMin, carSpeedMax);
        car.GetComponent<AICar>()?.SetSpeed(speed);

        activeCars.Add(car);
    }

    void RecycleCar(GameObject car)
    {
        car.SetActive(false);
        carPool.Enqueue(car);
    }
}
