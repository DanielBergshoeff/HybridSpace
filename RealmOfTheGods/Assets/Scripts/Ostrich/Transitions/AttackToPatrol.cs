using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Transition/AttackToPatrol")]
public class AttackToPatrol : Transition
{
    public override bool ConditionTriggered(FiniteStateMachine stateMachine)
    {
        return ChasedPlayerDead(stateMachine);
    }

    private bool ChasedPlayerDead(FiniteStateMachine stateMachine)
    {
        //return player is dead;
        return true;
    }
}
