using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Duck : ObjectBase
{
    enum GrowthLevel { duckling, adult }
    enum WorkState { idle, sleep, eat, mating, work }
    bool male;
    float age = 0;   //단위 = 1일
    float lifespan;

    public long SomethingStartTime { get; set; }
    public float Hunger { get; set; } = 0;
    public float Fatigue { get; set; } = 0;
    public float Stress { get; set; } = 0;
    public State currentState;
    NavMeshAgent agent;

    private Dictionary<string, State> stateList = new Dictionary<string, State>();

    private void Start()
    {
        stateList.Add("Idle", new Idle(this));
        stateList.Add("Eat", new Eat(this));
        stateList.Add("Sleep", new Sleep(this));
        stateList.Add("Mating", new Mating(this));

        ChangeState("Idle");

        lifespan = Random.Range(1825f, 3650f); //수명은 5년~10년 사이

        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //오리 나이를 증가시키자

        currentState.Update();

        if( Hunger > 100 || Fatigue > 10 )
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
        World.GetInstance().OnDuckDied(this.gameObject);
        GameObject.Destroy(this);
    }

    public float ChangeTargetValue( float figure, float expirationTime )
    {
        if( figure >= 10 )
            return 10f;
        else if( figure <= 0 )
            return 0f;

        figure += Time.deltaTime * expirationTime * World.reverseOneDay;
        return figure;
    }

    public void ChangeState(string stateName)
    {
        if ( stateList.ContainsKey(stateName) )
        {
            ChangeState(stateList[stateName]);
        }

        Debug.LogError($"존재하지 않는 오리의 상태 : {stateName}");
    }

    public void ChangeState(State state)
    {
        if( currentState == state )
            return;

        //현재 스테이트의 Exit를 호출
        if( currentState != null )
            currentState.Exit();
        //바꾸려는 스테이트의 Enter 호출
        state.Enter();
        //현재 스테이트를 바꾸려는 스테이트로 변경
        currentState = state;
    }

    public void EatFood( Food food )
    {
        Hunger += food.Fullness;
        if( Hunger >= 100 )
            Hunger = 100;

        World.GetInstance().DuckAteFood(food);
    }

    public void Move(Vector3 destination)
    {
        //네비게이션 이용하자
        agent.SetDestination(destination);
    }

    public void Sleeping(string state)
    {
        Debug.Log("zz..");

        switch( state )
        {
            case "sleepGround":
                currentState.hungerChangeValue = 30f;
                currentState.fatigueChangeValue = -25f;
                break;
            case "sleepShelter":
                currentState.hungerChangeValue = 0f;
                currentState.fatigueChangeValue = -40f;
                break;
        }
    }
}
