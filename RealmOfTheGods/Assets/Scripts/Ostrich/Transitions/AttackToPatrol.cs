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

    //Return true if the ostrich has finished killing, and the player is dead.
    //TODO Properly implement this function.
    private bool ChasedPlayerDead(FiniteStateMachine stateMachine)
    {
        //return player is dead;
        return true; 
    }
}
