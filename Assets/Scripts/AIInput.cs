using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInput : MonoBehaviour
{
    public float guidePointDistance;
    public float feelerRadius;
    public float wanderStrength;
    public Path path;
    public Transform car;
    public CarBehaviour behaviour;
    private float wander;

    void Update()
    {
        // always accelerate, reset steering
        behaviour.accelerationInput = 1;
        behaviour.turnInput = 0;

        // find path goal point and vector perpendicular to track direction
        PathPointInfo ppi = path.FindClosestLeadingPoint(car.position, guidePointDistance);
        Vector3 perpendicular = new Vector3(ppi.direction.z, 0, -ppi.direction.x);

        // grab the car's origin point to raycast from
        Vector3 carPos = car.position;
        carPos.y = ppi.point.y;

        // find the intersection between the car's heading and perpendicular to the track
        Vector3 intersect = VectorUtil.GetLineIntersectionPoint(carPos, carPos + car.forward, ppi.point, ppi.point + perpendicular, out bool found);
        bool doTurn = false;
        // also turn if the intercept is behind the car, this means the car is backward
        if (found && Vector3.Dot(car.forward, intersect) >= 0)
        {
            // if there is an intercept, that is the feeler
            // check if the feeler falls outside the path
            intersect.y = ppi.point.y;
            float feelerDistance = (intersect - ppi.point).magnitude;
            if (feelerDistance + feelerRadius > path.radius)
            {
                // if it does, we're going to hit a wall, time to correct
                doTurn = true;
            }
        }
        else
        {
            // no intercept found, steer toward the goal
            doTurn = true;
        }

        if (doTurn)
        {
            // turn toward the goal point
            float dir = Vector3.Dot(car.right, ppi.point - carPos);
            if (Mathf.Abs(dir) > Mathf.Epsilon)
            {
                behaviour.turnInput = dir / Mathf.Abs(dir);
                // reset wander
                wander = 0;
                return;
            }
        }
        
        // apply random wander if we did not turn
        wander += Random.Range(-1f, 1f) * wanderStrength * Time.fixedDeltaTime;
        behaviour.turnInput = wander;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        PathPointInfo ppi = path.FindClosestLeadingPoint(car.position, guidePointDistance);
        Vector3 perpendicular = new Vector3(ppi.direction.z, 0, -ppi.direction.x);

        // draw goal point feeler
        Vector3 intersect = VectorUtil.GetLineIntersectionPoint(car.position, car.position + car.forward, ppi.point, ppi.point + perpendicular, out bool found);
        if (found && Vector3.Dot(car.forward, intersect) >= 0)
        {
            Vector3 vehiclePoint = car.position;
            vehiclePoint.y = intersect.y = ppi.point.y;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(ppi.point, intersect);
            Gizmos.DrawLine(vehiclePoint, intersect);
            Gizmos.DrawWireSphere(intersect, feelerRadius);
        }

        // draw steering input
        Gizmos.color = Color.green;
        Gizmos.DrawLine(car.position, car.position + behaviour.turnInput * car.right * 5);

        // draw goal point
        if (Mathf.Abs(wander) > Mathf.Epsilon)
        {
            Gizmos.color = Color.blue;
        }
        else
        {
            Gizmos.color = Color.white;
        }
        Gizmos.DrawWireSphere(ppi.point, 0.25f);
        Gizmos.DrawLine(ppi.point, ppi.point + ppi.direction);
    }
#endif
}
