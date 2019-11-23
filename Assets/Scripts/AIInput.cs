using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInput : MonoBehaviour
{
    public float guidePointDistance;
    public float fallbackRaycastDistance;
    public Path path;
    public Transform car;
    public float feelerRadius;
    public CarBehaviour behaviour;
    private LayerMask wallMask;

    void Awake()
    {
        wallMask = LayerMask.GetMask("Wall");
    }

    void Update()
    {
        // always accelerate, assume no turn
        behaviour.accelerationInput = 1;
        behaviour.turnInput = 0;

        // find path goal point and vector perpendicular to track direction
        PathPointInfo ppi = path.FindClosestLeadingPoint(car.position, guidePointDistance);
        Vector3 perpendicular = new Vector3(ppi.direction.z, 0, -ppi.direction.x);

        // grab the car's origin point to raycast from
        Vector3 carPos = car.position;
        carPos.y = ppi.point.y;

        // find the intersection between the car's heading and perpendicular to the track
        Vector3 intersect = GetLineIntersectionPoint(carPos, carPos + car.forward, ppi.point, ppi.point + perpendicular, out bool found);
        if (found)
        {
            intersect.y = ppi.point.y;
            float feelerDistance = (intersect - ppi.point).magnitude;

            // check if the feeler is outside the path
            if (feelerDistance > path.radius - feelerRadius)
            {
                // going to hit a wall, time to correct
                // turn toward the goal point
                float dir = Vector3.Dot(car.right, ppi.point - carPos);
                if (Mathf.Abs(dir) > Mathf.Epsilon)
                {
                    behaviour.turnInput = dir / Mathf.Abs(dir);
                    return;
                }
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("No intersection found, using fallback raycast.");
#endif
            // raycast the max distance
            if (Physics.Raycast(carPos, car.forward, fallbackRaycastDistance, wallMask)
                || Physics.SphereCast(carPos, 1, car.forward, out RaycastHit _, fallbackRaycastDistance, wallMask))
            {
                // going to hit a wall, time to correct
                // turn toward the goal point
                float dir = Vector3.Dot(car.right, ppi.point - carPos);
                if (Mathf.Abs(dir) > Mathf.Epsilon)
                {
                    behaviour.turnInput = dir / Mathf.Abs(dir);
                    return;
                }
            }
        }

        // if we get here without returning, apply random wander
        behaviour.turnInput += Random.Range(-1f, 1f) * 2 *  Time.fixedDeltaTime;
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
        PathPointInfo ppi = path.FindClosestLeadingPoint(car.position, guidePointDistance);
        Vector3 perpendicular = new Vector3(ppi.direction.z, 0, -ppi.direction.x);

        // draw goal point feeler
        Vector3 intersect = GetLineIntersectionPoint(car.position, car.position + car.forward, ppi.point, ppi.point + perpendicular, out bool found);
        if (found)
        {
            intersect.y = ppi.point.y;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(ppi.point, intersect);
            Gizmos.DrawWireSphere(intersect, feelerRadius);
        }

        // draw steering input
        Gizmos.color = Color.green;
        Gizmos.DrawLine(car.position, car.position + behaviour.turnInput * car.right * 5);

        // draw goal point
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(ppi.point, 0.25f);
        Gizmos.DrawLine(ppi.point, ppi.point + ppi.direction);
    }
#endif
}
