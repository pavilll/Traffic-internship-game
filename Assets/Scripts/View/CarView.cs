using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarView : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float maxLifetime = 0f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float braking = 8f;
    private bool canMove = true;
    private float speedFactor = 1f;
    private float currentSpeed;
    private Rigidbody cachedRigidbody;
    private Collider[] cachedColliders;

    private void Awake()
    {
        cachedRigidbody = GetComponent<Rigidbody>();
        cachedColliders = GetComponentsInChildren<Collider>();

        if (cachedRigidbody == null)
        {
            Debug.LogError("CarView: Rigidbody component is missing.", this);
            enabled = false;
            return;
        }

        cachedRigidbody.useGravity = true;
        cachedRigidbody.isKinematic = false;
        cachedRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        cachedRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        cachedRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        cachedRigidbody.linearDamping = 0.5f;
        cachedRigidbody.angularDamping = 5f;
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
        Bounds bounds;
        if (!TryGetCombinedColliderBounds(out bounds))
        {
            return 1f;
        }

        return GetProjectedExtent(bounds, transform.forward);
    }

    public float GetSideExtent()
    {
        Bounds bounds;
        if (!TryGetCombinedColliderBounds(out bounds))
        {
            return 0.5f;
        }

        return GetProjectedExtent(bounds, transform.right);
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
            Vector3 stopVelocity = cachedRigidbody.linearVelocity;
            stopVelocity.x = 0f;
            stopVelocity.z = 0f;
            cachedRigidbody.linearVelocity = stopVelocity;
            return;
        }

        Vector3 velocity = cachedRigidbody.linearVelocity;
        Vector3 forwardVelocity = transform.forward * currentSpeed;
        velocity.x = forwardVelocity.x;
        velocity.z = forwardVelocity.z;
        cachedRigidbody.linearVelocity = velocity;
    }

    private float GetProjectedExtent(Bounds bounds, Vector3 direction)
    {
        Vector3 normalizedDirection = direction.normalized;
        Vector3 extents = bounds.extents;

        return Mathf.Abs(normalizedDirection.x) * extents.x +
               Mathf.Abs(normalizedDirection.y) * extents.y +
               Mathf.Abs(normalizedDirection.z) * extents.z;
    }

    private bool TryGetCombinedColliderBounds(out Bounds bounds)
    {
        if (cachedColliders == null || cachedColliders.Length == 0)
        {
            bounds = default;
            return false;
        }

        bool foundCollider = false;
        bounds = default;

        for (int i = 0; i < cachedColliders.Length; i++)
        {
            Collider current = cachedColliders[i];
            if (current == null || !current.enabled || current.isTrigger)
            {
                continue;
            }

            if (!foundCollider)
            {
                bounds = current.bounds;
                foundCollider = true;
            }
            else
            {
                bounds.Encapsulate(current.bounds);
            }
        }

        return foundCollider;
    }
}
