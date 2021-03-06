﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Duck : ObjectBase
{
    enum GrowthLevel { duckling, adult }
    enum WorkState { idle, sleep, eat, mating, work }
    public bool male;
    float age = 0;   //단위 = 1일
    long lifespan;
    long birthTime = 0;
    float originalHeat = 60f; //기초발정확률

    public float CurrentHeat { get; private set; }
    public long LastMatingTime = 0;
    public long SomethingStartTime { get; set; }
    public float Hunger { get; set; } = 0;
    public float Fatigue { get; set; } = 0;
    public float Stress { get; set; } = 0;
    State currentState;
    NavMeshAgent agent;

    public string GetCurrentStateName()
    {
        return currentState.ToString();
    }
    public State GetCurrentState()
    {
        return currentState;
    }

    private Dictionary<string, State> stateList = new Dictionary<string, State>();

    private void Start()
    {
        recognitionDistance = 0.8f;
        LastMatingTime = World.CurrentGameWorldTimeMS;

        stateList.Add("Idle", new Idle(this));
        stateList.Add("Eat", new Eat(this));
        stateList.Add("Sleep", new Sleep(this));
        stateList.Add("Mating", new Mating(this));
        stateList.Add("Fishing", new Fishing(this));
        stateList.Add("Carry", new Carry(this));

        lifespan = UnityEngine.Random.Range(World.oneDay * 100, World.oneDay * 180); //수명은 100일 ~ 180일 사이
        birthTime = World.CurrentGameWorldTimeMS;
        agent = GetComponent<NavMeshAgent>();

        //암컷오리는 발정확률이 숫컷보다 낮다
        if( male == false )
            originalHeat = 40f;
        CurrentHeat = originalHeat;

        ChangeState("Idle");
    }

    private void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }

    private void Update()
    {
        // 오리를 회전 시킨다.
        // 0.0f 보다.. Epsilon으로 비교하여 실수 오차로 인한 에러를 방지 한다...
        if ( agent.velocity.sqrMagnitude > Mathf.Epsilon )
            transform.rotation = Quaternion.LookRotation(agent.velocity.normalized);

        //오리 나이를 증가시키자
        age = (( World.CurrentGameWorldTimeMS - birthTime ) / World.oneDay ) + 1;

        //긴급 생존 조건
        if( Hunger >= 90 )
        {
            if( Hunger > Fatigue )
                ChangeState("Eat");
        }
        if( Fatigue >= 90 )
        {
            ChangeState("Sleep");
        }

        currentState?.Update();

        if( Hunger > 100 || Fatigue > 100 )
        {
            DuckDie();
            return;
        }

        if( age > lifespan )
        {
            DuckDie();
            return;
        }
    }

    void DuckDie()
    {
        //혹시 뭐 옮기고 있었다면 떨구자
        if( currentState == stateList["Carry"] )
        {
            currentState.Exit();
        }

        Debug.Log($"{ObjectID} : 꽥!");
        World.GetInstance().OnDuckDied(this);
        GameObject.Destroy(this.gameObject);
    }

    public float ChangeTargetValue( float figure, float expirationTime )
    {
        if( figure >= 100 )
            return figure;
        else if( figure < 0 )
            return 0f;

        figure += Time.deltaTime * expirationTime * World.reverseOneDay;
        return figure;
    }

    public void ResetCurrentHeat()
    {
        CurrentHeat = originalHeat;
    }

    public void ChangeState(string stateName, object extraData = null)
    {
        if ( stateList.ContainsKey(stateName) )
        {
            ChangeState(stateList[stateName], extraData);
        }
        else
        {
            Debug.LogError($"존재하지 않는 오리의 상태 : {stateName}");
        }
    }

    public void ChangeState(State state, object extraData = null )
    {
        if( currentState == state )
            return;

        Debug.Log($"{gameObject.name}의 상태 변경 : {state.ToString()}");
        //현재 스테이트의 Exit를 호출
        if( currentState != null )
            currentState.Exit();

        //바꾸려는 스테이트의 Enter 호출
        state.Enter(extraData);

        //현재 스테이트를 바꾸려는 스테이트로 변경
        currentState = state;
    }

    public void EatFood( Food food )
    {
        if( food == null )
            return;

        Hunger -= food.Fullness;
        if( Hunger <= 0 )
            Hunger = 0;

        World.GetInstance().DuckAteFood(food);
    }

    public void Move(Vector3 destination)
    {
        //네비게이션 이용하자
        agent.isStopped = true;
        agent.SetDestination(destination);
        agent.isStopped = false;
    }

    //오리의 행동 우선순위
    #region Priority
    //우선순위는 3순위까지 있음
    public List<KeyValuePair<string, KeyValuePair<Func<bool>, Action>>> priorityLists = new List<KeyValuePair<string, KeyValuePair<Func<bool>, Action>>>();

    public void ChangePriorityRanking(string targetKey, int priorityRank)
    {
        if( priorityRank > priorityLists.Count - 1 || priorityRank < 1 )
        {
            Debug.Log("바꿀 수 있는 우선순위밖임");
            return;
        }

        KeyValuePair<string, KeyValuePair<Func<bool>, Action>> target;
        int currentRank = -1;

        for( int i = 1; i < priorityLists.Count; i++ )
        {
            if( priorityLists[i].Key == targetKey )
            {
                target = priorityLists[i];
                currentRank = i;
                break;
            }
        }

        if( currentRank == -1 )
        {
            Debug.Log("타겟을 찾을 수 없었음");
            return;
        }
        else if( currentRank == priorityRank )
        {
            Debug.Log("우선순위 변동없음");
            return;
        }
        //우선순위가 변하면 하나씩 밀고 지정한 자리에 넣는다
        else if( currentRank > priorityRank )
        {
            for( int i = currentRank; i > priorityRank; i-- )
            {
                priorityLists[i] = priorityLists[i - 1];
            }
            priorityLists[priorityRank] = target;
        }
        else if( currentRank < priorityRank )
        {
            for( int i = currentRank; i < priorityRank; i++ )
            {
                priorityLists[i] = priorityLists[i + 1];
            }
            priorityLists[priorityRank] = target;
        }
        Debug.Log($"{targetKey} : {currentRank}순위 -> {priorityRank}순위");
    }

    public void SwapPriorityRanking( string key1, string key2 )
    {
        bool isFindIndex = false;
        int key1Index = -1;
        int key2Index = -1;

        //각각의 인덱스를 찾는다
        for( int i = 1; i < priorityLists.Count; i++ )
        {
            if( priorityLists[i].Key == key1 )
            {
                key1Index = i;

                if( isFindIndex )
                    break;
                else
                    isFindIndex = true;
            }
            if( priorityLists[i].Key == key2 )
            {
                key2Index = i;

                if( isFindIndex )
                    break;
                else
                    isFindIndex = true;
            }
        }

        //끝내 인덱스를 찾지 못했다면 리턴
        if( key1Index < 0 || key2Index < 0 )
            return;

        var temp = priorityLists[key1Index];
        priorityLists[key1Index] = priorityLists[key2Index];
        priorityLists[key2Index] = temp;

        Debug.Log($"{key1}:{key1Index}순위 <-> {key2}:{key2Index}순위");
    }
    #endregion
}
