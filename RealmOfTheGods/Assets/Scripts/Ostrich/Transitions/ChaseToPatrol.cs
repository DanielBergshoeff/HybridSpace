using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Transition/ChaseToPatrol")]
public class ChaseToPatrol : Transition {

    public override bool ConditionTriggered(FiniteStateMachine stateMachine)
    {
        return TargetOutOfRange(stateMachine);
    }

    private bool TargetOutOfRange(FiniteStateMachine stateMachine)
    {
        return Vector3.Distance(stateMachine.transform.position, stateMachine.chaseTarget.position) > stateMachine.radius;
    }
}
