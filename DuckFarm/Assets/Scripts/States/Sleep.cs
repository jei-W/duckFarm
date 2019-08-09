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

    public override void Enter()
    {
        Debug.Log($"{owner.ObjectID} 졸려!");
        closeShelter = World.GetInstance().FindCloseBuilding(owner, World.BuildingType.shelter) as PocketBuilding;

        //들어갈 수 있는 축사를 찾는다
        //들어갈 수 있는 축사가 없으면 현재 위치에서 잠자기를 실행한다
        //노숙은 축사보다 피로도 감소폭이 작으며, 스트레스가 줄어들지 않는다

        if( closeShelter != null )
        {
            ChangeSleeping("goingToShelter");
            destination = closeShelter.Door.position;
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
        //초기값으로 셋팅
        hungerChangeValue = 30f;
        fatigueChangeValue = 30f;

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
            Debug.Log($"{owner.ObjectID} 일어났따! ({World.CurrentGameWorldTimeMS})");
            owner.ChangeState("Idle");

            // 나를 꺼내쥬....
            if ( currentState == "sleepShelter" && closeShelter != null )
                closeShelter.ExitObject(owner.ObjectID);

            wakeUpTimer = 0;
        }
    }

    void ChangeSleeping(string state)
    {
        if( currentState == state )
            return;

        currentState = state;
        if ( state == "sleepGround" || state == "sleepShelter" )
        {
            Debug.Log($"{owner.ObjectID} 잔다!");
            //땅바닥이던 축사던, 잔다
            ownerAgent.isStopped = true;
            // 한번만 해야할 것 같은데.. 흠 

            owner.Sleeping(currentState);
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

    public override void FixedUpdate()
    {
        // 물리의 위치비교니깐.. 여기서 해야겠는걸?

        // 거리가 0.01f... 일수가 없을 것 같아요. 왜냐하면 쉘터의 오브젝트 한 중앙일텐데, 충돌 영역 때문에 
        // 오리가 저만큼 다가가지 않을꺼거든.
        // 저 정도가 안정거리 일텐데, position 보면은 절대 거리 차가 0.01f 가 안될거여요.
        // 로그 봤듯이 거리차이가 1.5 이상이여요
        // 정확하게 검사하려면, 타겟 오브젝트의 충돌 영역 반지름, 나의 반지름의 합. 이될거여요.
        // 아니면 정확하게 충돌영역 밖의 입장 위치를 넣어두는게 좋을 것 같아요.

        if( currentState == "goingToShelter" )
        {
            Debug.Log($"{owner.gameObject.name} 쉘터와의 거리 차이 : {ownerAgent.remainingDistance}");
            if( closeShelter != null && ownerAgent.remainingDistance < 0.2f )
            {
                //축사에 도착했따, 
                Debug.Log($"{owner.ObjectID} 축사 도착 했는데..?");
                closeShelter.EnterObject(owner);
                ChangeSleeping("sleepShelter");
            }
        }
    }

    public override void Update()
    {
        base.Update();

        if( currentState == "goingToShelter" )
        {
            // 아래부분은 매 프레임 검사해줘야 하는 것이므로, 업데이트 함수에서...
            if( owner.Fatigue >= 90.0f || closeShelter == null ) // 혹시나 남겨둠.
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
                    destination = closeShelter.Door.position;
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