using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    void Enter();
    CarInput Execute();
    void Exit();
}

public class InputStateMachine
{
    IState currentState;

    public void ChangeState(IState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }

    public CarInput Update()
    {
        if (currentState != null) return currentState.Execute();

        return new CarInput
        {
            acceleration = 0,
            turn = 0
        };
    }
}
