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
        player = GameObject.FindWithTag("Player").transform;
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

    #region it you want to use update than use this region
    //private void Update()
    //{
    //    float playerZ = player.position.z;
    //    float minZ = GetMinRoadZ();
    //    float maxZ = GetMaxRoadZ();

    //    // When close to the front, extend forward
    //    if (playerZ + roadLength * (poolSize / 4f) > maxZ)
    //    {
    //        ReuseOldestRoad(forward: true, newZ: maxZ + roadLength);
    //    }

    //    // When close to the back, extend backward
    //    if (playerZ - roadLength * (poolSize / 4f) < minZ)
    //    {
    //        ReuseOldestRoad(forward: false, newZ: minZ - roadLength);
    //    }
    //}
    #endregion

    #region it you want to use fixupdate than use this region
    private float checkThreshold = 30f;
    private float lastCheckedZ = float.MinValue;

    private void FixedUpdate()
    {
        float playerZ = player.position.z;

        // Only check if the player has moved enough in Z
        if (Mathf.Abs(playerZ - lastCheckedZ) >= checkThreshold)
        {
            TryExtendRoads(playerZ);
            lastCheckedZ = playerZ;
        }
    }

    private void TryExtendRoads(float playerZ)
    {
        float minZ = GetMinRoadZ();
        float maxZ = GetMaxRoadZ();

        //Debug.Log($"Player Z: {playerZ}, Min Z: {minZ}, Max Z: {maxZ}");

        if (playerZ + roadLength * (poolSize / 3f) > maxZ)
        {
            //Debug.Log($"Player Z: {playerZ}, playerZ + roadLength * (poolSize / 4f): {playerZ + roadLength * (poolSize / 5f)}");

            ReuseOldestRoad(forward: true, newZ: maxZ + roadLength);
        }

        if (playerZ - roadLength * (poolSize / 3f) < minZ)
        {
            //Debug.Log($"Player Z: {playerZ}, playerZ - roadLength * (poolSize / 4f): {playerZ - roadLength * (poolSize / 5f)}");

            ReuseOldestRoad(forward: false, newZ: minZ - roadLength);
        }
    }
    #endregion


    private float GetMinRoadZ()
    {
        float min = float.MaxValue;
        foreach (var r in roadPool)
            min = Mathf.Min(min, r.transform.position.z);
        return min;
    }

    private float GetMaxRoadZ()
    {
        float max = float.MinValue;
        foreach (var r in roadPool)
            max = Mathf.Max(max, r.transform.position.z);
        return max;
    }


    private void ReuseOldestRoad(bool forward, float newZ)
    {
        GameObject road = GetFarthestRoad(forward: !forward);
        road.transform.position = new Vector3(0f, 0f, newZ);
    }



    private void CreatePool()
    {
        GameObject allRoads = new GameObject("All Roads");

        for (int i = 0; i < poolSize; i++)
        {
            GameObject prefab = roadPrefabs[Random.Range(0, roadPrefabs.Length)].roadObject;
            GameObject go = Instantiate(prefab);
            go.isStatic = false;
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
