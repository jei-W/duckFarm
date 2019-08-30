using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedFactory : BuildingBase, IFoodConsumeableBuilding
{
    //재료 요구량
    int requireInputFood = 5;
    int CurrentInputFood = 0;
    //생산량
    int makingOutputFoodAmount = 5;
    Queue<Food> outputFood;

    public FeedFactory()
    {
        outputFood = new Queue<Food>(makingOutputFoodAmount);
    }

    public bool FoodIsEmpty()
    {
        //Feed가 없어?
        return outputFood.Count == 0;
    }

    public bool FoodIsFull()
    {
        //재료가 꽉찼어?
        return CurrentInputFood == requireInputFood;
    }

    public Food GetFood()
    {
        //완성된 먹이를 꺼내먹는 기능?
        return outputFood.Dequeue();
    }

    public bool InputFood( Food targetObject )
    {
        //재료를 집어넣는 기능?
        throw new System.NotImplementedException();
    }

    public void BecameRottenFood( Food targetFood )
    {
        //푸드팩토리에서 음식이 상하지 않는다
        //상한 식량이 생기면 신선도를 초기화 한다
        targetFood.ResetFreshness();
    }
}
