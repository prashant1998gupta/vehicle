using System.Collections.Generic;
using UnityEngine;

public class RoadSpawner : MonoBehaviour
{
    public GameObject roadTilePrefab;
    public int numberOfTiles = 3;
    public float tileLength = 100f;
    public Transform player;

    private List<GameObject> activeTiles = new List<GameObject>();
    private float spawnZ = 0f;

    void Start()
    {
        for (int i = 0; i < numberOfTiles; i++)
        {
            SpawnTile();
        }
    }

    void Update()
    {
        if (player.position.z - 35 > spawnZ - numberOfTiles * tileLength)
        {
            SpawnTile();
            DeleteTile();
        }
    }

    void SpawnTile()
    {
        GameObject tile = Instantiate(roadTilePrefab, new Vector3(0, 0, spawnZ), Quaternion.identity);
        activeTiles.Add(tile);
        spawnZ += tileLength;
    }

    void DeleteTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }
}
