using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 5f, -10f);
    public float followSpeed = 10f;
    public float rotationSpeed = 5f;

    private Vector3 velocity = Vector3.zero;

    void FixedUpdate()
    {
        if (!target) return;

        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, 1f / followSpeed);

        Quaternion desiredRot = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, rotationSpeed * Time.fixedDeltaTime);
    }
}
