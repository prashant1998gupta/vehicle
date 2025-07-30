using UnityEngine;

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
    private float targetFOV = 50f;
    private float speed = 0f;
    private float currentT, oldT;
    private int cameraSwitchCount = 0;

    private void Start()
    {
        cam = GetComponent<Camera>();
        transform.position = new Vector3(2f, 1f, 55f);
        transform.rotation = Quaternion.Euler(0f, -40f, 0f);

        // Remove default audio listener
        if (GetComponent<AudioListener>())
            Destroy(GetComponent<AudioListener>());

        // Add external audio listener
        audioListener = new GameObject("Audio Listener");
        audioListener.transform.SetParent(transform, false);
        audioListener.AddComponent<AudioListener>();
    }

    private void Update()
    {
       
    }

    private void LateUpdate()
    {
        if (!playerCar)
        {
            TryAssignPlayer();
            return;
        }

        if (!cam)
            cam = GetComponent<Camera>();

        if (playerRigid != playerCar.GetComponent<Rigidbody>())
            playerRigid = playerCar.GetComponent<Rigidbody>();

        // Smooth FOV
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * 3f);

        switch (cameraMode)
        {
            case CameraMode.Top:
                TopCamera();
                break;
        }

        if (enableCameraShake)
            targetPosition += Random.insideUnitSphere * speed * cameraShakeAmount;

        transform.position = targetPosition;

        // Audio listener follows only X from car
        audioListener.transform.position = new Vector3(playerCar.position.x, transform.position.y, transform.position.z);

        // Store for smooth lerp
        pastFollowerPosition = transform.position;
        pastTargetPosition = targetPosition;

        currentT = (transform.position.z - oldT);
        oldT = transform.position.z;
    }

    private void FixedUpdate()
    {
        if (!playerRigid)
            return;

        speed = Mathf.Lerp(speed, playerRigid.linearVelocity.magnitude * 3.6f, Time.deltaTime * 1.5f);
    }

    public void ChangeCamera()
    {
        cameraSwitchCount++;
        if (cameraSwitchCount >= 3)
            cameraSwitchCount = 0;

        cameraMode = (CameraMode)cameraSwitchCount;
    }

    private void SwitchToNextCamera()
    {
        cameraSwitchCount++;
        ChangeCamera();
    }

    private void TryAssignPlayer()
    {
      
    }

    private void TopCamera()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(topRotation, 0f, 0f), Time.deltaTime * 2f);

        targetPosition = new Vector3(0f, playerCar.position.y, playerCar.position.z);
        targetPosition -= transform.rotation * Vector3.forward * topDistance;
        targetPosition = new Vector3(targetPosition.x, topHeight, targetPosition.z);

        if (Time.timeSinceLevelLoad < 3f)
            transform.position = SmoothApproach(pastFollowerPosition, pastTargetPosition, targetPosition, (speed / 2f) * Mathf.Clamp(Time.timeSinceLevelLoad - 1.5f, 0f, 10f));

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

#if UNITY_2017_1_OR_NEWER
        if (l != Vector3.negativeInfinity && l != Vector3.positiveInfinity && l != Vector3.zero)
            return l;
        else
            return transform.position;
#else
        return l;
#endif
    }
}
