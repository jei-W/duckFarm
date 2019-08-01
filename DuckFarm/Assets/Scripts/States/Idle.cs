using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : State
{
    Vector3 randomPosition;

    public Idle( Duck duck ) : base(duck) { }

    public override void Enter()
    {
        randomPosition = owner.transform.position;

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

        //대기상태에서는 멋대로 돌아다닌다
        if( owner.transform.parent == null )
        {
            if( Mathf.Abs(( owner.transform.position - randomPosition ).x) < 0.01f
                && Mathf.Abs(( owner.transform.position - randomPosition ).y) < 0.01f )
            {
                randomPosition = new Vector3(Random.Range(-30f, 30f), 0, Random.Range(-10f, 10f));
            }
            owner.transform.position = Vector3.MoveTowards(owner.transform.position, randomPosition, owner.WalkSpeed);
        }
    }
}
