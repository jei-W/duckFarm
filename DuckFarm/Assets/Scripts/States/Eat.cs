using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eat : State
{
    Food targetFood;
    IFoodConsumeableBuilding restaurant;
    string currentState = "";
    float recognitionDistance = 1f;


    public Eat( Duck duck ) : base(duck) { }

    public override void Enter( object extraData = null )
    {
        Debug.Log("배고파!");

        //1순위- 사료공장, 2순위- 저장소, 3순위- 완료된 작물, 4순위- 물고기and지렁이
        var building = World.GetInstance().FindCloseBuilding(owner, World.BuildingType.feedFactory);
        restaurant = building as IFoodConsumeableBuilding;
        if( building != null )
            recognitionDistance = building.recognitionDistance;
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
    }

    public override void FixedUpdate()
    {
        if( currentState == "goingToRestaurant" )
        {
            if( restaurant != null && ownerAgent.remainingDistance < recognitionDistance )
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
                //if(restaurant is FeedFactory && restaurant.IsEmpty )
                //레스토랑이 사료공장인데, 사료가 없으면 다른 사료공장을 찾는다
                Vector3 destination = Vector3.zero;
                if( restaurant == null || restaurant.FoodIsEmpty() )
                {
                    var building = World.GetInstance().FindMainStorage();
                    destination = building.transform.position;
                    restaurant = building as IFoodConsumeableBuilding;
                    if( building != null )
                        recognitionDistance = building.recognitionDistance;
                }
                owner.Move(destination);
                break;
            case "EatingAtRestaurant":
                Debug.Log("냠냠");
                targetFood = restaurant.GetFood();
                owner.EatFood(targetFood);
                owner.ChangeState("Idle");
                break;
            case "EatingSomething":
                break;
            default:
                Debug.Log("state가 잘못됐어!");
                break;

        }
    }
}
