using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalState : InputStateMachine.State, ICollisionSubscriber
{
    private readonly AIInput owner;
    private float wander;
    private readonly float wanderStrength;

    public NormalState(AIInput owner, float wanderStrength)
    {
        this.owner = owner;
        this.wanderStrength = wanderStrength;
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

    public void OnCollision(Collision col)
    {
        // only keep track of collisions with cars
        if (col.transform.CompareTag("Car"))
        {
            StateMachine.ChangeToState<AggressiveState>();
        }
    }
}
