using UnityEngine;
using System.Collections;

//[CreateAssetMenu (menuName = "AI/Darkness/State/AttackState")]
public class AttackState 
{
   /* [Range(1, 3)]
    public int attackSpeedModifier;
    [Range(0, 5)]
    public float attackCooldown;

    [Range(0, 3)]
    public float attackInitiationRange;


    protected override void FirstTimeSetup()
    {
        stateType = StateType.AGGRESSIVE;
    }

    public override void InitializeState(Darkness controller)
    {
        base.InitializeState(controller);
        Darkness_Manager.OnRequestNewTarget(controller.creationID, true);
        controller.pather.destination = controller.Target.location.position;
        if(controller.playerDist > attackInitiationRange)
            controller.pather.canMove = true;
        else controller.pather.canMove = false;
        controller.pather.canSearch = true;
    }

    public override void UpdateState(Darkness controller)
    { 
        //TODO check if the darkness is facing the player. if not start rotating towards the player
        //controller.pather.destination = controller.Target.location.position;
        //controller.UpdateAnimator(StateType.CHASING);
        if(controller.playerDist < attackInitiationRange && !controller.attacked) 
        {
            controller.attacked = true;
            //controller.UpdateAnimator(this.stateType);
            //controller.pather.canMove = false;
            controller.StartCoroutine(controller.AttackCooldown(attackCooldown));
            //if(controller.animeController.animation.)
            //controller.StartCoroutine(controller.AttackCooldown(attackCooldown, controller.idleHash));
        }   
        else 
        {
            controller.pather.canMove = true;
        }
        CheckTransitions(controller);
    }

    public override void ExitState(Darkness controller)
    {
        //base.ExitState(controller);
        //controller.pather.endReachedDistance -= 1.0f;
        //controller.attacked = false;
        //controller.animeController.SetBool(controller.attackAfterHash, true);
    }*/
}