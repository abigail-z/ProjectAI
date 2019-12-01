using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressiveState : InputStateMachine.State
{
    private readonly AIInput owner;
    private float wander;
    private readonly float wanderStrength;

    public AggressiveState(AIInput owner, float wanderStrength)
    {
        this.owner = owner;
        this.wanderStrength = wanderStrength;
    }

    public override CarInput Execute()
    {
        Debug.Log("I'm mad now!");

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
            // apply random wander if we did not turn
            wander += Random.Range(-1f, 1f) * wanderStrength * Time.fixedDeltaTime;
            return new CarInput
            {
                acceleration = 1,
                turn = wander
            };
        }
    }
}
