using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Dark_State : ScriptableObject
{
    public enum StateType {PASSIVE, AGGRESSIVE, DEATH, REMAIN}

    public StateType stateType;
    public Dark_Transition[] transitions;
    public Dark_Action[] actions;

    [SerializeField, Range(0, 3)]
    public float attackSpeedModifier;

    [SerializeField, Range(0, 15)]
    protected float stopDist;

    [SerializeField, Range(0,360)]
    protected int rotationSpeed;

    [SerializeField, Range(0, 5)]
    protected float pathUpdateRate;

    [SerializeField]
    protected bool hasExitTimer;

    [Range(0, 5)]
    public float exitTime;

    public virtual void InitializeState(DarknessMinion controller)
    {
        /*controller.pather.rotationSpeed = rotationSpeed;
        controller.pather.endReachedDistance = stopDist;
        controller.pather.maxSpeed = speedModifier;
        controller.pather.repathRate = pathUpdateRate;*/
    }
    public void UpdateState(DarknessMinion controller)
	{
		
	}
    public void ExitState(DarknessMinion controller)
    {
        
    }


    protected void ExecuteActions(DarknessMinion controller) //check if action has proper flags checked for 
    {
        foreach(Dark_Action d_Action in actions)
        {
            //if()
        }
    }

    protected void CheckTransitions(DarknessMinion controller)
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

    protected Vector3 RandomPoint(Vector3 center, float radiusLower, float radiusUpper)
    {
        Vector2 point = UnityEngine.Random.insideUnitCircle * Mathf.Sqrt(UnityEngine.Random.Range(radiusLower, radiusUpper));
        return new Vector3(point.x + center.x, center.y, point.y + center.z);
    }

    protected void ProcessStateChange(Dark_State approvedState, DarknessMinion controller)
    {
        controller.ChangeState(approvedState);
    }

    protected void RemoveDarkness(DarknessMinion controller)
    {
        if(stateType != Dark_State.StateType.DEATH)
        {
            this.ExitState(controller);
            controller.updateStates = false;
            //controller.ChangeState(controller.DeathState);
        }
    }
}