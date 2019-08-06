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
    float hunger = 0;
    float fatigue = 0;
    public long SomethingStartTime { get; set; }
    public float Hunger { get { return hunger; } set { hunger = value; } }
    public float Fatigue { get { return fatigue; } set { fatigue = value; } }
    public float Stress { get; set; } = 0;
    public State currentState;
    NavMeshAgent agent;

    public Dictionary<string, State> stateList = new Dictionary<string, State>();

    private void Start()
    {
        stateList.Add("Idle", new Idle(this));
        stateList.Add("Eat", new Eat(this));
        stateList.Add("Sleep", new Sleep(this));
        stateList.Add("Mating", new Mating(this));
        currentState = stateList["Idle"];
        lifespan = Random.Range(1825f, 3650f); //수명은 5년~10년 사이

        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        IncreaseTargetValue(ref hunger, currentState.HungerPace);
        IncreaseTargetValue(ref fatigue, currentState.FatiguePace);

        //오리 나이를 증가시키자

        currentState.Update();

        if( Hunger > 100 || fatigue > 10 )
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

    public int IncreaseTargetValue( ref float figure, float expirationTime )
    {
        if( figure > 10 )
        {
            return 10;
        }

        figure += Time.deltaTime * expirationTime;
        return (int)figure;
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
        hunger += food.Fullness;
        if( hunger >= 100 )
            hunger = 100;

        World.GetInstance().DuckAteFood(food);
    }

    public void Move(Vector3 destination)
    {
        //네비게이션 이용하자
        agent.SetDestination(destination);
    }

    public void Sleeping()
    {

    }
}
