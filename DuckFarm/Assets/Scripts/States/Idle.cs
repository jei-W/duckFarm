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
        randomPosition = RandomPosition();
        owner.GetComponent<NavMeshAgent>().autoBraking = false;
        
        Debug.Log("대기!");
    }

    public override void Exit()
    {
        owner.GetComponent<NavMeshAgent>().autoBraking = true;
        //ownerAgent.isStopped = true;
    }

    public override void Update()
    {
        base.Update();

        //대기상태에서는 멋대로 돌아다닌다
        if( owner.transform.parent == null )
        {
            // 이거.. 0.01f 좀 위험한뎅, 충돌나서.. 실제 거리는 엄청 멀텐뎅.
            if( ownerAgent.remainingDistance < 0.1f )
            {
                randomPosition = RandomPosition();
                owner.Move(randomPosition);
            }
        }

        if( owner.Fatigue >= 60 )
        {
            owner.ChangeState("Sleep");
            return;
        }

        if( owner.Hunger >= 70 )
        {
            owner.ChangeState("Eat");
            return;
        }

        //일정확률로 발정상태에 빠진다
        //if( Random.Range(1, 100) <= 30 )
        //    owner.ChangeState("Mating");
    }

    Vector3 RandomPosition()
    {
        float radius = 3f;
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += owner.transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if( NavMesh.SamplePosition(randomDirection, out hit, radius, 1) )
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }
}
