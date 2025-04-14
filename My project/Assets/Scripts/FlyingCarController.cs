using UnityEngine;

public class FlyingCarController : MonoBehaviour
{
    [Header("Levitation Settings")]
    public float levitationHeight = 2f;
    public float levitationForce = 10f;
    public float hoverOscillationSpeed = 2f;
    public float hoverOscillationAmount = 0.2f;
    public LayerMask groundLayer;

    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float strafeSpeed = 7f;
    public float liftSpeed = 5f;

    [Header("Tilt Settings")]
    public float tiltAmount = 15f;
    public float tiltSpeed = 5f;

    private Rigidbody rb;
    private float hoverTimer;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        Hover();
        HandleLift();
        HandleMovement();
        HandleTilt();
    }

    void Hover()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, groundLayer))
        {
            float distanceToGround = hit.distance;
            float targetHeight = levitationHeight;

            float forceAmount = (targetHeight - distanceToGround) * levitationForce;

            hoverTimer += Time.fixedDeltaTime * hoverOscillationSpeed;
            float oscillation = Mathf.Sin(hoverTimer) * hoverOscillationAmount;

            rb.AddForce(Vector3.up * (forceAmount + oscillation), ForceMode.Acceleration);
        }
    }

    void HandleLift()
    {
        float verticalInput = 0f;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            verticalInput = 1f;
            if (animator) animator.SetBool("IsFlyingUp", true);
        }
        else
        {
            if (animator) animator.SetBool("IsFlyingUp", false);
        }

        if (Input.GetKey(KeyCode.LeftControl))
            verticalInput = -1f;

        rb.AddForce(Vector3.up * verticalInput * liftSpeed, ForceMode.Acceleration);
    }


    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 moveInput = new Vector3(h, 0, v);

        if (moveInput.magnitude > 0.1f)
        {
            Vector3 moveDirection = (transform.forward * v + transform.right * h).normalized;
            rb.AddForce(moveDirection * moveSpeed, ForceMode.Acceleration);
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * .5f);
        }
    }


    void HandleTilt()
    {
        float h = Input.GetAxis("Horizontal"); 

        float targetZRotation = -h * tiltAmount;
        Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, targetZRotation);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * tiltSpeed);
    }
}
