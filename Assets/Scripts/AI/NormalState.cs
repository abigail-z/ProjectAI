﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalState : InputStateMachine.State, ICollisionSubscriber
{
    private readonly AIInput owner;
    private float wander;
    private readonly float wanderStrength;
    private readonly int collisionsUntilAggressive;
    private int collisions;

    public NormalState(AIInput owner, float wanderStrength, int collisionsUntilAggressive)
    {
        this.owner = owner;
        this.wanderStrength = wanderStrength;
        this.collisionsUntilAggressive = collisionsUntilAggressive;
    }

    public override void Enter()
    {
        collisions = 0;
    }

    public override CarInput Execute()
    {
        if (collisions > collisionsUntilAggressive)
        {
            StateMachine.ChangeToState<AggressiveState>();
        }

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

    public void OnCollision(Collision col)
    {
        if (col.transform.CompareTag("Car"))
        {
            collisions += 1;
        }
    }
}
