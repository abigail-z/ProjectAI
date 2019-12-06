using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStateMachine
{
    private State currentState;
    private readonly List<State> states = new List<State>();

    public void AddState(State newState)
    {
        states.Add(newState);
        newState.OnAddToStateMachine(this);
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

    public void ChangeToState<T>()
    {
        foreach (State s in states)
        {
            if (s is T)
            {
                if (currentState != null)
                    currentState.Exit();

                currentState = s;

                currentState.Enter();
                return;
            }
        }
    }

    public abstract class State
    {
        public InputStateMachine StateMachine { get; private set; }

        public void OnAddToStateMachine(InputStateMachine sm)
        {
            StateMachine = sm;
        }

        public abstract void Enter();

        public abstract void Exit();

        public abstract CarInput Execute();
    }
}
