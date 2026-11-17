using UnityEngine;

// Shared physics impact/lifetime logic for kunai and fireball prefabs.
[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class KunaiProjectile : MonoBehaviour
{
    public const float Lifetime = 3f;
    [SerializeField] private DamageType damageType = DamageType.Physical;
    public DamageType DamageKind => damageType;
    public bool IsHoming => false;
    public event System.Action<Vector3> Impacted;

    private Rigidbody body;
    private SphereCollider hitCollider;
    private Vector3 direction;
    private float speed;
    private bool hasHit;
    public int Damage { get; private set; } = 1;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        hitCollider = GetComponent<SphereCollider>();
        hitCollider.isTrigger = true;
        direction = transform.forward;
        Destroy(gameObject, Lifetime);
    }

    public void Initialize(EnemyHealth enemy, float movementSpeed, int damage = 1)
    {
        Initialize(enemy != null ? enemy.transform.position : transform.position + transform.forward,
            movementSpeed, damage);
    }

    public void Initialize(Vector3 aimPoint, float movementSpeed, int damage = 1)
    {
        speed = movementSpeed;
        Damage = Mathf.Max(1, damage);
        Vector3 offset = aimPoint - body.position;
        if (offset.sqrMagnitude > 0.0001f) direction = offset.normalized;
    }

    private void FixedUpdate()
    {
        if (hasHit)
        {
            return;
        }

        float distance = speed * Time.fixedDeltaTime;
        // Sweep the trigger's volume as well, so fast kunai cannot skip an enemy
        // between physics steps. Both paths share the once-only hit guard.
        RaycastHit[] hits = Physics.SphereCastAll(body.position, hitCollider.radius,
            direction, distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        foreach (RaycastHit hit in hits)
        {
            if (TryHit(hit.collider))
            {
                return;
            }
        }

        body.MoveRotation(Quaternion.LookRotation(direction));
        body.MovePosition(body.position + direction * distance);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryHit(other);
    }

    private bool TryHit(Collider other)
    {
        if (hasHit)
        {
            return false;
        }

        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
        if (enemy == null || enemy.CurrentHealth <= 0)
        {
            return false;
        }

        hasHit = true;
        hitCollider.enabled = false;
        body.detectCollisions = false;
        Impacted?.Invoke(other.ClosestPoint(body.position));
        enemy.TakeDamage(Damage, damageType);
        Destroy(gameObject);
        return true;
    }
}
