using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressiveState : InputStateMachine.State, ICollisionSubscriber
{
    private readonly AIInput owner;
    private float wander;
    private readonly float wanderStrength;
    private readonly LayerMask carMask;
    private readonly float guidePointDistance;
    private readonly float maxTime;
    private float elapsedTime;

    public AggressiveState(AIInput owner, float wanderStrength, float guidePointDistance, float time)
    {
        this.owner = owner;
        this.wanderStrength = wanderStrength;
        this.guidePointDistance = guidePointDistance;
        maxTime = time;
        carMask = LayerMask.GetMask("Car");
    }

    public override void Enter()
    {
#if UNITY_EDITOR
        Debug.Log("Aggressive State");
#endif

        base.Enter();

        owner.guidePointDistance /= 2;
        elapsedTime = 0;
    }

    public override CarInput Execute()
    {
        // check if state has any time remaining
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= maxTime)
        {
            StateMachine.ChangeToState<NormalState>();
        }

        // do path follow
        float pathFollowInput = owner.PathFollowInput();
        if (Mathf.Abs(pathFollowInput) > 0)
        {
            // need to turn for path follow, need to reset wander
            wander = 0;
            return new CarInput
            {
                acceleration = 1,
                turn = pathFollowInput
            };
        }
        else
        {
            // find closest car
            Vector3 closest = Vector3.zero;
            Collider[] colliders = Physics.OverlapSphere(owner.car.position, 10, carMask);
            foreach (Collider col in colliders)
            {
                if (col.transform.root != owner.transform && (col.transform.position - owner.car.position).magnitude > closest.magnitude)
                {
                    closest = col.transform.position - owner.car.position;
                }
            }

            // try to steer into it
            float dir = Vector3.Dot(owner.car.right, closest);
            if (Mathf.Abs(dir) > Mathf.Epsilon)
            {
                // get sign of direction
                dir /= Mathf.Abs(dir);

                // if the target is behind us, slow down
                float accel = 1;
                float lead = Vector3.Dot(owner.car.forward, closest);
                if (lead < 0)
                {
                    accel = 0.9f;
                }

                return new CarInput
                {
                    acceleration = accel,
                    turn = dir
                };
            }

            // apply random wander if we did not turn for any other reason
            wander += Random.Range(-1f, 1f) * wanderStrength * Time.fixedDeltaTime;
            return new CarInput
            {
                acceleration = 1,
                turn = wander
            };
        }
    }

    public override void Exit()
    {
        base.Exit();

        owner.guidePointDistance = guidePointDistance;
    }

    public void OnCollision(Collision col)
    {
        // only keep track of collisions with cars
        if (Active && col.transform.CompareTag("Car"))
        {
            elapsedTime = 0;
        }
    }
}
