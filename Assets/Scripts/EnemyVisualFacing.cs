using UnityEngine;

// Rotate only the model; the gameplay root, route and collider stay unchanged.
public class EnemyVisualFacing : MonoBehaviour
{
    [SerializeField] private Transform gameplayRoot;
    private Vector3 previousPosition;

    private void OnEnable()
    {
        previousPosition = gameplayRoot.position;
    }

    private void LateUpdate()
    {
        Vector3 direction = gameplayRoot.position - previousPosition;
        previousPosition = gameplayRoot.position;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.00000001f)
            transform.rotation = Quaternion.LookRotation(direction);
    }
}
