using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStateMachine
{
    private State currentState;
    private readonly List<State> states = new List<State>();

    public void ChangeToState<T>()
    {
        if (currentState != null)
            currentState.Exit();

        foreach (State s in states)
        {
            if (s is T)
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

    public void AddState(State newState)
    {
        states.Add(newState);
        newState.OnAddToStateMachine(this);
    }

    public abstract class State
    {
        public InputStateMachine StateMachine { get; private set; }
        public bool Active { get; private set; }

        public void OnAddToStateMachine(InputStateMachine sm)
        {
            StateMachine = sm;
        }

        public virtual void Enter()
        {
            Active = true;
        }

        public virtual void Exit()
        {
            Active = false;
        }

        public abstract CarInput Execute();
    }
}
