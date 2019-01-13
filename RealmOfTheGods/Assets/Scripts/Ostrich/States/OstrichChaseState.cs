using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State/OstrichChase")]
public class OstrichChaseState : State {



    public override void OnEnter(FiniteStateMachine stateMachine)
    {
        SelectChaseTarget(stateMachine);
    }

    public override void OnExit(FiniteStateMachine stateMachine)
    {

    }

    public override void OnUpdate(FiniteStateMachine stateMachine)
    {
        stateMachine.navAgent.SetDestination(stateMachine.chaseTarget.position);
    }

    private void SelectChaseTarget(FiniteStateMachine stateMachine)
    {
        List<GameObject> players = stateMachine.players;
        Transform currentTarget = players[0].transform;

        foreach (GameObject player in players)
        {
            Transform playerTransform = player.transform;
            float distanceToPlayer = Vector3.Distance(stateMachine.transform.position, playerTransform.position);

            if (distanceToPlayer > stateMachine.radius)
            {
                continue;
            }

            //if (player has egg <-this condition is to be implemented) {
            //stateMachine.chaseTarget = currentTarget;
            //return;
            //}

            if (distanceToPlayer < Vector3.Distance(stateMachine.transform.position, currentTarget.position))
            {
                currentTarget = playerTransform;
            }
        }

        stateMachine.chaseTarget = currentTarget;
    }

}
