using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Idle : State
{
    Func<bool> changeStateToMatingCondition;
    Func<bool> jobConditionForFishing;
    Func<bool> jobConditionForLeftWork;
    Func<bool> surviveCondition;
    Func<bool> jobConditionForUserCommand;

    Action changeStateToMating;
    Action workForFishing;
    Action workForSomethingToLeft;
    Action changeStateToSurvive;
    Action workForUserCommand;



    public Idle( Duck duck ) : base(duck)
    {
        changeStateToMatingCondition = () => {
            // 일정확률로 발정상태에 빠진다
            // 태어난지 5일 이후? 혹은... 마지막 발정 이후 5일 이후 부터 가능
            if( owner.LastMatingTime + World.oneDay * 5 <= World.CurrentGameWorldTimeMS )
            {
                if( owner.CurrentHeat >= GlobalRandom.GetRandom(1, 100) )
                    return true;
            }

            return false;
        };
        changeStateToMating = () => owner.ChangeState("Mating");

        jobConditionForLeftWork = () => World.GetInstance().IsJobEmpty(World.JobType.CarrySomethingStopped) == false;
        workForSomethingToLeft = () => {
            JobInfo job = World.GetInstance().GetFirstJob(World.JobType.CarrySomethingStopped);
            if( job != null )
                owner.ChangeState("Carry", new Dictionary<string, ObjectBase>() {
                        { "target", job.targetObject },
                        { "targetBuilding", job.targetBuilding }
                });
        };

        jobConditionForFishing = () => World.GetInstance().IsJobEmpty(World.JobType.CatchFishingInPond) == false;
        workForFishing = () => {
            // 물고기 캐러 가자
            JobInfo job = World.GetInstance().GetFirstJob(World.JobType.CatchFishingInPond);
            if( job != null )
                owner.ChangeState("Fishing", job.targetBuilding);
        };

        jobConditionForUserCommand = () => ( World.GetInstance().FindEggAtGround() != null );
            //&&( World.GetInstance().IsJobEmpty(World.JobType.CarryOnEggToHatchery) == false
            //|| World.GetInstance().IsJobEmpty(World.JobType.CarryOnEggToMainStorage) == false ));
        workForUserCommand = () =>
        {
            Egg eggOnGround = World.GetInstance().FindEggAtGround();
            if( World.GetInstance().IsJobEmpty(World.JobType.CarryOnEggToHatchery) == false )
            {
                // job은 꺼내지 않는다(계속 유지)
                // 알옮기자, 부화장으로
                owner.ChangeState("Carry", new Dictionary<string, ObjectBase>() {
                        { "target", eggOnGround },
                        { "targetBuilding", World.GetInstance().FindEnterablePocketBuilding(owner, World.BuildingType.hatchery) } });
            }
            else if( World.GetInstance().IsJobEmpty(World.JobType.CarryOnEggToMainStorage) == false )
            {
                // 알옮기자, 저장고로
                owner.ChangeState("Carry", new Dictionary<string, ObjectBase>() {
                        { "target", eggOnGround },
                        { "targetBuilding", World.GetInstance().FindMainStorage() }});
            }
            return;
        };

        surviveCondition = () => owner.Hunger >= 60 || owner.Fatigue >= 60;
        changeStateToSurvive = () => {
            if( owner.Hunger >= owner.Fatigue )
                owner.ChangeState("Eat");
            else
                owner.ChangeState("Sleep");
        };

        //우선순위 리스트에 다 때려넣어보자
        //0순위는 부동
        owner.priorityLists.Add(new KeyValuePair<string, KeyValuePair<Func<bool>, Action>>("CarrySomethingStopped", new KeyValuePair<Func<bool>, Action>(jobConditionForLeftWork, workForSomethingToLeft)));
        owner.priorityLists.Add(new KeyValuePair<string, KeyValuePair<Func<bool>, Action>>("낚시", new KeyValuePair<Func<bool>, Action>(jobConditionForFishing, workForFishing)));
        owner.priorityLists.Add(new KeyValuePair<string, KeyValuePair<Func<bool>, Action>>("유저의 명령", new KeyValuePair<Func<bool>, Action>(jobConditionForUserCommand, workForUserCommand)));
        owner.priorityLists.Add(new KeyValuePair<string, KeyValuePair<Func<bool>, Action>>("생존", new KeyValuePair<Func<bool>, Action>(surviveCondition, changeStateToSurvive)));
        owner.priorityLists.Add(new KeyValuePair<string, KeyValuePair<Func<bool>, Action>>("번식", new KeyValuePair<Func<bool>, Action>(changeStateToMatingCondition, changeStateToMating)));
    }

    public override void Enter( object extraData = null )
    {
        base.Enter(extraData);

        owner.GetComponent<NavMeshAgent>().autoBraking = false;
        WanderRandomPosition();

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
            //우선순위를 차례로 검사해서 실행한다
            foreach( var priorityList in owner.priorityLists )
            {
                if( priorityList.Value.Key() )
                {
                    priorityList.Value.Value();
                    break;
                }
            }

            // 우선순위를 다 통과해서 할 일이 없으면 배회한다
            // 근데 결국은 다시 무한발정 이잖아??
            if( ownerAgent.remainingDistance < 0.5f )
            {
                WanderRandomPosition();
            }
        }
    }

    void WanderRandomPosition()
    {
        float radius = 3f;
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
        randomDirection += owner.transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if( NavMesh.SamplePosition(randomDirection, out hit, radius, 1) )
        {
            finalPosition = hit.position;
        }

        owner.Move(finalPosition);
    }
}
