using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "State/AbstractState")]
public abstract class State : ScriptableObject {

    public List<Transition> transitions;

    public abstract void OnEnter(FiniteStateMachine stateMachine);
    public abstract void OnUpdate(FiniteStateMachine stateMachine);
    public abstract void OnExit(FiniteStateMachine stateMachine);

    public void AddTransition(Transition transition)
    {
        transitions.Add(transition);
    }

    public Transition triggeredTransition(FiniteStateMachine stateMachine)
    {
        foreach (Transition transition in transitions)
        {
            if (transition.ConditionTriggered(stateMachine))
            {
                return transition;
            }
        }
        return null;
    }
}
