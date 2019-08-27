using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eat : State
{
    BuildingBase targetBuilding;
    Food targetFood;
    IFoodConsumeableBuilding restaurant;
    string currentState = "";
    float recognitionDistance = 1f;


    public Eat( Duck duck ) : base(duck) { }

    public override void Enter( object extraData = null )
    {
        Debug.Log("배고파!");

        //1순위- 사료공장, 2순위- 저장소, 3순위- 완료된 작물, 4순위- 물고기and지렁이
        targetBuilding = World.GetInstance().FindFoodLeftRestaurant(owner);
        restaurant = targetBuilding as IFoodConsumeableBuilding;
        if( targetBuilding != null )
            recognitionDistance = targetBuilding.recognitionDistance;
        ChangeEatingState("goingToRestaurant");
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        if( owner.Hunger <= 30 )
        {
            owner.ChangeState("Idle");
        }

        //상태 분기
        switch( currentState )
        {
            case "goingToRestaurant":
                if( restaurant.FoodIsEmpty() )
                {
                    if( targetBuilding is MainStorage )
                        //사료공장도 비었고, 저장소도 비었다
                        //작물이나 물고기를 먹자
                        ChangeEatingState("EatingSomething");
                    else
                    {
                        //가려던 사료공장이 중간에 비었다.
                        //다른 사료공장을 찾아보자
                        targetBuilding = World.GetInstance().FindFoodLeftRestaurant(owner);
                        restaurant = targetBuilding as IFoodConsumeableBuilding;
                        if( targetBuilding != null )
                            recognitionDistance = targetBuilding.recognitionDistance;
                        owner.Move(targetBuilding.transform.position);
                    }
                }
                break;
            default:
                Debug.Log("state가 잘못됐어!");
                break;

        }
    }

    public override void FixedUpdate()
    {
        //도착했는지 거리 확인(물리)는 여기서..
        if( currentState == "goingToRestaurant" )
        {
            if( ownerAgent.remainingDistance < recognitionDistance )
            {
                //밥에 도착했따, 
                Debug.Log($"{owner.ObjectID} 밥.. 도착..");
                ChangeEatingState("EatingAtRestaurant");
            }
        }
    }

    void ChangeEatingState( string state )
    {
        if( currentState == state )
            return;

        currentState = state;

        switch(state)
        {
            case "goingToRestaurant":
                owner.Move(targetBuilding.transform.position);
                break;
            case "EatingAtRestaurant":
                Debug.Log("냠냠");
                targetFood = restaurant.GetFood();
                owner.EatFood(targetFood);
                owner.ChangeState("Idle");
                break;
            case "EatingSomething":
                owner.ChangeState("Fishing");
                break;
            default:
                Debug.Log("state가 잘못됐어!");
                break;

        }
    }
}
