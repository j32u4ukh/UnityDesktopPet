using UnityEngine;

public static class Utils
{
    public static string formatString(this Vector2 vector, int digits = 3)
    {
        return string.Format($"({{0:F{digits}}}, {{1:F{digits}}})", vector.x, vector.y);
    }

    public static string formatString(this Vector3 vector, int digits = 3)
    {
        return string.Format($"({{0:F{digits}}}, {{1:F{digits}}}, {{2:F{digits}}})", vector.x, vector.y, vector.z);
    }
}
