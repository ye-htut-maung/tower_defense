using UnityEngine;

public static class ProjectileAiming
{
    // Solve |targetPosition + velocity*t - origin| = projectileSpeed*t.
    public static bool TryGetInterceptPoint(Vector3 origin, Vector3 targetPosition,
        Vector3 velocity, float projectileSpeed, float maximumFlightTime, out Vector3 point)
    {
        point = targetPosition;
        if (projectileSpeed <= 0f || maximumFlightTime <= 0f) return false;

        Vector3 offset = targetPosition - origin;
        float c = offset.sqrMagnitude;
        if (c < 0.000001f) return true;
        float a = velocity.sqrMagnitude - projectileSpeed * projectileSpeed;
        float b = 2f * Vector3.Dot(offset, velocity);
        float time;
        if (Mathf.Abs(a) < 0.000001f)
        {
            if (Mathf.Abs(b) < 0.000001f) return false;
            time = -c / b;
        }
        else
        {
            float discriminant = b * b - 4f * a * c;
            if (discriminant < 0f) return false;
            float root = Mathf.Sqrt(discriminant);
            float first = (-b - root) / (2f * a);
            float second = (-b + root) / (2f * a);
            time = first > 0f ? first : second;
            if (second > 0f) time = Mathf.Min(time, second);
        }
        if (float.IsNaN(time) || float.IsInfinity(time) || time <= 0f || time > maximumFlightTime)
            return false;
        point = targetPosition + velocity * time;
        return true;
    }
}
