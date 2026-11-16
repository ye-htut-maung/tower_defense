using UnityEngine;

[DisallowMultipleComponent]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField, Min(0f)] private float movementSpeed = 4f;
    [SerializeField] private Transform[] waypoints;
    [SerializeField, Min(0f)] private float heightOffset = 1.1f;

    private int targetIndex;
    public Vector3 Velocity { get; private set; }
    public event System.Action ReachedGate;

    public void SetRoute(Transform[] route)
    {
        waypoints = route;
    }

    private void Start()
    {
        if (waypoints == null || waypoints.Length < 2)
        {
            Debug.LogError("EnemyMovement needs at least two waypoints in route order.", this);
            enabled = false;
            return;
        }

        foreach (Transform waypoint in waypoints)
        {
            if (waypoint == null)
            {
                Debug.LogError("EnemyMovement has an unassigned waypoint.", this);
                enabled = false;
                return;
            }
        }

        transform.position = waypoints[0].position + Vector3.up * heightOffset;
        targetIndex = 1;
        Velocity = (waypoints[1].position - waypoints[0].position).normalized * Mathf.Max(0f, movementSpeed);
    }

    private void Update()
    {
        if (targetIndex >= waypoints.Length)
        {
            Velocity = Vector3.zero;
            return;
        }

        Vector3 target = waypoints[targetIndex].position + Vector3.up * heightOffset;
        Vector3 previous = transform.position;
        transform.position = Vector3.MoveTowards(
            transform.position, target, Mathf.Max(0f, movementSpeed) * Time.deltaTime);
        Velocity = Time.deltaTime > 0f ? (transform.position - previous) / Time.deltaTime : Vector3.zero;

        // Reach the corner exactly before selecting the next straight segment.
        if (transform.position == target)
        {
            transform.position = target;
            targetIndex++;
            if (targetIndex == waypoints.Length)
            {
                Velocity = Vector3.zero;
                ReachedGate?.Invoke();
            }
        }
    }

    private void OnDisable()
    {
        Velocity = Vector3.zero;
    }
}
