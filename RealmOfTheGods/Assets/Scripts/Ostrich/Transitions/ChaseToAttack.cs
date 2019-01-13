using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Transition/ChaseToAttack")]
public class ChaseToAttack : Transition
{
    public override bool ConditionTriggered(FiniteStateMachine stateMachine)
    {
        return ReachedTarget(stateMachine);
    }

    private bool ReachedTarget(FiniteStateMachine stateMachine)
    {
        return stateMachine.navAgent.remainingDistance < 0.5f;
    }
}
