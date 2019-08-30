using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 피.싱. 오리가 연못에서 물고기를 잡다 ㅎㅎㅎ..
public class Fishing : State
{
    bool isWorkOver;

    Pond targetPond = null;
    Food currentFish = null;

    string currentState = "goToPond";
    public Fishing( Duck duck ) : base(duck)
    {

    }

    public override void Enter( object extraData = null )
    {
        base.Enter(extraData);

        // extraData에는 연못(buildingBase가 들어있을 것이다)
        targetPond = extraData as Pond;
        if ( targetPond == null )
        {
            Debug.LogError("Fishing은 Enter로 낚시할 연못의 buildingBase를 전달 해야한다.");
            return;
        }

        currentState = "goToPond"; // pond로 걸어가자.
        owner.Move(targetPond.transform.position);

        isWorkOver = false;
    }

    public override void Exit()
    {
        //물고기를 잡으러 가는 중에 끝났으면 다시 물고기 잡는 일을 등록한다
        if( isWorkOver ==false)
            World.GetInstance().RequestCatchFish(targetPond);
    }

    public override void Update()
    {
        base.Update();

        if ( targetPond == null )
        {
            // Enter로 pond가 전달이 안되었다.
            // Idle로 변경시킨다
            owner.ChangeState("Idle");
            return;
        }

        if ( currentState == "goToPond" )
        {
            if ( ownerAgent.remainingDistance < targetPond.recognitionDistance )
            {
                currentState = "getFish";
                return;
            }
        }
        else if ( currentState == "getFish" )
        {
            if ( targetPond.FoodIsEmpty() )
            {
                // 연못에 물고기가 없다!
                // 상태를 바꿀것인가.. 적어도 몇초동안은 기다려 볼 것인가?
                // 지렁이 잡으러 가기엔 지금 너무 귀찮당! 힘들다 오리가. 오리 다리 아프다. 느리다.
                // 일단은 물고기가 생길때 까지 기다린다.
            }
            else
            {
                // 약간의 딜레이 시간을 준다.. ..귀찮으니깐 바로 캐가도록 하자.

                currentFish = targetPond.GetFood();

                Debug.Log($"{owner.ObjectID} 물고기를 얻었다 : {currentFish.ObjectID}");
                // 물고기를 머리에 이고 가자.
                currentFish.transform.parent = owner.transform;
                currentFish.transform.localPosition = Vector3.zero;
                currentFish.gameObject.active = false;
                currentState = "goToStorage";

                isWorkOver = true;
            }
        }
        else if( currentState == "goToStorage" )
        {
            // 잡은 물고기를 창고로 옮기자
            owner.ChangeState("Carry", new Dictionary<string, ObjectBase>() {
                        { "target", currentFish },
                        { "targetBuilding", World.GetInstance().FindMainStorage() }
                    });
            return;
        }
    }
}
