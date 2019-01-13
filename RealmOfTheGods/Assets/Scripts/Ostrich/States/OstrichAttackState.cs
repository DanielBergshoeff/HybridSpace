using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State/OstrichAttack")]
public class OstrichAttackState : State
{
    //TODO Implement the killing of the player.
    public override void OnEnter(FiniteStateMachine stateMachine)
    {
        //Attack/kill the chased player, trigger animation etc.
    }

    public override void OnExit(FiniteStateMachine stateMachine)
    {
        //Reset the chase target
        //TODO Remove the player from the available chaseable players.
        stateMachine.chaseTarget = null;
    }

    public override void OnUpdate(FiniteStateMachine stateMachine)
    {
        //Is there anything you want to do while this state is running?
    }
}
