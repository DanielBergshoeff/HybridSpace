using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "State/OstrichPatrol")]
public class OstrichPatrolState : State {


    public override void OnEnter(FiniteStateMachine stateMachine)
    {
        SelectWalkTarget(stateMachine);
    }

    public override void OnUpdate(FiniteStateMachine stateMachine)
    {
        //Switches waypoint when the ostrich has reached its current waypoint.
        if (ReachedWalkTarget(stateMachine))
        {
            SelectWalkTarget(stateMachine);
        }
    }

    public override void OnExit(FiniteStateMachine stateMachine)
    {
        stateMachine.walkTarget = null;
    }

    //Picks a random waypoint to go to.
    //Perhaps you'd like to have the ostrich walk in a certain path?
    void SelectWalkTarget(FiniteStateMachine stateMachine)
    {
        List<GameObject> waypoints = stateMachine.waypoints;
        int randomWaypoint = Random.Range(0, waypoints.Count);
        stateMachine.walkTarget = waypoints[randomWaypoint].transform;
        stateMachine.navAgent.SetDestination(stateMachine.walkTarget.position);
    }

    //Returns true if the ostrich has reached a waypoint.
    private bool ReachedWalkTarget(FiniteStateMachine stateMachine)
    {
        return stateMachine.navAgent.remainingDistance < 0.5f;
    }
}
