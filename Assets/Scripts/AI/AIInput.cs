using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInput : MonoBehaviour
{
    public float guidePointDistance;
    public float feelerRadius;
    public float wanderStrength;
    public float aggressiveTime;
    public int minBoost;
    public Path currentPath;
    public Path fastPath;
    public Path coinPath;
    public Transform car;
    public CarBehaviour behaviour;
    public ParticleSystem angerSmoke;
    public CarBehaviour otherCar;

    // state machine vars
    private InputStateMachine stateMachine;

    // decision tree vars
    private Decision<AIInput, Path> treeRoot;

    void Awake()
    {
        // state machine setup
        CollisionNotifier collisionNotifier = transform.Find("Sphere").GetComponent<CollisionNotifier>();
        stateMachine = new InputStateMachine();

        NormalState normalState = new NormalState(this, wanderStrength, collisionNotifier);
        stateMachine.AddState(normalState);

        AggressiveState aggressiveState = new AggressiveState(this, wanderStrength, guidePointDistance, aggressiveTime, collisionNotifier);
        stateMachine.AddState(aggressiveState);

        stateMachine.ChangeToState<NormalState>();

        // decision tree setup
        Decision<AIInput, Path> speedComparisonBranch = new DecisionQuery<AIInput, Path>
        {
            Test = (ai) => ai.otherCar.boostCount > ai.behaviour.boostCount,
            Positive = new DecisionResult<AIInput, Path>(coinPath),
            Negative = new DecisionResult<AIInput, Path>(fastPath)
        };

        Decision<AIInput, Path> minCountBranch = new DecisionQuery<AIInput, Path>
        {
            Test = (ai) => ai.behaviour.boostCount < ai.minBoost,
            Positive = new DecisionResult<AIInput, Path>(coinPath),
            Negative = speedComparisonBranch
        };

        treeRoot = new DecisionQuery<AIInput, Path>
        {
            Test = (ai) => ai.currentPath.CheckIfOnCriticalSection(ai.car.position),
            Positive = new DecisionResult<AIInput, Path>(null),
            Negative = minCountBranch
        };
    }

    void FixedUpdate()
    {
        Path newPath = treeRoot.Evaluate(this);
        currentPath = newPath ?? currentPath;

        behaviour.ApplyInput(stateMachine.Update());
    }

    public float PathFollowInput()
    {
        PathPointInfo goal = currentPath.FindClosestLeadingPoint(car.position, guidePointDistance);

        // match car height to goal height
        Vector3 carPos = car.position;
        carPos.y = goal.point.y;

        // get perpendicular line to the guide point's direction
        Vector3 perpendicular = new Vector3(goal.direction.z, 0, -goal.direction.x);

        bool doTurn = false;

        // find the intersection between the car's heading and perpendicular to the track
        Vector3 intersect = VectorUtil.GetLineIntersectionPoint(carPos, carPos + car.forward, goal.point, goal.point + perpendicular, out bool found);
        // also turn if the intercept is behind the car, this means the car is backward
        if (found && Vector3.Dot(car.forward, intersect - carPos) >= 0)
        {
            // if there is an intercept, that is the feeler
            // check if the feeler falls outside the path
            intersect.y = goal.point.y;
            float feelerDistance = (intersect - goal.point).magnitude;
            if (feelerDistance + feelerRadius > currentPath.radius)
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

        // if we do need to turn, return the direction
        if (doTurn)
        {
            float dir = Vector3.Dot(car.right, goal.point - carPos);
            if (Mathf.Abs(dir) > Mathf.Epsilon)
            {
                return dir / Mathf.Abs(dir);
            }
        }

        // don't need to turn
        return 0;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        PathPointInfo goal = currentPath.FindClosestLeadingPoint(car.position, guidePointDistance);
        Vector3 perpendicular = new Vector3(goal.direction.z, 0, -goal.direction.x);
        Vector3 carPos = car.position;
        carPos.y = goal.point.y;

        // draw goal point feeler
        Vector3 intersect = VectorUtil.GetLineIntersectionPoint(car.position, car.position + car.forward, goal.point, goal.point + perpendicular, out bool found);
        if (found && Vector3.Dot(car.forward, intersect - carPos) >= 0)
        {
            Vector3 vehiclePoint = car.position;
            vehiclePoint.y = intersect.y = goal.point.y;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(goal.point, intersect);
            Gizmos.DrawLine(vehiclePoint, intersect);
            Gizmos.DrawWireSphere(intersect, feelerRadius);
        }

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(goal.point, 0.25f);
        Gizmos.DrawLine(goal.point, goal.point + goal.direction);
    }
#endif
}
