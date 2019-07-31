using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : State
{
    public Idle( Duck duck ) : base(duck) { }

    public override void Enter()
    {
        //수치 증가속도 초기화
        hungerPace = 2f;
        fatiguePace = 2f;
        Debug.Log("대기!");
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        if( owner.Fatigue >= 7 )
        {
            owner.ChangeState(owner.stateList["Sleep"]);
            return;
        }

        if( owner.Hunger >= 7 )
        {
            owner.ChangeState(owner.stateList["Eat"]);
            return;
        }
    }

}
