using System.Collections.Generic;
using System;

namespace Ardot.REPO.EnemyOverhaul;

public class StateMachine<TState>(Action stateMachineAction) where TState : Enum
{
    public TState State;
    public TState FutureState;
    public float StateTimer;
    
    public bool StateStartImpulse = false;
    public bool StateEndImpulse = false;

    public enum StateSetMode
    {
        None,
        SetThisUpdate,
        Set,
    }

    public StateSetMode SetMode = StateSetMode.None;

    public Action StateMachineAction = stateMachineAction;

    public List<object> StateData = new ();
    public List<float> StateTimers = new ();

    public void Update (float deltaTime)
    {
        StateTimer -= deltaTime;

        for(int x = 0; x < StateTimers.Count; x++)
            StateTimers[x] -= deltaTime;

        if(SetMode == StateSetMode.Set)
            StateEndImpulse = true;

        StateMachineAction();

        if(SetMode == StateSetMode.Set)
        {
            State = FutureState;
            StateStartImpulse = true;
            SetMode = StateSetMode.None;
            StateData.Clear();
            StateTimers.Clear();
        }

        if(SetMode == StateSetMode.SetThisUpdate)
            SetMode = StateSetMode.Set;

        StateEndImpulse = false;
    }

    public bool SetState(TState state, float time)
    {
        if(SetMode != StateSetMode.None)
            return false;
            
        StateTimer = time;
        FutureState = state;
        SetMode = StateSetMode.SetThisUpdate;
        return true;
    }

    public bool ConsumeStateImpulse()
    {
        if(StateStartImpulse)
        {
            StateStartImpulse = false;
            return true;
        }

        return false;
    }

    public T GetStateData<T>(int index)
    {
        return (T)StateData[index];
    }

    public void SetStateData<T>(int index, T value)
    {
        while(StateData.Count <= index)
            StateData.Add(default);

        StateData[index] = value;
    }

    public float GetStateTimer(int index)
    {
        return StateTimers[index];
    }

    public void SetStateTimer(int index, float time)
    {
        while(StateTimers.Count <= index)
            StateTimers.Add(0);
        
        StateTimers[index] = time;
    }
}