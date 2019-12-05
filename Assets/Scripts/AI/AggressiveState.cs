using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressiveState : InputStateMachine.State
{
    private readonly AIInput owner;
    private float wander;
    private readonly float wanderStrength;
    private readonly float guidePointDistance;
    private readonly float maxTime;
    private float elapsedTime;
    private readonly CollisionNotifier collisionNotifier;
    private readonly CollisionNotifier.OnCollision collisionDelegate;

    public AggressiveState(AIInput owner, float wanderStrength, float guidePointDistance, float time, CollisionNotifier collisionNotifier)
    {
        this.owner = owner;
        this.wanderStrength = wanderStrength;
        this.guidePointDistance = guidePointDistance;
        maxTime = time;
        this.collisionNotifier = collisionNotifier;
        collisionDelegate = new CollisionNotifier.OnCollision(OnCollision);
    }

    public override void Enter()
    {
        base.Enter();

        owner.angerSmoke.Play();
        owner.guidePointDistance /= 2;
        elapsedTime = 0;
        collisionNotifier.CollisionEvent += collisionDelegate;
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
            // if the opponent is nearby, steer into them
            Vector3 carOffset = owner.otherCar.car.position - owner.car.position;
            if (carOffset.magnitude < 10)
            {
                // try to steer into it
                float dir = Vector3.Dot(owner.car.right, carOffset);
                if (Mathf.Abs(dir) > Mathf.Epsilon)
                {
                    // get sign of direction
                    dir /= Mathf.Abs(dir);

                    // if the target is behind us, slow down
                    float accel = 1;
                    float lead = Vector3.Dot(owner.car.forward, carOffset);
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

        owner.angerSmoke.Stop();
        owner.guidePointDistance = guidePointDistance;
        collisionNotifier.CollisionEvent -= collisionDelegate;
    }

    void OnCollision(Collision col)
    {
        // only keep track of collisions with cars
        if (Active && col.transform.CompareTag("Car"))
        {
            elapsedTime = 0;
        }
    }
}
