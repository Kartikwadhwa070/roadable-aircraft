using UnityEngine;

public class FlyingCarCameraController : MonoBehaviour
{
    public Transform target; 
    public Vector3 offset = new Vector3(0, 5, -10);  
    public float followSpeed = 5f;    
    public float rotationSpeed = 3f;    
    public float zoomSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 6f;

    private float currentZoom = 10f;
    private float yaw = 0f;
    private float pitch = 10f;

    void Start()
    {
        currentZoom = -offset.z;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (!target) return;

        HandleRotation();
        HandleZoom();
        FollowTarget();
    }

    void HandleRotation()
    {
        yaw += Input.GetAxis("Mouse X") * rotationSpeed;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
        pitch = Mathf.Clamp(pitch, -20f, 80f);
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scroll * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
    }

    void FollowTarget()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = target.position - (rotation * Vector3.forward * currentZoom) + Vector3.up * offset.y;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * followSpeed);
        transform.LookAt(target.position + Vector3.up * 2f);
    }
}
