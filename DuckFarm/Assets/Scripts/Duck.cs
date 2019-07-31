using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duck : ObjectBase
{
    enum GrowthLevel { duckling, adult }
    enum WorkState { idle, sleep, work }
    bool male;
    float age = 0;
    float hunger = 0;
    float fatigue = 0;
    public float Hunger { get { return hunger; } set { hunger = value; } }
    public float Fatigue { get { return fatigue; } set { fatigue = value; } }
    public float Stress { get; set; } = 0;
    public State currentState;

    public Dictionary<string, State> stateList = new Dictionary<string, State>();

    private void Start()
    {
        stateList.Add("Idle", new Idle(this));
        stateList.Add("Eat", new Eat(this));
        stateList.Add("Sleep", new Sleep(this));
        currentState = stateList["Idle"];
    }

    private void Update()
    {
        IncreaseTargetValue(ref hunger, currentState.HungerPace);
        IncreaseTargetValue(ref fatigue, currentState.FatiguePace);
        IncreaseTargetValue(ref age, 0.5f);

        currentState.Update();

        if( Hunger > 100 || fatigue > 10 )
        {
            DuckDie();
            return;
        }
    }

    //void 건물에 들어갈 수 있는지 물어본다

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
}
