using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
     State currentState;
 

     public void Initialize(State startingState)
     {
        currentState=startingState;
        startingState.OnEnter();
     }

    public void ChangeState(State newState)
    {
        currentState.OnExit();

        currentState=newState;
        newState.OnEnter();
    }

}
