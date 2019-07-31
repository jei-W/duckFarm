using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eat : State
{
    public Eat( Duck duck ) : base(duck) { }

    public override void Enter()
    {
        Debug.Log("배고파!");
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        // 먹을것을 찾아다닌다
        if( owner.Hunger == 0 )
        {
            owner.ChangeState(owner.stateList["Idle"]);
        }
    }
}
