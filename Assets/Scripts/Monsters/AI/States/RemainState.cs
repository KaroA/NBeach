using UnityEngine;
using UnityEditor;

[CreateAssetMenu (menuName = "AI/Darkness/State/RemainState")]
public class RemainState: Dark_State
{
    public override void InitializeState(Darkness controller) {
        stateType = StateType.REMAIN;
    }
    public override void UpdateState(Darkness controller) {}
    protected override void ExitState(Darkness controller) {}
}