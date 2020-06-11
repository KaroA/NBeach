using UnityEngine;
using UnityEditor;

[CreateAssetMenu (menuName = "AI/Darkness/State/ChaseState")]
public class ChaseState : Dark_State
{

    protected override void FirstTimeSetup()
    {
        stateType = StateType.CHASING;
    }

    public override void InitializeState(Darkness controller)
    {
        base.InitializeState(controller);
        Darkness_Manager.OnRequestNewTarget(controller.creationID, false);
        controller.UpdatePath();
        controller.moving = true;
    }

    public override void UpdateState(Darkness controller)
    {
        controller.UpdatePath();
        controller.UpdateAnimator(this.stateType);
        CheckTransitions(controller);
    }

    public override void ExitState(Darkness controller)
    {
        //base.ExitState(controller);
        controller.EndMovement();
    }
}