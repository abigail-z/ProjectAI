using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInput : MonoBehaviour
{
    public Path path;
    public Transform car;

    void FixedUpdate()
    {
        PathPointInfo ppi = path.FindClosestLeadingPoint(car.position, 5);
    }

    public Vector3 GetIntersectionPoint(Vector3 A1, Vector3 A2, Vector3 B1, Vector3 B2, out bool found)
    {
        float tmp = (B2.x - B1.x) * (A2.z - A1.z) - (B2.z - B1.z) * (A2.x - A1.x);

        if (tmp == 0)
        {
            // No solution!
            found = false;
            return Vector2.zero;
        }

        float mu = ((A1.x - B1.x) * (A2.z - A1.z) - (A1.z - B1.z) * (A2.x - A1.x)) / tmp;

        found = true;

        return new Vector3(
            B1.x + (B2.x - B1.x) * mu,
            0,
            B1.z + (B2.z - B1.z) * mu
        );
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        PathPointInfo ppi = path.FindClosestLeadingPoint(car.position, 5);
        Vector3 perpendicular = new Vector3(ppi.direction.z, 0, -ppi.direction.x);

        Vector3 intersect = GetIntersectionPoint(car.position, car.position + car.forward, ppi.point, ppi.point + perpendicular, out bool found);
        if (found)
        {
            intersect.y = ppi.point.y;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(ppi.point, intersect);
            Gizmos.DrawWireSphere(intersect, 0.1f);
        }

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(ppi.point, 0.25f);
        Gizmos.DrawLine(ppi.point, ppi.point + ppi.direction);
    }
#endif
}
