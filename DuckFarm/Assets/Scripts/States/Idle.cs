using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Idle : State
{
    Vector3 randomPosition;

    Func<bool> changeStateToMatingCondition;
    Func<bool> jobConditionForFishing;
    Func<bool> jobConditionForCarryEggToHatchery;
    Func<bool> jobConditionForCarryEggToMainStorage;
    Func<bool> changeStateToEatCondition;
    Func<bool> changeStateToSleepCondition;
    Func<bool> jobConditionForLeftWork;

    Action changeStateToMating;
    Action workForFishing;
    Action workForCarryEggToHatchery;
    Action workForCarryEggToMainStorage;
    Action changeStateToEat;
    Action changeStateToSleep;
    Action workForSomethingToLeft;

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

        jobConditionForCarryEggToHatchery = () => World.GetInstance().IsJobEmpty(World.JobType.CarryOnEggToHatchery) == false;
        workForCarryEggToHatchery = () => {
            // 알옮기자, 부화장으로
            JobInfo job = World.GetInstance().GetFirstJob(World.JobType.CarryOnEggToHatchery);
            if( job != null )
            {
                owner.ChangeState("Carry", new Dictionary<string, ObjectBase>() {
                        { "target", job.targetObject },
                        { "targetBuilding", World.GetInstance().FindEnterablePocketBuilding(owner, World.BuildingType.hatchery) }
                });
            }
        };

        jobConditionForCarryEggToMainStorage = () => World.GetInstance().IsJobEmpty(World.JobType.CarryOnEggToMainStorage) == false;
        workForCarryEggToMainStorage = () => {
            // 알옮기자, 저장고로
            JobInfo job = World.GetInstance().GetFirstJob(World.JobType.CarryOnEggToMainStorage);
            if( job != null )
            {
                owner.ChangeState("Carry", new Dictionary<string, ObjectBase>() {
                        { "target", job.targetObject },
                        { "targetBuilding", World.GetInstance().FindMainStorage() }
                });
            }
        };

        changeStateToEatCondition = () => owner.Hunger >= 60;
        changeStateToEat = () => owner.ChangeState("Eat");

        changeStateToSleepCondition = () => owner.Fatigue >= 70;
        changeStateToSleep = () => owner.ChangeState("Sleep");

        //우선순위 리스트에 다 때려넣어보자
        owner.priorityLists[0] = new Dictionary<string,KeyValuePair<Func<bool>, Action>> {
            { "CarrySomethingStopped", new KeyValuePair<Func<bool>,Action>(jobConditionForLeftWork, workForSomethingToLeft) },
            { "ChangeStateToFishing", new KeyValuePair<Func<bool>,Action>(jobConditionForFishing, workForFishing) },
            { "CarryEggToHatchery", new KeyValuePair<Func<bool>,Action>(jobConditionForCarryEggToHatchery, workForCarryEggToHatchery) },
            { "CarryEggToMainStorage", new KeyValuePair<Func<bool>,Action>(jobConditionForCarryEggToMainStorage, workForCarryEggToMainStorage) },
        };
        owner.priorityLists[1] = new Dictionary<string, KeyValuePair<Func<bool>, Action>> {
            { "ChangeStateToEat", new KeyValuePair<Func<bool>,Action>(changeStateToEatCondition, changeStateToEat) },
            { "ChangeStateToSleep", new KeyValuePair<Func<bool>,Action>(changeStateToSleepCondition, changeStateToSleep) }
        };
        owner.priorityLists[2] = new Dictionary<string, KeyValuePair<Func<bool>, Action>> {
            { "ChangeStateToMating", new KeyValuePair<Func<bool>,Action>(changeStateToMatingCondition, changeStateToMating) }
        };
    }

    public override void Enter( object extraData = null )
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
            //우선순위를 차례로 검사해서 실행한다
            foreach( var priorityList in owner.priorityLists )
            {
                foreach( var somethingAction in priorityList )
                {
                    if( somethingAction.Value.Key() )
                    {
                        somethingAction.Value.Value();
                        return;
                    }
                }
            }

            // 우선순위를 다 통과해서 할 일이 없으면 배회한다
            // 근데 결국은 다시 무한발정 이잖아??
            if( ownerAgent.remainingDistance < 0.5f )
            {
                randomPosition = RandomPosition();
                owner.Move(randomPosition);
            }
        }
    }

    Vector3 RandomPosition()
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
        return finalPosition;
    }
}
