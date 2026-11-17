using UnityEngine;

// Presentation only; the projectile owns hit detection and damage.
[RequireComponent(typeof(KunaiProjectile))]
public class FireballVisuals : MonoBehaviour
{
    [SerializeField] private ParticleSystem impactBurst;
    private KunaiProjectile projectile;

    private void OnEnable()
    {
        projectile = GetComponent<KunaiProjectile>();
        projectile.Impacted += ShowImpact;
    }

    private void OnDisable()
    {
        if (projectile != null) projectile.Impacted -= ShowImpact;
    }

    private void ShowImpact(Vector3 position)
    {
        if (impactBurst != null)
            Instantiate(impactBurst, position, Quaternion.identity);
    }
}
