using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainStorage : BuildingBase
{
    //food 최대 저장량, 현재 저장량
    int MaximumFoodCapacity = 5;
    int CurrentFoodCapacity = 0;
    //자원 최대 저장량, 현재 저장량
    int MaximumMatCapacity = 10;
    int CurrentMatCapacity = 0;

    //Food 저장 리스트
    Dictionary<string, Food> foodList = new Dictionary<string, Food>();
    //자원 저장 리스트

    //(bool) is full? 메소드
    public bool FoodIsFull()
    {
        if( MaximumFoodCapacity <= CurrentFoodCapacity )
            return false;
        else return true;
    }

    public bool MaterialIsFull()
    {
        if( MaximumMatCapacity <= CurrentMatCapacity )
            return false;
        else return true;
    }

    public bool FoodIsEmpty()
    {
        if( CurrentFoodCapacity == 0 )
            return true;
        else return false;
    }

    public bool MaterialIsEmpty()
    {
        if( CurrentMatCapacity == 0 )
            return true;
        else return false;
    }

    //저장 메소드
    public void SaveObject(ObjectBase targetObject)
    {
        switch(targetObject)
        {
            case Food food:
                foodList.Add(food.ObjectID, food);
                CurrentFoodCapacity++;
                break;
            //case Material mat:
            //break;
        }
    }

    //꺼내기 메소드
    public Food GetFood()
    {
        float lowestFreshness = 100;
        string foodKey = null;
        Food foodValue = null;

        foreach( var food in foodList )
        {
            float freshness = food.Value.GetFreshness();
            if( freshness <= lowestFreshness )
            {
                lowestFreshness = freshness;
                foodKey = food.Key;
                foodValue = food.Value;
            }
        }

        if( foodKey != null )
            foodList.Remove(foodKey);
        return foodValue;
    }

    //**(임시 테스트용)** food는 일정시간마다 하나씩 자동으로 찬다
    public void AutoMakingFood()
    {
        if( !FoodIsFull() )
        {
            WorldTimer.GetInstance().RegisterTimer()
        }
        foodList.Add(food.ObjectID, food);
        CurrentFoodCapacity++;
    }
}
