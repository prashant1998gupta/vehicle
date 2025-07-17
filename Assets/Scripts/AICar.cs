using UnityEngine;

public class AICar : MonoBehaviour
{
    private float speed = 10f;

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
