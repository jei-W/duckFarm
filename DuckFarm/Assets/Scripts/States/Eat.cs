using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eat : State
{
    Food targetFood;
    BuildingBase restaurant;
    string currentState = "";
     
    public Eat( Duck duck ) : base(duck) { }

    public override void Enter()
    {
        Debug.Log("배고파!");

        //1순위- 사료공장, 2순위- 저장소, 3순위- 완료된 작물, 4순위- 물고기and지렁이
        restaurant = World.GetInstance().FindCloseBuilding(owner, World.BuildingType.feedFactory);
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        ChangeEatingState("goingToRestaurant");

        if( owner.Hunger <= 30 )
        {
            owner.ChangeState("Idle");
        }
    }

    public override void FixedUpdate()
    {
        if( currentState == "goingToRestaurant" )
        {
            if( restaurant != null && ownerAgent.remainingDistance < 0.2f )
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

                if( restaurant == null ) //저장소에 식량이 없는경우의 조건도 추가하자
                {
                    restaurant = World.GetInstance().FindMainStorage();
                }
                owner.Move(restaurant.transform.position);
                break;
            case "EatingAtRestaurant":
                if( restaurant is MainStorage )
                    targetFood = restaurant.GetComponent<MainStorage>().GetFood();
                owner.EatFood(targetFood);
                break;
            case "EatingSomething":
                break;
            default:
                Debug.Log("state가 잘못됐어!");
                break;

        }
    }
}
