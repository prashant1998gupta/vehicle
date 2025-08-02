using UnityEngine;
using System.Collections.Generic;

public class HR_RoadPooling : MonoBehaviour
{
    [System.Serializable]
    public class RoadPrefab
    {
        public GameObject roadObject;
    }

    public RoadPrefab[] roadPrefabs;
    public int poolSize = 20;
    public bool useAutomaticLength = true;
    public float manualRoadLength = 60f;
    public LayerMask asphaltLayer;

    private float roadLength;
    private List<GameObject> roadPool = new List<GameObject>();
    private Transform player;

    private float playerZLastFrame;

    private float forwardZ;
    private float backwardZ;

    private void Awake()
    {
        player = Camera.main.transform;
        roadLength = useAutomaticLength ? CalculateRoadLength(roadPrefabs[0].roadObject) : manualRoadLength;

        CreatePool();

        // Place initial roads centered around the player
        float startZ = player.position.z - (roadLength * poolSize / 2f);
        for (int i = 0; i < roadPool.Count; i++)
        {
            roadPool[i].transform.position = new Vector3(0f, 0f, startZ + i * roadLength);
            roadPool[i].SetActive(true);
        }

        playerZLastFrame = player.position.z;
    }

 /*   private void Update()
    {
        float playerZ = player.position.z;
        float direction = Mathf.Sign(playerZ - playerZLastFrame);

        for (int i = 0; i < roadPool.Count; i++)
        {
            GameObject road = roadPool[i];
            float distanceFromPlayer = Mathf.Abs(road.transform.position.z - playerZ);

            if (distanceFromPlayer > roadLength * poolSize / 2f)
            {
                // Move road in front or behind depending on direction
                float newZ = playerZ + direction * (roadLength * poolSize / 2f);
                road.transform.position = new Vector3(0f, 0f, newZ);
            }
        }

        playerZLastFrame = playerZ;
    }*/

    private void Update()
    {
        float playerZ = player.position.z;

        // Spawn forward if needed
        if (playerZ + (roadLength * (poolSize / 4f)) > forwardZ)
        {
            ReuseOldestRoad(forward: true);
        }

        // Spawn backward if needed
        if (playerZ - (roadLength * (poolSize / 4f)) < backwardZ)
        {
            ReuseOldestRoad(forward: false);
        }
    }

    private void ReuseOldestRoad(bool forward)
    {
        GameObject oldest = GetFarthestRoad(forward: !forward);
        float newZ = forward ? forwardZ : backwardZ - roadLength;

        oldest.transform.position = new Vector3(0f, 0f, newZ);

        if (forward)
            forwardZ += roadLength;
        else
            backwardZ -= roadLength;
    }



    private void CreatePool()
    {
        GameObject allRoads = new GameObject("All Roads");

        for (int i = 0; i < poolSize; i++)
        {
            GameObject prefab = roadPrefabs[Random.Range(0, roadPrefabs.Length)].roadObject;
            GameObject go = Instantiate(prefab);
            go.isStatic = false;
            HR_SetLightmapsManually.Instance.AlignLightmaps(prefab, go);
            go.transform.SetParent(allRoads.transform);
            roadPool.Add(go);
        }

        foreach (var r in roadPrefabs)
        {
            r.roadObject.SetActive(false); // deactivate template
        }

        // Initial road layout (centered around player)
        float playerZ = player.position.z;
        float half = poolSize / 2;

        forwardZ = playerZ;
        backwardZ = playerZ;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject road = roadPool[i];
            float z = playerZ + (i - half) * roadLength;
            road.transform.position = new Vector3(0f, 0f, z);
            road.SetActive(true);
        }

        forwardZ = playerZ + ((half) * roadLength);
        backwardZ = playerZ - ((half) * roadLength);

    }

    private GameObject GetFarthestRoad(bool forward)
    {
        GameObject selected = roadPool[0];
        foreach (var road in roadPool)
        {
            if (forward)
            {
                if (road.transform.position.z > selected.transform.position.z)
                    selected = road;
            }
            else
            {
                if (road.transform.position.z < selected.transform.position.z)
                    selected = road;
            }
        }
        return selected;
    }


    private float CalculateRoadLength(GameObject roadObj)
    {
        GameObject temp = Instantiate(roadObj, Vector3.zero, Quaternion.identity);
        Bounds bounds = temp.GetComponentInChildren<Renderer>().bounds;

        foreach (Renderer r in temp.GetComponentsInChildren<Renderer>())
        {
            if ((1 << r.gameObject.layer & asphaltLayer.value) != 0)
                bounds.Encapsulate(r.bounds);
        }

        float length = bounds.size.z;
        Destroy(temp);
        return length;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(Vector3.zero, new Vector3(14f, 1f, roadLength));
    }
}
