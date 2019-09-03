using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 연못 
public class Pond : BuildingBase, IFoodConsumeableBuilding
{
    // 연못은 최대 물고기 생산량이 있다
    //일정시간마다 자동으로 물고기가 생긴다
    int MaximumFoodCapacity = 30;
    int CurrentFoodCapacity = 0;

    // Start is called before the first frame update
    void Start()
    {
        // 아직 연못을 생성하는게 없으므로..
        // 마우스로 직접 pond를 생성할 건데, start에서 world에 job을 만들게 하자
        // 오리가 있든, 없든, 대기상태의 오리가 있든 없든 일단 큐에 담아두겟지.
        recognitionDistance = 0.5f;

        AutoMakingFood();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool FoodIsFull()
    {
        if( CurrentFoodCapacity >= MaximumFoodCapacity )
            return true;
        else
            return false;
    }

    public bool FoodIsEmpty()
    {
        if( CurrentFoodCapacity == 0 )
            return true;
        else
            return false;
    }

    public bool InputFood( Food targetObject )
    {
        return false;
    }

    public Food GetFood()
    {
        if( FoodIsEmpty() )
            return null;

        // 즉석에서 물고기 생성하여 반환
        var food = World.GetInstance().ProduceFood(World.FoodType.fish);
        //반환되는 물고기는 신선하다
        food.ResetFreshness();
        --CurrentFoodCapacity;
        return food;
    }

    public void BecameRottenFood( Food targetFood )
    {
        //상한 물고기가 생기면 신선도를 초기화 한다
        targetFood.ResetFreshness();
    }

    long _timerID = 0;

    void AutoMakingFood()
    {
        if( !FoodIsFull() )
        {
            Debug.Log("물고기 생김");
            ++CurrentFoodCapacity;
        }

        //일정시간마다 물고기 잡는 일이 없으면 등록한다
        if( World.GetInstance().IsJobEmpty(World.JobType.CatchFishingInPond) )
        {
            Debug.Log($"{this.ObjectID} 물고기 잡자");
            World.GetInstance().RequestCatchFish(this);
        }
        _timerID = WorldTimer.GetInstance().RegisterTimer(World.CurrentGameWorldTimeMS + World.oneDay, TimerCallback);
    }
    void TimerCallback( long timerID )
    {
        if( _timerID == timerID )
        {
            _timerID = 0;
            AutoMakingFood();
        }
    }
}
