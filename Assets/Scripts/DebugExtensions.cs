using UnityEngine;

public static class DebugExtensions
{
    public static void DrawSphere(Vector3 center, float radius, Color color, float duration = 4f, int segments = 16)
    {
        float angleStep = 360f / segments;

        // Draw the 3 primary orientation rings (XY, XZ, YZ planes)
        for (int i = 0; i < segments; i++)
        {
            float angle1 = Mathf.Deg2Rad * (i * angleStep);
            float angle2 = Mathf.Deg2Rad * ((i + 1) * angleStep);

            // Ring 1: Horizontal (XZ Plane)
            Vector3 pos1_XZ = center + new Vector3(Mathf.Cos(angle1) * radius, 0, Mathf.Sin(angle1) * radius);
            Vector3 pos2_XZ = center + new Vector3(Mathf.Cos(angle2) * radius, 0, Mathf.Sin(angle2) * radius);
            Debug.DrawLine(pos1_XZ, pos2_XZ, color, duration);

            // Ring 2: Vertical Face-on (XY Plane)
            Vector3 pos1_XY = center + new Vector3(Mathf.Cos(angle1) * radius, Mathf.Sin(angle1) * radius, 0);
            Vector3 pos2_XY = center + new Vector3(Mathf.Cos(angle2) * radius, Mathf.Sin(angle2) * radius, 0);
            Debug.DrawLine(pos1_XY, pos2_XY, color, duration);

            // Ring 3: Vertical Side-on (YZ Plane)
            Vector3 pos1_YZ = center + new Vector3(0, Mathf.Sin(angle1) * radius, Mathf.Cos(angle1) * radius);
            Vector3 pos2_YZ = center + new Vector3(0, Mathf.Sin(angle2) * radius, Mathf.Cos(angle2) * radius);
            Debug.DrawLine(pos1_YZ, pos2_YZ, color, duration);
        }
    }
}
