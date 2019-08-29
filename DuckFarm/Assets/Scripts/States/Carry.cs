using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carry : State
{
    bool isWorkOver;

    BuildingBase targetBuilding = null;
    ObjectBase targetObject = null;

    string currentState = "";
    public Carry( Duck duck ) : base(duck)
    {

    }

    public override void Enter( object extraData = null )
    {
        var data = extraData as Dictionary<string, ObjectBase>;
        if( data == null )
        {
            Debug.LogError("Carry는 Enter로 운반할 target과 targetBuilding을 Dictionary<string, ObjectBase> 형태로 넣어주어야 한다");
            return;
        }
        targetObject = data["target"];
        targetBuilding = data["targetBuilding"] as BuildingBase;

        //이미 타겟오브젝트를 들고 있는 상태라면
        if( targetObject.transform.parent == owner )
        {
            owner.Move(targetBuilding.transform.position);
            currentState = "goToDestinate";
        }
        else
        {
            owner.Move(targetObject.transform.position);
            currentState = "goToObject";
        }

        isWorkOver = false;
    }

    public override void Exit()
    {
        // 물건을 옮기는 중에 나간다면
        if( isWorkOver == false )
        {
            //옮기던 물건을 바닥에 떨군다
            Debug.Log("긴급탈출! 못옮겨!");
            targetObject.transform.parent = null;
            targetObject.gameObject.active = true;

            //떨군거 옮기는 일을 다시 등록한다
            World.GetInstance().RequestCarrySomethingStopped(targetObject, targetBuilding);
        }
    }

    public override void Update()
    {
        base.Update();

        //집어넣는게 불가능한 상태면 대기상태로 돌아간다
        if( currentState == "goToDestinate" )
        {
            if( targetBuilding is PocketBuilding )
            {
                if( ( targetBuilding as PocketBuilding ).AskEnterable() == false )
                {
                    StopCarrySomthing();
                    return;
                }
            }
            else if( targetBuilding is IFoodConsumeableBuilding )
            {
                if( ( targetBuilding as IFoodConsumeableBuilding ).FoodIsFull() )
                {
                    StopCarrySomthing();
                    return;
                }
            }
            else if( targetBuilding is IResourceConsumeableBuilding )
            {
                if( ( targetBuilding as IResourceConsumeableBuilding ).ResourceIsFull() )
                {
                    StopCarrySomthing();
                    return;
                }
            }

            if( ownerAgent.remainingDistance < targetBuilding.recognitionDistance )
            {
                bool enableEnter = true;

                if( targetBuilding is PocketBuilding )
                {
                    // 여튼 뭔가 들어가는거
                    ( targetBuilding as PocketBuilding ).EnterObject(targetObject);
                }
                else if( targetBuilding is IFoodConsumeableBuilding )
                {
                    // 음식 형태
                    enableEnter = ( targetBuilding as IFoodConsumeableBuilding ).InputFood(targetObject as Food);
                }
                else if( targetBuilding is IResourceConsumeableBuilding )
                {
                    // 리소스 형태
                    enableEnter = ( targetBuilding as IResourceConsumeableBuilding ).InputResource(targetObject as Resource);
                }

                if( enableEnter == false ) //집어넣을 수 없을 때
                    StopCarrySomthing();
                else
                    owner.ChangeState("Idle");

                return;
            }
        }

        if( currentState == "goToObject" && ownerAgent.remainingDistance < targetObject.recognitionDistance )
        {
            owner.Move(targetBuilding.transform.position);
            targetObject.transform.parent = owner.transform;
            targetObject.gameObject.active = false;
            targetObject.transform.localPosition = Vector3.zero;

            currentState = "goToDestinate";
        }
    }

    //옮기기 중지
    void StopCarrySomthing()
    {
        isWorkOver = true;

        Debug.Log("안옮겨! 못옮겨!");
        targetObject.transform.parent = null;
        targetObject.gameObject.active = true;
        owner.ChangeState("Idle");
    }
}
