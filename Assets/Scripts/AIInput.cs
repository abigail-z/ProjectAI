using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInput : MonoBehaviour
{
    public Path path;
    public Transform car;
    private CarBehaviour behaviour;
    private LayerMask wallMask;
    private Rigidbody sphere;

    void Awake()
    {
        behaviour = GetComponent<CarBehaviour>();
        wallMask = LayerMask.GetMask("Wall");
        sphere = transform.Find("Sphere").GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // always accelerate, assume no turn
        behaviour.accelerationInput = 1;
        behaviour.turnInput = 0;

        // find path goal point and vector perpendicular to track direction
        PathPointInfo ppi = path.FindClosestLeadingPoint(car.position, 10);
        Vector3 perpendicular = new Vector3(ppi.direction.z, 0, -ppi.direction.x);

        // grab the car's origin point to raycast from
        Vector3 carPos = car.position;
        carPos.y = ppi.point.y;


        // find the intersection between the car's heading and perpendicular to the track
        Vector3 intersect = GetLineIntersectionPoint(carPos, carPos + car.forward, ppi.point, ppi.point + perpendicular, out bool found);
        if (found)
        {
            // raycast to the intersect between the goal line and forward line
            intersect.y = ppi.point.y;
            float maxDistance = (intersect - carPos).magnitude;
            if (Physics.SphereCast(carPos, 1, intersect - carPos, out RaycastHit _, maxDistance, wallMask))
            {
                // going to hit a wall, time to correct
                // turn toward the goal point
                float dir = Vector3.Dot(car.right, ppi.point - carPos);
                if (dir != 0)
                {
                    behaviour.turnInput = dir / Mathf.Abs(dir);
                    return;
                }
            }
        }
        else
        {
            // raycast the max distance
            if (Physics.SphereCast(carPos, 1, car.forward, out RaycastHit _, 8, wallMask))
            {
                // going to hit a wall, time to correct
                // turn toward the goal point
                float dir = Vector3.Dot(car.right, ppi.point - carPos);
                if (dir != 0)
                {
                    behaviour.turnInput = dir / Mathf.Abs(dir);
                    return;
                }
            }
        }
    }

    public Vector3 GetLineIntersectionPoint(Vector3 A1, Vector3 A2, Vector3 B1, Vector3 B2, out bool found)
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
    void OnDrawGizmos()
    {
        PathPointInfo ppi = path.FindClosestLeadingPoint(car.position, 10);
        Vector3 perpendicular = new Vector3(ppi.direction.z, 0, -ppi.direction.x);

        Vector3 intersect = GetLineIntersectionPoint(car.position, car.position + car.forward, ppi.point, ppi.point + perpendicular, out bool found);
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
        Gizmos.DrawWireSphere(car.position, 1.5f);
    }
#endif
}
