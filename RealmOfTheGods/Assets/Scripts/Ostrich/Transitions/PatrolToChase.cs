using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Transition/PatrolToChase")]
public class PatrolToChase : Transition {


    public override bool ConditionTriggered(FiniteStateMachine stateMachine)
    {
        return InRangeOfPlayer(stateMachine);
    }

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
