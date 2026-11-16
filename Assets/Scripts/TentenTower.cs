using UnityEngine;

// Shared ranged attack driver; prefab identity selects Tenten or Sasuke behavior.
public class TentenTower : MonoBehaviour
{
    public enum TowerKind { Tenten, Sasuke }
    [SerializeField] private TowerKind towerKind;
    public bool IsSasuke => towerKind == TowerKind.Sasuke;
    public bool SupportsUpgrade => !IsSasuke;
    public int DeploymentCost => IsSasuke ? 60 : 50;
    public int SellValue => Mathf.RoundToInt(DeploymentCost * 0.7f);
    public string DisplayName => IsSasuke ? "Genin Sasuke" : IsChunin ? "Chunin Tenten" : "Genin Tenten";
    public string AttackDescription => IsSasuke ? "Fire Style: Fireball Jutsu\n2 fire damage per hit" : $"Ranged physical damage ({DamagePerHit} per hit)";
    [SerializeField] private EnemyHealth target;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private KunaiProjectile kunaiPrefab;
    [SerializeField, Min(0.1f)] private float attackRange = 5.5f;
    [SerializeField, Min(0.05f)] private float timeBetweenShots = 0.9f;
    [SerializeField, Min(0.1f)] private float projectileSpeed = 10f;
    [SerializeField] private Renderer accentRenderer;
    [SerializeField] private Material chuninAccent;

    public bool IsChunin { get; private set; }
    public int DamagePerHit => IsSasuke || IsChunin ? 2 : 1;

    public void UpgradeToChunin()
    {
        if (IsSasuke || IsChunin) return;
        IsChunin = true;
        accentRenderer.sharedMaterial = chuninAccent;
    }

    private float nextShotTime;
    public float AttackRange => attackRange;

    public void SetTarget(EnemyHealth enemy)
    {
        target = enemy;
    }

    private void OnEnable()
    {
        nextShotTime = 0f;
    }

    private void Update()
    {
        if (target == null || target.CurrentHealth <= 0 ||
            Vector2.Distance(new Vector2(transform.position.x, transform.position.z),
                new Vector2(target.transform.position.x, target.transform.position.z)) > attackRange)
        {
            target = null;
            float closest = attackRange * attackRange;
            foreach (EnemyHealth candidate in FindObjectsByType<EnemyHealth>())
            {
                Vector3 offset = candidate.transform.position - transform.position;
                offset.y = 0f;
                if (candidate.CurrentHealth > 0 && offset.sqrMagnitude <= closest)
                {
                    closest = offset.sqrMagnitude;
                    target = candidate;
                }
            }
        }

        if (target == null || target.CurrentHealth <= 0 ||
            throwPoint == null || kunaiPrefab == null)
        {
            return;
        }

        Vector3 direction = target.transform.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude > attackRange * attackRange)
        {
            return;
        }

        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        if (Time.time < nextShotTime)
        {
            return;
        }

        Vector3 velocity = target.TryGetComponent<EnemyMovement>(out var movement)
            ? movement.Velocity : Vector3.zero;
        float speed = Mathf.Max(0.1f, projectileSpeed);
        Vector3 origin = throwPoint.position;
        if (!ProjectileAiming.TryGetInterceptPoint(origin, target.transform.position,
            velocity, speed, KunaiProjectile.Lifetime, out Vector3 intercept)) return;
        Vector3 launchDirection = intercept - origin;
        Quaternion launchRotation = launchDirection.sqrMagnitude > 0.0001f
            ? Quaternion.LookRotation(launchDirection) : transform.rotation;
        KunaiProjectile kunai = Instantiate(kunaiPrefab, origin, launchRotation);
        kunai.Initialize(intercept, speed, DamagePerHit);
        nextShotTime = Time.time + Mathf.Max(0.05f, timeBetweenShots);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
