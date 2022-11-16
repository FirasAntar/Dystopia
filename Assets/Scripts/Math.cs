using UnityEngine;

public static class Math
{
    public static Vector3 vectorAB(Vector3 A, Vector3 B)
    {
        return (B - A);
    }
    public static Vector3 vectorABnormalized(Vector3 A, Vector3 B)
    {
        return (B - A).normalized;
    }
    public static Vector3 vectorABXZ(Vector3 A, Vector3 B)
    {
        Vector3 ab = B - A;
        ab.y = 0;
        return ab;
    }
    public static float distanceAB(Vector3 A, Vector3 B)
    {
        return vectorAB(A, B).magnitude;
    }
    public static float dampFloat(float from, float to, float speed, float deltaTime)
    {
        return Mathf.Lerp(from, to, 1.0f - Mathf.Exp(-speed * deltaTime));
    }
    public static Vector3 dampVector3(Vector3 from, Vector3 to, float speed, float deltaTime)
    {
        return Vector3.Lerp(from, to, 1.0f - Mathf.Exp(-speed * deltaTime));
    }
    public static Quaternion dampQuaternion(Quaternion from, Quaternion to, float speed, float deltaTime)
    {
        return Quaternion.Slerp(from, to, 1.0f - Mathf.Exp(-speed * deltaTime));
    }
    public static float Angle(Vector3 A, Vector3 B)
    {
        float angle = Vector3.Angle(A, B);
        Vector3 cross = Vector3.Cross(A, B);
        if (cross.y < 0) angle = -angle;
        return angle;
    }
}
