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
    float lifespan;
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

        stateList.Add("Idle", new Idle(this));
        stateList.Add("Eat", new Eat(this));
        stateList.Add("Sleep", new Sleep(this));
        stateList.Add("Mating", new Mating(this));
        stateList.Add("Fishing", new Fishing(this));
        stateList.Add("Carry", new Carry(this));

        ChangeState("Idle");

        lifespan = Random.Range(World.oneDay * 100.0f, World.oneDay * 180.0f); //수명은 100일 ~ 180일 사이

        agent = GetComponent<NavMeshAgent>();

        //암컷오리는 발정확률이 숫컷보다 낮다
        if( male == false )
            originalHeat = 40f;
        CurrentHeat = originalHeat;
    }

    private void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }

    // 너무 로그가 많이 떠서...
    long tempLastTime = 0;
    private void Update()
    {
        // 오리를 회전 시킨다.
        // 0.0f 보다.. Epsilon으로 비교하여 실수 오차로 인한 에러를 방지 한다...
        if ( agent.velocity.sqrMagnitude > Mathf.Epsilon )
            transform.rotation = Quaternion.LookRotation(agent.velocity.normalized);

        //오리 나이를 증가시키자
        age += Time.deltaTime * World.oneDay;

        if ( World.CurrentGameWorldTimeMS/1000 != tempLastTime )
        {
            Debug.Log($"{ObjectID} {currentState.ToString()} 배고픔:{Hunger} / 피곤:{Fatigue}");
            tempLastTime = World.CurrentGameWorldTimeMS / 1000;
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

    public void Sleeping(string state)
    {
        Debug.Log($"{ObjectID} :zz..");
        if( !agent.isStopped )
            agent.isStopped = true;

        switch( state )
        {
            case "sleepGround":
                currentState.hungerChangeValue = 10f;
                currentState.fatigueChangeValue = -25f;
                break;
            case "sleepShelter":
                currentState.hungerChangeValue = 0f;
                currentState.fatigueChangeValue = -40f;
                break;
        }
    }
}
