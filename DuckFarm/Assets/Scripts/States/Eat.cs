using System;
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
        base.Enter(extraData);

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
        if( owner.Hunger <= 45 )
        {
            owner.ChangeState("Idle");
            return;
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
                        ChangeEatingState("FindSomethingFood");
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
            case "goingToSomethingFood":
                if( targetFood == null || targetFood.transform.parent != null )
                    ChangeEatingState("FindSomethingFood");
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
        else if( currentState == "goingToSomethingFood" )
        {
            if( ownerAgent.remainingDistance < recognitionDistance )
            {
                //밥에 도착했따, 
                Debug.Log($"{owner.ObjectID} 밥.. 도착..");
                ChangeEatingState("EatingSomething");
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
                Debug.Log($"{owner.ObjectID} : 냠냠");
                targetFood = restaurant.GetFood();
                owner.EatFood(targetFood);
                owner.ChangeState("Idle");
                break;
            case "FindSomethingFood":
                Food something = null;

                // 원래라면 밭농작물 - 바닥에 떨어진 푸드 - 낚시(물고기무한정X) - 바닥에 떨어진 알 순서
                Func<Food> foodOnGround = () => World.GetInstance().FindFoodAtGroundNotEgg();
                Func<Food> eggOnGround = () => World.GetInstance().FindEggAtGround();

                List<Func<Food>> somethingFoodFuncList = new List<Func<Food>> { foodOnGround, eggOnGround };
                foreach(var findSomtingFood in somethingFoodFuncList )
                {
                    something = findSomtingFood();

                    if( something != null )
                    {
                        targetFood = something;
                        recognitionDistance = targetFood.recognitionDistance;

                        ChangeEatingState("goingToSomethingFood");
                        return;
                    }
                }
                owner.ChangeState("Fishing", World.GetInstance().FindCloseBuilding(owner, World.BuildingType.pond));
                break;
            case "goingToSomethingFood":
                owner.Move(targetFood.transform.position);
                break;
            case "EatingSomething":
                if( targetFood.transform.parent != null )
                {
                    ChangeEatingState("goingToSomethingFood");
                    return;
                }

                Debug.Log($"{owner.ObjectID} : 냠냠");
                owner.EatFood(targetFood);
                owner.ChangeState("Idle");
                break;
            default:
                Debug.Log("state가 잘못됐어!");
                break;

        }
    }
}
