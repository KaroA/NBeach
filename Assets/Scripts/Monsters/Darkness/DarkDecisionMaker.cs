using System;
using System.Collections.Generic;
using UnityEngine;


namespace DarknessMinion
{

    public class DarkDecisionMaker
    {
        public enum DecisionName { IsAggressive, IsIdling, PausedForNextCommand, IsWandering, InAttackRange, PlayerOutOfRange, AttackSuccessfull, NavTargetClose, IdleComplete }
        Dictionary<DecisionName, Func<Darkness, bool>> Decisions;

        public DarkDecisionMaker()
        {
            Decisions = new Dictionary<DecisionName, Func<Darkness, bool>>();
            Decisions.Add(DecisionName.IsAggressive, AggresiveCheck);
            Decisions.Add(DecisionName.PausedForNextCommand, PausedNextCommandCheck);
            Decisions.Add(DecisionName.IsWandering, WanderingCheck);
            Decisions.Add(DecisionName.PlayerOutOfRange, PlayerOutOfRangeCheck);
            Decisions.Add(DecisionName.InAttackRange, AttackRangeCheck);
            Decisions.Add(DecisionName.AttackSuccessfull, AttackOnCooldownCheck);
            Decisions.Add(DecisionName.NavTargetClose, NavTargetCloseCheck);
            Decisions.Add(DecisionName.IdleComplete, IdleOnCooldownCheck);
            Decisions.Add(DecisionName.IsIdling, IdlingCheck);
        }

        public bool MakeDecision(DecisionName dName, Darkness controller)
        {
            try
            {
                if (controller != null)
                    return Decisions[dName].Invoke(controller);
                else return false;
            }
            catch(KeyNotFoundException k)
            {
                Debug.LogError("Key not found in DarknDecisionMaker: " + dName.ToString() + "Resulting in this error " + k);
                return false;
            }
        }

        private bool IdleOnCooldownCheck(Darkness controller)
        {
            return !controller.CheckActionsOnCooldown(DarkState.CooldownStatus.Idling);
        }

        private bool AggresiveCheck(Darkness controller)
        {
           return controller.agRatingCurrent == Darkness.AggresionRating.Attacking;
        }

        private bool AttackRangeCheck(Darkness controller)
        {
            if (controller.playerDist < controller.swtichDist)
                return true;
            else return false;
        }

        private bool AttackSuccessfullCheck(Darkness controller)
        {
            if (controller.attacked)
                return true;
            else return false;
        }

        private bool AttackOnCooldownCheck(Darkness controller)
        {
            return controller.CheckActionsOnCooldown(DarkState.CooldownStatus.Attacking);
        }

        private bool PausedNextCommandCheck(Darkness controller)
        {
            if (controller.agRatingCurrent == Darkness.AggresionRating.Idling)
                return true;
            else return false;
        }

        private bool WanderingCheck(Darkness controller)
        {
            if (controller.agRatingCurrent == Darkness.AggresionRating.Wandering)
                return true;
            else return false;
        }

        private bool IdlingCheck(Darkness controller)
        {
            if(controller.agRatingCurrent == Darkness.AggresionRating.Idling)
                return true;
            else return false;
        }

        private bool PlayerOutOfRangeCheck(Darkness controller)
        {
            if (controller.playerDist > controller.swtichDist)
                return true;
            else return false;
        }

        private bool NavTargetCloseCheck(Darkness controller)
        {
            if (controller.navTargetDist < controller.swtichDist)
                return true;
            else return false;
        }
    }
}