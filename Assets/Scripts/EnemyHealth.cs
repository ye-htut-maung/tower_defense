using UnityEngine;

[DisallowMultipleComponent]
public class EnemyHealth : MonoBehaviour
{
    [SerializeField, Min(1)] private int maxHealth = 3;

    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }
    public event System.Action<EnemyHealth> Defeated;

    private void Awake()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || CurrentHealth <= 0)
        {
            return;
        }

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);

        if (CurrentHealth == 0)
        {
            Defeated?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
