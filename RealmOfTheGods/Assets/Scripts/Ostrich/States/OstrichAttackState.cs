using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State/OstrichAttack")]
public class OstrichAttackState : State
{
    public override void OnEnter(FiniteStateMachine stateMachine)
    {
        //Attack/kill the chased player, trigger animation etc.
    }

    public override void OnExit(FiniteStateMachine stateMachine)
    {
        stateMachine.chaseTarget = null;
    }

    public override void OnUpdate(FiniteStateMachine stateMachine)
    {
        //Is there anything you want to do while this state is running?
    }
}
