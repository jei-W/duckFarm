using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Idle : State
{
    Vector3 randomPosition;

    public Idle( Duck duck ) : base(duck) { }

    public override void Enter()
    {
        randomPosition = owner.transform.position;
        owner.GetComponent<NavMeshAgent>().autoBraking = false;
        
        Debug.Log("대기!");
    }

    public override void Exit()
    {
        owner.GetComponent<NavMeshAgent>().autoBraking = true;
    }

    public override void Update()
    {
        base.Update();

        if( owner.Fatigue >= 70 )
        {
            owner.ChangeState("Sleep");
            return;
        }

        if( owner.Hunger >= 70 )
        {
            owner.ChangeState("Eat");
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
            owner.Move(randomPosition);
        }

        //일정확률로 발정상태에 빠진다
        if( Random.Range(1, 100) <= 30 )
            owner.ChangeState("Mating");
    }
}
