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
        //Keep chasing the chased target.
        stateMachine.navAgent.SetDestination(stateMachine.chaseTarget.position);
    }

    //Checks every player in range of the ostricht.
    //If the player has the egg, he will be the chased target of the ostrich.
    //If no player has the egg, the ostrich will start chasing the closest player (AT THAT POINT, the ostrich will not keep switching targets until
    //the currently chased player has left the radius or has died.

    //TODO Properly implement the check whether a player has the egg.
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
