using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{

    public StateMachine  fsm;
    public IdleState idle;
    public MoveState move;


    // Start is called before the first frame update
    void Awake()
    {
        fsm=new StateMachine();
        idle=new IdleState();
        move=new MoveState();

        fsm.Initialize(idle);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
