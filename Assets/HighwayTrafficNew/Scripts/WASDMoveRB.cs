using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WASDMoveRB : MonoBehaviour
{
    public float speed = 5f;
    public bool cameraRelative = false;

    Rigidbody rb;

    void Awake() { rb = GetComponent<Rigidbody>(); rb.freezeRotation = true; }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");   // A/D or Left/Right
        float v = Input.GetAxisRaw("Vertical");     // W/S or Up/Down

        Vector3 input = new Vector3(h, 0f, v).normalized;

        Vector3 dir = input;
        if (cameraRelative && Camera.main)
        {
            Vector3 fwd = Camera.main.transform.forward; fwd.y = 0f; fwd.Normalize();
            Vector3 right = Camera.main.transform.right; right.y = 0f; right.Normalize();
            dir = (fwd * v + right * h).normalized;
        }

        Vector3 targetVel = dir * speed;
        Vector3 velChange = targetVel - rb.linearVelocity;
        velChange.y = 0f; // keep gravity intact
        rb.AddForce(velChange, ForceMode.VelocityChange);
    }
}
