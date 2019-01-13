using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Transition/PatrolToChase")]
public class PatrolToChase : Transition {


    public override bool ConditionTriggered(FiniteStateMachine stateMachine)
    {
        return InRangeOfPlayer(stateMachine);
    }

    //Return true if the ostrich is in range of any player.
    //Note: this was quite a simple fix, perhaps you have better ideas of registering detection of any player?
    //Note: The selection of which player to chase will be handled in the OstrichChaseState.

    bool InRangeOfPlayer(FiniteStateMachine stateMachine)
    {
        foreach (GameObject player in stateMachine.players)
        {
            Transform playerTransform = player.transform;
            float distanceToPlayer = Vector3.Distance(stateMachine.transform.position, playerTransform.position);

            if (distanceToPlayer <= stateMachine.radius)
            {
                return true;
            }
        }
        return false;
    }

    
}
