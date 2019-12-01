using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStateMachine
{
    private IState currentState;
    private readonly List<IState> states = new List<IState>();

    public void ChangeToState<IState>()
    {
        if (currentState != null)
            currentState.Exit();

        foreach (InputStateMachine.IState s in states)
        {
            if (s is IState)
            {
                currentState = s;
                break;
            }
        }

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

    public void AddState(IState newState)
    {
        states.Add(newState);
        newState.OnAddToStateMachine(this);
    }

    public abstract class IState
    {
        public InputStateMachine StateMachine { get; private set; }

        public void OnAddToStateMachine(InputStateMachine sm)
        {
            StateMachine = sm;
        }

        public void Enter() { }

        public void Exit() { }

        public abstract CarInput Execute();
    }
}
