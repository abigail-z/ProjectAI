using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalState : InputStateMachine.State
{
    private readonly AIInput owner;
    private float wander;
    private readonly float wanderStrength;
    private readonly CollisionNotifier collisionNotifier;
    private readonly CollisionNotifier.OnCollision collisionDelegate;

    public NormalState(AIInput owner, float wanderStrength, CollisionNotifier collisionNotifier)
    {
        this.owner = owner;
        this.wanderStrength = wanderStrength;
        this.collisionNotifier = collisionNotifier;
        collisionDelegate = new CollisionNotifier.OnCollision(OnCollision);
    }


    public override void Enter()
    {
#if UNITY_EDITOR
        Debug.Log("Normal State");
#endif

        base.Enter();

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
            // apply random wander if we did not turn
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
        base.Exit();

        collisionNotifier.CollisionEvent -= collisionDelegate;
    }

    void OnCollision(Collision col)
    {
        // only keep track of collisions with cars
        if (Active && col.transform.CompareTag("Car"))
        {
            StateMachine.ChangeToState<AggressiveState>();
        }
    }
}
