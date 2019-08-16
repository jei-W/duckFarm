using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carry : State
{
    ObjectBase targetBuilding = null;
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
        targetBuilding = data["targetBuilding"];

        owner.Move(targetObject.transform.position);
        currentState = "goToObject";
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
                }
            }
            else if( targetBuilding is IFoodConsumeableBuilding )
            {
                if( ( targetBuilding as IFoodConsumeableBuilding ).FoodIsFull() )
                {
                    StopCarrySomthing();
                }
            }
            else if( targetBuilding is IResourceConsumeableBuilding )
            {
                if( ( targetBuilding as IResourceConsumeableBuilding ).ResourceIsFull() )
                {
                    StopCarrySomthing();
                }
            }
        }

        if ( ownerAgent.remainingDistance < 1.0f )
        {
            if ( currentState == "goToObject")
            {
                owner.Move(targetBuilding.transform.position);
                targetObject.transform.parent = owner.transform;
                targetObject.gameObject.active = false;
                targetObject.transform.localPosition = Vector3.zero; 

                currentState = "goToDestinate";
            }
            else
            {
                // 바부 바보 멍~총이
                if( targetBuilding is PocketBuilding )
                {
                    // 여튼 뭔가 들어가는거
                    ( targetBuilding as PocketBuilding ).EnterObject(targetObject);
                }
                else if( targetBuilding is IFoodConsumeableBuilding )
                {
                    // 음식 형태
                    ( targetBuilding as IFoodConsumeableBuilding ).InputFood(targetObject as Food);
                }
                else if( targetBuilding is IResourceConsumeableBuilding )
                {
                    // 리소스 형태
                    ( targetBuilding as IResourceConsumeableBuilding ).InputResource(targetObject as Resource);
                }

                owner.ChangeState("Idle");
            }
        }
    }

    //옮기기 중지
    void StopCarrySomthing()
    {
        Debug.Log("안옮겨! 못옮겨!");
        targetObject.transform.parent = null;
        targetObject.gameObject.active = true;
        owner.ChangeState("Idle");
    }
}
