using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainStorage : BuildingBase, IFoodConsumeableBuilding, IResourceConsumeableBuilding
{
    //food 최대 저장량, 현재 저장량
    int MaximumFoodCapacity = 5;
    int CurrentFoodCapacity = 0;
    //자원 최대 저장량, 현재 저장량
    int MaximumResCapacity = 10;
    int CurrentResCapacity = 0;

    //Food 저장 리스트
    Dictionary<string, Food> foodList = new Dictionary<string, Food>();
    //자원 저장 리스트
    Dictionary<string, Resource> resourceList = new Dictionary<string, Resource>();


    //(bool) is full? 메소드
    public bool FoodIsFull()
    {
        if( MaximumFoodCapacity <= CurrentFoodCapacity )
            return true;
        else return false;
    }

    public bool ResourceIsFull()
    {
        if( MaximumResCapacity <= CurrentResCapacity )
            return false;
        else return true;
    }

    public bool FoodIsEmpty()
    {
        if( CurrentFoodCapacity == 0 )
            return true;
        else return false;
    }

    public bool ResourceIsEmpty()
    {
        if( CurrentResCapacity == 0 )
            return true;
        else return false;
    }

    //저장 메소드
    public void InputFood(Food targetObject)
    {
        foodList.Add(targetObject.ObjectID, targetObject);
        CurrentFoodCapacity++;

        targetObject.transform.parent = this.transform;
        var meshRenderer = targetObject.GetComponentInChildren<MeshRenderer>() as MeshRenderer;
        if( meshRenderer != null )
        {
            meshRenderer.enabled = false;
        }
    }
    public void InputResource( Resource targetObject )
    {
        resourceList.Add(targetObject.ObjectID, targetObject);
        CurrentResCapacity++;

        targetObject.transform.parent = this.transform;
        var meshRenderer = targetObject.GetComponentInChildren<MeshRenderer>() as MeshRenderer;
        if( meshRenderer != null )
        {
            meshRenderer.enabled = false;
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

        foodValue.transform.parent = null;
        var meshRenderer = foodValue.GetComponentInChildren<MeshRenderer>() as MeshRenderer;
        if( meshRenderer != null )
        {
            meshRenderer.enabled = true;
        }
        return foodValue;
    }

    public Resource GetResource( World.ResourceType type )
    {
        string resourceKey = null;
        Resource resourceValue = null;

        foreach( var resource in resourceList )
        {
            if( resource.Value.type == type )
            {
                resourceKey = resource.Key;
                resourceValue = resource.Value;
                break;
            }
        }

        if( resourceKey != null )
            resourceList.Remove(resourceKey);

        resourceValue.transform.parent = null;
        var meshRenderer = resourceValue.GetComponentInChildren<MeshRenderer>() as MeshRenderer;
        if( meshRenderer != null )
        {
            meshRenderer.enabled = true;
        }
        return resourceValue;
    }


    //**(임시 테스트용)** food는 일정시간마다 하나씩 자동으로 찬다
    long _timerID = 0;

    public void Start()
    {
        _timerID = WorldTimer.GetInstance().RegisterTimer(World.CurrentGameWorldTimeMS + 3000, TimerCallback);
    }

    public void AutoMakingFood()
    {
        var food = World.GetInstance().ProduceFood(World.FoodType.feed);
        InputFood(food);
        Debug.Log("밥생깅");

        if( !FoodIsFull() )
        {

            _timerID = WorldTimer.GetInstance().RegisterTimer(World.CurrentGameWorldTimeMS + 3000, TimerCallback);
        }
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
