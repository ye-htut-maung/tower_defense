using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(EnemyHealth))]
public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Transform barRoot;
    [SerializeField] private Image fill;
    [SerializeField, Min(0f)] private float verticalOffset = 1.25f;

    private EnemyHealth health;
    private Camera viewCamera;

    private void Awake()
    {
        health = GetComponent<EnemyHealth>();
        viewCamera = Camera.main;
    }

    private void LateUpdate()
    {
        bool visible = health.CurrentHealth > 0 && health.CurrentHealth < health.MaxHealth;
        barRoot.gameObject.SetActive(visible);
        if (!visible) return;

        float fraction = Mathf.Clamp01((float)health.CurrentHealth / health.MaxHealth);
        fill.rectTransform.anchorMax = new Vector2(fraction, 1f);
        barRoot.position = transform.position + Vector3.up * verticalOffset;
        if (viewCamera == null) viewCamera = Camera.main;
        if (viewCamera != null) barRoot.rotation = viewCamera.transform.rotation;
    }

    private void OnDisable()
    {
        if (barRoot != null) barRoot.gameObject.SetActive(false);
    }
}
