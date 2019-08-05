using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eat : State
{
    Food targetFood;
    BuildingBase restaurant;
    public Eat( Duck duck ) : base(duck) { }

    public override void Enter()
    {
        Debug.Log("배고파!");

        //**(임시)** 메인스토리지의 위치를 받는다
        restaurant = World.GetInstance().FindMainStorage();
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        // 먹을것을 찾아다닌다 
        //메인스토리지에 도착했다
        MainStorage mainStorage = restaurant.GetComponent<MainStorage>();
        if( targetFood != null && mainStorage.FoodIsEmpty() == false )
        {
            targetFood = mainStorage.GetFood();
            owner.EatFood(targetFood);
        }

        if( owner.Hunger <= 30 )
        {
            owner.ChangeState(owner.stateList["Idle"]);
        }
    }
}
