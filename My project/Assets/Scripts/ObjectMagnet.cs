using UnityEngine;

public class ObjectMagnet : MonoBehaviour
{
    public Transform rayOrigin;            
    public float rayDistance = 10f;
    public Transform attachPoint;
    public GameObject ropePrefab;
    public float pullSpeed = 5f;
    public LayerMask pickupLayer;

    private GameObject currentTarget;
    private GameObject currentRope;
    private bool isPulling;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && currentTarget == null)
        {
            TryPickup();
        }

        if (isPulling && currentTarget)
        {
            PullObject();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReleaseObject();
        }
    }

    void TryPickup()
    {
        if (Physics.Raycast(rayOrigin.position, Vector3.down, out RaycastHit hit, rayDistance, pickupLayer))
        {
            currentTarget = hit.collider.gameObject;

            currentRope = Instantiate(ropePrefab, attachPoint.position, Quaternion.identity);
            currentRope.transform.SetParent(attachPoint);
            UpdateRopeTransform();

            isPulling = true;
            Rigidbody rb = currentTarget.GetComponent<Rigidbody>();
            if (rb) rb.useGravity = false;
        }
    }

    void PullObject()
    {
        currentTarget.transform.position = Vector3.MoveTowards(
            currentTarget.transform.position,
            attachPoint.position,
            pullSpeed * Time.deltaTime
        );

        UpdateRopeTransform();

        float dist = Vector3.Distance(currentTarget.transform.position, attachPoint.position);
        if (dist < 0.2f)
        {
            currentTarget.transform.SetParent(attachPoint);
            isPulling = false;
        }
    }

    void ReleaseObject()
    {
        if (currentTarget)
        {
            currentTarget.transform.SetParent(null);
            Rigidbody rb = currentTarget.GetComponent<Rigidbody>();
            if (rb) rb.useGravity = true;

            currentTarget = null;
        }

        if (currentRope)
        {
            Destroy(currentRope);
            currentRope = null;
        }

        isPulling = false;
    }

    void UpdateRopeTransform()
    {
        if (!currentTarget || !currentRope) return;

        Vector3 dir = currentTarget.transform.position - attachPoint.position;
        float dist = dir.magnitude;

        currentRope.transform.position = attachPoint.position + dir / 2f;
        currentRope.transform.rotation = Quaternion.LookRotation(dir);
        currentRope.transform.localScale = new Vector3(0.1f, 0.1f, dist);
    }
}
