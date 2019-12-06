using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalState : InputStateMachine.State
{
    private readonly AIInput owner;
    private float wander;
    private readonly float wanderStrength;
    private readonly LayerMask coinMask;
    private readonly CollisionNotifier collisionNotifier;
    private readonly CollisionNotifier.OnCollision collisionDelegate;

    public NormalState(AIInput owner, float wanderStrength, CollisionNotifier collisionNotifier)
    {
        this.owner = owner;
        this.wanderStrength = wanderStrength;
        this.collisionNotifier = collisionNotifier;
        coinMask = LayerMask.GetMask("Coin");
        collisionDelegate = new CollisionNotifier.OnCollision(OnCollision);
    }


    public override void Enter()
    {
        // subscribe to collision notifier
        collisionNotifier.CollisionEvent += collisionDelegate;
    }


    public override CarInput Execute()
    {
        // do path follow
        float pathFollowInput = owner.PathFollowInput();
        if (Mathf.Abs(pathFollowInput) > 0)
        {
            // need to turn for path follow, need to reset wander
            wander = 0;
            return new CarInput
            {
                acceleration = 0.95f,
                turn = pathFollowInput
            };
        }
        else
        {
            // find closest coin
            Vector3 closest = Vector3.zero;
            Collider[] colliders = Physics.OverlapSphere(owner.car.position, 10, coinMask);
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

                return new CarInput
                {
                    acceleration = 0.95f,
                    turn = dir
                };
            }

            // apply random wander if we did not turn for any other reason
            wander += Random.Range(-1f, 1f) * wanderStrength * Time.fixedDeltaTime;
            return new CarInput
            {
                acceleration = 0.95f,
                turn = wander
            };
        }
    }
    public override void Exit()
    {
        // unsubsubscribe from collision notifier
        collisionNotifier.CollisionEvent -= collisionDelegate;
    }

    void OnCollision(Collision col)
    {
        // only keep track of collisions with cars
        if (col.transform.CompareTag("Car"))
        {
            StateMachine.ChangeToState<AggressiveState>();
        }
    }
}
