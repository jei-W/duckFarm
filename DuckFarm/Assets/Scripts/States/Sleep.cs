using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleep : State
{
    Vector3 destination;
    PocketBuilding closeShelter = null;
    float sleepTime;
    string currentState = "";

    long wakeUpTimer = 0;
    public Sleep( Duck duck ) : base(duck) { }

    public override void Enter( object extraData = null )
    {
        base.Enter(extraData);

        Debug.Log($"{owner.ObjectID} 졸려!");
        closeShelter = World.GetInstance().FindCloseBuilding(owner, World.BuildingType.shelter) as PocketBuilding;

        //들어갈 수 있는 축사를 찾는다
        //들어갈 수 있는 축사가 없으면 현재 위치에서 잠자기를 실행한다
        //노숙은 축사보다 피로도 감소폭이 작으며, 스트레스가 줄어들지 않는다

        if( closeShelter != null )
        {
            ChangeSleeping("goingToShelter");
            destination = closeShelter.transform.position;
            owner.Move(destination);
        }
        else
        {
            ChangeSleeping("sleepGround");
        }

        sleepTime = Random.Range(World.oneDay * 0.25f, World.oneDay * 0.3f);
    }

    public override void Exit()
    {
        ////초기값으로 셋팅
        //hungerChangeValue = 10f;
        //fatigueChangeValue = 5f;
        currentState = "";

        //타이머 등록해제
        if( wakeUpTimer != 0 )
        {
            WorldTimer.GetInstance().UnregisterTimer(wakeUpTimer);
            wakeUpTimer = 0;
        }
    }

    void OnWakeUpCallback(long timerID)
    {
        if ( timerID == wakeUpTimer )
        {
            // 나를 꺼내쥬....
            if( currentState == "sleepShelter" && closeShelter != null )
                closeShelter.ExitObject(owner.ObjectID);

            wakeUpTimer = 0;

            Debug.Log($"{owner.ObjectID} 일어났따! ({World.CurrentGameWorldTimeMS})");
            owner.ChangeState("Idle");
        }
    }

    void ChangeSleeping(string state)
    {
        if( currentState == state )
            return;

        currentState = state;
        if ( state == "sleepGround" )
        {
            Debug.Log($"{owner.ObjectID} 땅바닥에서 잔다!");
            ownerAgent.isStopped = true;

            //owner.Sleeping(currentState);
            hungerChangeValue = 3f;
            fatigueChangeValue = -25f;

            //랜덤시간 타이머 등록
            if( wakeUpTimer != 0 )
            {
                WorldTimer.GetInstance().UnregisterTimer(wakeUpTimer);
                wakeUpTimer = 0;
            }
            
            // 타이머가 계속 .. 생성되고 제거되고 있다.
            wakeUpTimer = WorldTimer.GetInstance().RegisterTimer(World.CurrentGameWorldTimeMS + (int)sleepTime, OnWakeUpCallback);
            Debug.Log($"{sleepTime} 있다가 일어날래!({World.CurrentGameWorldTimeMS})");
        }
        if( state == "sleepShelter" )
        {
            Debug.Log($"{owner.ObjectID} 축사에서 잔다!");
            ownerAgent.isStopped = true;

            //owner.Sleeping(currentState);
            hungerChangeValue = 0f;
            fatigueChangeValue = -50f;

            //랜덤시간 타이머 등록
            if( wakeUpTimer != 0 )
            {
                WorldTimer.GetInstance().UnregisterTimer(wakeUpTimer);
                wakeUpTimer = 0;
            }

            // 타이머가 계속 .. 생성되고 제거되고 있다.
            wakeUpTimer = WorldTimer.GetInstance().RegisterTimer(World.CurrentGameWorldTimeMS + (int)sleepTime, OnWakeUpCallback);
            Debug.Log($"{sleepTime} 있다가 일어날래!({World.CurrentGameWorldTimeMS})");
        }
        else if ( state == "goingToShelter")
        {

        }
    }


    public override void Update()
    {
        base.Update();

        if( currentState == "goingToShelter" )
        {
            if( closeShelter != null && ownerAgent.remainingDistance < closeShelter.recognitionDistance )
            {
                //축사에 도착했따, 
                Debug.Log($"{owner.ObjectID} 축사 도착 했는데..?");
                closeShelter.EnterObject(owner);
                ChangeSleeping("sleepShelter");
            }

            // 아래부분은 매 프레임 검사해줘야 하는 것이므로, 업데이트 함수에서...
            if( owner.Fatigue >= 93.0f || closeShelter == null ) // 혹시나 남겨둠.
            {
                // 너무 졸립다! 
                ChangeSleeping("sleepGround");
                return;
            }
            //가려던 축사가 다 찼으면
            else if( closeShelter.AskEnterable() == false ) // 이것도 계속 검사해줘야하는데, 조건을.. 검사조건을 딱 한번 통과할테니깐. 
            {
                // 이 아래 코드들도 하나만 통과될 거고,
                //다른 축사를 찾아본다
                closeShelter = World.GetInstance().FindEnterablePocketBuilding(owner, World.BuildingType.shelter);
                if( closeShelter != null )
                {
                    // Door는 무조건 있을거야!
                    destination = closeShelter.transform.position;
                    owner.Move(destination);
                }
                else
                {
                    // closeShelter가 null이면 일 때 처리가 없군, 더이상 들어갈 수 있는 가까운 축사가 없을 때.
                    // 아냐, 여기서 바로 처리해주는게 나아요 다음 프레임 업데이트에서 처리하는거는 아냐.
                    // goingToShelter state는 여기서 딱 끝내주는게 맞아요.
                    ChangeSleeping("sleepGround");
                }
            }
        }
    }
}