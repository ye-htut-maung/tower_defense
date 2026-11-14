using UnityEngine;

public class TentenTower : MonoBehaviour
{
    [SerializeField] private EnemyHealth target;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private KunaiProjectile kunaiPrefab;
    [SerializeField, Min(0.1f)] private float attackRange = 5.5f;
    [SerializeField, Min(0.05f)] private float timeBetweenShots = 0.9f;
    [SerializeField, Min(0.1f)] private float projectileSpeed = 10f;
    [SerializeField] private Renderer accentRenderer;
    [SerializeField] private Material chuninAccent;

    public bool IsChunin { get; private set; }
    public int DamagePerHit => IsChunin ? 2 : 1;

    public void UpgradeToChunin()
    {
        if (IsChunin) return;
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

        KunaiProjectile kunai = Instantiate(kunaiPrefab, throwPoint.position, transform.rotation);
        kunai.Initialize(target, Mathf.Max(0.1f, projectileSpeed), DamagePerHit);
        nextShotTime = Time.time + Mathf.Max(0.05f, timeBetweenShots);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
