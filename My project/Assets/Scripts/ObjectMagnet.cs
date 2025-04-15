using UnityEngine;

public class ObjectMagnet : MonoBehaviour
{
    [Header("Setup")]
    public Transform rayOrigin;
    public float rayDistance = 10f;
    public LayerMask pickupLayer;

    [Header("Attachment")]
    public Transform attachPoint;
    public float pullSpeed = 5f;

    [Header("Rope Visual")]
    public GameObject ropePrefab;
    private GameObject ropeInstance;

    private GameObject targetObject;
    private Rigidbody targetRb;

    private bool isPulling = false;
    private bool isAttached = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (!targetObject)
                TryPickup();
        }

        if (isPulling)
        {
            PullTarget();
            UpdateRope();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReleaseTarget();
        }
    }

    void TryPickup()
    {
        if (Physics.Raycast(rayOrigin.position, Vector3.down, out RaycastHit hit, rayDistance, pickupLayer))
        {
            targetObject = hit.collider.gameObject;
            targetRb = targetObject.GetComponent<Rigidbody>();

            if (!targetRb) return;

            targetRb.useGravity = false;
            targetRb.linearVelocity = Vector3.zero;

            CreateRope();
            isPulling = true;
        }
    }

    void PullTarget()
    {
        if (!targetObject) return;

        Vector3 direction = attachPoint.position - targetObject.transform.position;
        float distance = direction.magnitude;

        if (distance > 0.1f)
        {
            targetObject.transform.position = Vector3.MoveTowards(
                targetObject.transform.position,
                attachPoint.position,
                pullSpeed * Time.deltaTime
            );
        }
        else
        {
            AttachTarget();
        }
    }

    void AttachTarget()
    {
        if (!targetObject) return;

        targetObject.transform.SetParent(attachPoint);
        targetObject.transform.localPosition = Vector3.zero;
        isPulling = false;
        isAttached = true;

        if (targetRb) targetRb.isKinematic = true;
    }

    void ReleaseTarget()
    {
        if (!targetObject) return;

        targetObject.transform.SetParent(null);

        if (targetRb)
        {
            targetRb.useGravity = true;
            targetRb.isKinematic = false;
        }

        DestroyRope();

        targetObject = null;
        targetRb = null;
        isPulling = false;
        isAttached = false;
    }

    void CreateRope()
    {
        if (ropePrefab && !ropeInstance)
        {
            ropeInstance = Instantiate(ropePrefab, attachPoint.position, Quaternion.identity);
            ropeInstance.transform.SetParent(null); // world space
        }
    }

    void UpdateRope()
    {
        if (!ropeInstance || !targetObject) return;

        Vector3 start = attachPoint.position;
        Vector3 end = targetObject.transform.position;
        Vector3 midPoint = (start + end) / 2f;
        Vector3 direction = end - start;
        float length = direction.magnitude;

        ropeInstance.transform.position = midPoint;
        ropeInstance.transform.rotation = Quaternion.LookRotation(direction);
        ropeInstance.transform.localScale = new Vector3(0.1f, 0.1f, length);
    }

    void DestroyRope()
    {
        if (ropeInstance)
        {
            Destroy(ropeInstance);
            ropeInstance = null;
        }
    }
}
