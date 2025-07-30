using UnityEngine;

[RequireComponent(typeof(Camera))]
public class HR_CarCamera : MonoBehaviour
{
    public enum CameraMode { Top, TPS, FPS }
    public CameraMode cameraMode;

    [Header("Camera Settings")]
    [SerializeField] private float topFOV = 48f;
    [SerializeField] private float tpsFOV = 55f;
    [SerializeField] private float fpsFOV = 65f;

    [SerializeField] private float topHeight = 8.5f;
    [SerializeField] private float topDistance = 8f;
    [SerializeField] private float topRotation = 30f;

    [SerializeField] private float cameraShakeAmount = 0.00025f;
    [SerializeField] private bool enableCameraShake = false;

    [Header("References")]
    public Transform playerCar;

    private Camera cam;
    private GameObject audioListener;
    private Rigidbody playerRigid;

    private Vector3 targetPosition;
    private Vector3 pastFollowerPosition, pastTargetPosition;
    private float targetFOV;
    private float speed;
    private float currentT, oldT;
    private int cameraSwitchCount;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        // Remove existing AudioListener
        if (TryGetComponent(out AudioListener listener))
            Destroy(listener);

        // Add new AudioListener on a child object
        audioListener = new GameObject("Audio Listener");
        audioListener.transform.SetParent(transform, false);
        audioListener.AddComponent<AudioListener>();
    }

    private void LateUpdate()
    {
        if (!playerCar)
        {
            TryAssignPlayer();
            return;
        }

        if (playerRigid == null && playerCar.TryGetComponent(out Rigidbody rb))
            playerRigid = rb;

        // Smooth FOV transition
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * 3f);

        // Choose camera mode
        switch (cameraMode)
        {
            case CameraMode.Top:
                TopCamera();
                break;
        }

        if (enableCameraShake)
            targetPosition += Random.insideUnitSphere * speed * cameraShakeAmount;

        transform.position = targetPosition;

        // Audio Listener tracks car's X axis only
        audioListener.transform.position = new Vector3(playerCar.position.x, transform.position.y, transform.position.z);

        // Store for smooth lerping
        pastFollowerPosition = transform.position;
        pastTargetPosition = targetPosition;

        currentT = transform.position.z - oldT;
        oldT = transform.position.z;
    }

    private void FixedUpdate()
    {
        if (!playerRigid) return;

        // Updated to use linearVelocity instead of the obsolete velocity property
        speed = Mathf.Lerp(speed, playerRigid.linearVelocity.magnitude * 3.6f, Time.fixedDeltaTime * 1.5f);
    }

    public void ChangeCamera()
    {
        cameraSwitchCount = (cameraSwitchCount + 1) % 3;
        cameraMode = (CameraMode)cameraSwitchCount;
    }

    private void TryAssignPlayer()
    {
        // TODO: Add your player assignment logic here (e.g., find by tag)
        Debug.LogWarning("HR_CarCamera: Player car not assigned.");
    }

    private void TopCamera()
    {
        Quaternion targetRot = Quaternion.Euler(topRotation, 0f, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * 2f);

        Vector3 carPos = playerCar.position;
        targetPosition = new Vector3(0f, carPos.y, carPos.z) - targetRot * Vector3.forward * topDistance;
        targetPosition.y = topHeight;

        if (Time.timeSinceLevelLoad < 3f)
        {
            float smoothFactor = (speed / 2f) * Mathf.Clamp(Time.timeSinceLevelLoad - 1.5f, 0f, 10f);
            transform.position = SmoothApproach(pastFollowerPosition, pastTargetPosition, targetPosition, smoothFactor);
        }

        targetFOV = topFOV;
    }

    private Vector3 SmoothApproach(Vector3 pastPos, Vector3 pastTarget, Vector3 targetPos, float delta)
    {
        if (Time.timeScale == 0 || float.IsNaN(delta) || float.IsInfinity(delta) || delta == 0 ||
            pastPos == Vector3.zero || pastTarget == Vector3.zero || targetPos == Vector3.zero)
            return transform.position;

        float t = (Time.deltaTime * delta) + 0.00001f;
        Vector3 v = (targetPos - pastTarget) / t;
        Vector3 f = pastPos - pastTarget + v;
        Vector3 l = targetPos - v + f * Mathf.Exp(-t);

        return (l != Vector3.negativeInfinity && l != Vector3.positiveInfinity && l != Vector3.zero) ? l : transform.position;
    }
}
