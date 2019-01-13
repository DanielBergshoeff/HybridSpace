using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FiniteStateMachine : MonoBehaviour{

    [SerializeField]
    private State startState;

    private State currentState;

    public List<GameObject> waypoints;
    public Transform walkTarget;
    public List<GameObject> players;
    public Transform chaseTarget;
    public NavMeshAgent navAgent;
    public float radius;

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();

        currentState = startState;
        currentState.OnEnter(this);
    }
	
	void Update () {
        Transition triggeredTransition = currentState.triggeredTransition(this);
        if (triggeredTransition != null)
        {
            Debug.Log(triggeredTransition.debugText);
            currentState.OnExit(this);
            currentState = triggeredTransition.GetNextState();
            currentState.OnEnter(this);
        }
        currentState.OnUpdate(this);
	}

}
