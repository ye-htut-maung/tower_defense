using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class KunaiProjectile : MonoBehaviour
{
    private const float Lifetime = 3f;

    private Rigidbody body;
    private SphereCollider hitCollider;
    private EnemyHealth target;
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
        target = enemy;
        speed = movementSpeed;
        Damage = Mathf.Max(1, damage);
    }

    private void FixedUpdate()
    {
        if (hasHit)
        {
            return;
        }

        // Track the moving capsule; if it disappears, continue until expiry.
        if (target != null && target.CurrentHealth > 0)
        {
            Vector3 offset = target.transform.position - body.position;
            if (offset.sqrMagnitude > 0.0001f)
            {
                direction = offset.normalized;
            }
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
        enemy.TakeDamage(Damage);
        Destroy(gameObject);
        return true;
    }
}
