using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AICar : MonoBehaviour
{
    public float speed = 10f;
    public float detectionDistance = 10f;
    public float laneChangeCooldown = 2f;

    private Rigidbody rb;
    private float lastLaneChangeTime = -999f;
    private int currentLane = 1; // default lane index (0 to 3)

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    //void Start()
    //{
    //    // Find closest lane on spawn
    //    float minDist = Mathf.Infinity;
    //    for (int i = 0; i < LaneManager.lanePositions.Length; i++)
    //    {
    //        float dist = Mathf.Abs(transform.position.x - LaneManager.lanePositions[i]);
    //        if (dist < minDist)
    //        {
    //            minDist = dist;
    //            currentLane = i;
    //        }
    //    }
    //}


    void OnEnable()
    {
        // Reset lane info when reactivated (from pool)
        float minDist = Mathf.Infinity;
        for (int i = 0; i < LaneManager.lanePositions.Length; i++)
        {
            float dist = Mathf.Abs(transform.position.x - LaneManager.lanePositions[i]);
            if (dist < minDist)
            {
                minDist = dist;
                currentLane = i;
            }
        }

        // Optional: Reset last lane change time
        lastLaneChangeTime = Time.time;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void FixedUpdate()
    {
        if (DetectObstacleAhead())
        {
            TryLaneChange();
        }

        // Move forward
        Vector3 move = transform.forward * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }

    bool DetectObstacleAhead()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hit, detectionDistance))
        {
            if (hit.collider.CompareTag("AICar"))
            {
                return true;
            }
        }
        return false;
    }

    void TryLaneChange()
    {
        if (Time.time - lastLaneChangeTime < laneChangeCooldown) return;

        int[] laneOrder = { -1, 1 }; // Try left, then right
        foreach (int offset in laneOrder)
        {
            int targetLane = currentLane + offset;
            if (targetLane < 0 || targetLane >= LaneManager.lanePositions.Length) continue;

            float targetX = LaneManager.lanePositions[targetLane];

            // Check if lane is clear
            Collider[] hits = Physics.OverlapBox(new Vector3(targetX, transform.position.y, transform.position.z + 3), new Vector3(1f, 1f, 4f));
            bool isClear = true;
            foreach (var col in hits)
            {
                if (col.CompareTag("AICar")) { isClear = false; break; }
            }

            if (isClear)
            {
                // Change lane
                Vector3 newPos = new Vector3(targetX, transform.position.y, transform.position.z);
                transform.position = Vector3.Lerp(transform.position, newPos, 0.5f); // Quick lane snap
                currentLane = targetLane;
                lastLaneChangeTime = Time.time;
                break;
            }
        }
    }
}
