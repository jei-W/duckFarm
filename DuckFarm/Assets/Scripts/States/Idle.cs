using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Idle : State
{
    Vector3 randomPosition;

    public Idle( Duck duck ) : base(duck) { }

    public override void Enter( object extraData = null )
    {
        randomPosition = RandomPosition();
        owner.GetComponent<NavMeshAgent>().autoBraking = false;
        
        Debug.Log("대기!");
    }

    public override void Exit( )
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
            bool somethingWorking = false;
            // 일감의 우선순위가 없으므로.. 일단 물고기 잡는 일이 들어와 있는지 확인 해보자
            if ( World.GetInstance().IsJobEmpty(World.JobType.CatchFinshInPond) == false )
            {
                // 물고기 캐러 가자
                JobInfo job = World.GetInstance().GetFirstJob(World.JobType.CatchFinshInPond);
                if ( job == null )
                {
                }
                else
                {
                    owner.ChangeState("Fishing", job.targetBuilding);
                    somethingWorking = true;
                }
            }

            // 이거.. 0.01f 좀 위험한뎅, 충돌나서.. 실제 거리는 엄청 멀텐뎅.
            if ( somethingWorking == false && ownerAgent.remainingDistance < 0.1f )
            {
                // 일정확률로 발정상태에 빠진다
                // 태어난지 5일 이후? 혹은... 마지막 발정 이후 5일 이후 부터 가능
                if ( owner.LastMatingTime + World.oneDay * 1 >= World.CurrentGameWorldTimeMS && GlobalRandom.GetRandom(1, 100) <= owner.CurrentHeat )
                {
                    owner.ChangeState("Mating");
                }
                else
                {
                    randomPosition = RandomPosition();
                    owner.Move(randomPosition);
                }
            }

        }

        if ( owner.Fatigue >= 60 )
        {
            owner.ChangeState("Sleep");
            return;
        }

        if( owner.Hunger >= 70 )
        {
            owner.ChangeState("Eat");
            return;
        }
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
