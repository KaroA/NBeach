using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Dark_State : ScriptableObject
{
    public enum StateType { CHASING, IDLE, ATTACK, DEATH, PAUSE, REMAIN, WANDER }

    public enum TargetType { DIRECT_PLAYER, FLANK_PLAYER, PATROL}
    public StateType stateType;
    public Dark_Transition[] transitions;
    public List<Dark_State> ReferencedBy;
    //protected Lookup<AI_Transition.Transition_Priority, AI_Transition> priorityTransitions;


    [SerializeField, Range(0, 3)]
    public float speedModifier;

    [SerializeField, Range(0, 15)]
    protected float stopDist;

    [SerializeField, Range(0,360)]
    protected int rotationSpeed;

    [SerializeField, Range(0, 5)]
    protected float pathUpdateRate;



    public virtual void Startup()
    {
        AI_Manager.RemoveDarkness += RemoveDarkness;
        if(ReferencedBy == null || ReferencedBy.Count < 1)
            ReferencedBy = new List<Dark_State>();
        foreach(Dark_Transition ai in transitions)
        {
            if(!ai.trueState.ReferencedBy.Contains(this))
                ai.trueState.ReferencedBy.Add(this);
            if(!ai.falseState.ReferencedBy.Contains(this))
                ai.falseState.ReferencedBy.Add(this);
        }
    }

    public virtual void InitializeState(Darkness controller)
    {
        controller.pather.rotationSpeed = rotationSpeed;
        controller.pather.endReachedDistance = stopDist;
        controller.pather.maxSpeed = speedModifier;
        controller.pather.repathRate = pathUpdateRate;
    }
    public abstract void UpdateState(Darkness controller);
    public abstract void ExitState(Darkness controller);

    protected virtual void FirstTimeSetup()
    {
        stateType = StateType.REMAIN;
    }

    protected void CheckTransitions(Darkness controller)
    {
        for(int i = 0; i < transitions.Length; i++)
        {
            bool decisionResult = transitions[i].decision.MakeDecision(transitions[i].decisionChoice,controller);
            if(decisionResult) 
            {
                if(transitions[i].trueState.stateType == StateType.REMAIN)
                    continue;
                else ProcessStateChange(transitions[i].trueState, controller); 
            }
            else if(!decisionResult) 
            {
                if(transitions[i].falseState.stateType == StateType.REMAIN)
                    continue;
                else ProcessStateChange(transitions[i].falseState, controller);
            }
        }   
    }

    protected void ProcessStateChange(Dark_State approvedState, Darkness controller) //TODO Have Darkness start a coroutine to begin transitioning. 
    {
        controller.ChangeState(approvedState);
    }

    protected void RemoveDarkness(Darkness controller)
    {
        if(stateType != Dark_State.StateType.DEATH)
        {
            this.ExitState(controller);
            controller.updateStates = false;
            //controller.ChangeState(controller.DeathState);
        }
    }
}