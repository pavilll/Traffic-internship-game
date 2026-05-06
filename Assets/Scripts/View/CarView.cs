using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarView : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float maxLifetime = 30f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float braking = 8f;
    private bool canMove = true;
    private float speedFactor = 1f;
    private float currentSpeed;
    private Rigidbody cachedRigidbody;
    private Collider cachedCollider;

    private void Awake()
    {
        cachedRigidbody = GetComponent<Rigidbody>();
        cachedCollider = GetComponent<Collider>();

        if (cachedRigidbody == null)
        {
            Debug.LogError("CarView: Rigidbody component is missing.", this);
            enabled = false;
            return;
        }

        cachedRigidbody.useGravity = false;
        cachedRigidbody.isKinematic = true;
        cachedRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        cachedRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Start()
    {
        if (maxLifetime > 0f)
        {
            Destroy(gameObject, maxLifetime);
        }
    }

    public void SetMove(bool value)
    {
        canMove = value;
    }

    public void SetSpeedFactor(float value)
    {
        speedFactor = Mathf.Clamp01(value);
    }

    public void SetSpeed(float value)
    {
        speed = Mathf.Max(0.1f, value);
    }

    public float GetForwardExtent()
    {
        if (cachedCollider == null)
        {
            return 1f;
        }

        return GetProjectedExtent(cachedCollider.bounds, transform.forward);
    }

    public float GetSideExtent()
    {
        if (cachedCollider == null)
        {
            return 0.5f;
        }

        return GetProjectedExtent(cachedCollider.bounds, transform.right);
    }

    private void FixedUpdate()
    {
        if (cachedRigidbody == null)
        {
            return;
        }

        float targetSpeed = canMove ? speed * speedFactor : 0f;
        float changeRate = targetSpeed > currentSpeed ? acceleration : braking;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, changeRate * Time.fixedDeltaTime);

        if (currentSpeed <= 0.001f)
        {
            return;
        }

        Vector3 targetPosition = cachedRigidbody.position + transform.forward * currentSpeed * Time.fixedDeltaTime;
        cachedRigidbody.MovePosition(targetPosition);
    }

    private float GetProjectedExtent(Bounds bounds, Vector3 direction)
    {
        Vector3 normalizedDirection = direction.normalized;
        Vector3 extents = bounds.extents;

        return Mathf.Abs(normalizedDirection.x) * extents.x +
               Mathf.Abs(normalizedDirection.y) * extents.y +
               Mathf.Abs(normalizedDirection.z) * extents.z;
    }
}
