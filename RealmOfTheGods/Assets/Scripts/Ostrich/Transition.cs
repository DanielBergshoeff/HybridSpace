using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Transition : ScriptableObject{

    public State nextState;
    public string debugText;

    public abstract bool ConditionTriggered(FiniteStateMachine stateMachine);

    public State GetNextState()
    {
        return nextState;
    }
}
