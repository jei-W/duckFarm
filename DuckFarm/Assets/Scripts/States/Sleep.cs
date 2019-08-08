using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleep : State
{
    Vector3 destination;
    PocketBuilding closeShelter = null;
    float sleepTime;
    string currentState = "";
    public Sleep( Duck duck ) : base(duck) { }

    public override void Enter()
    {
        Debug.Log("졸려!");
        closeShelter = World.GetInstance().FindCloseBuilding(owner, World.BuildingType.shelter) as PocketBuilding;

        //들어갈 수 있는 축사를 찾는다
        //들어갈 수 있는 축사가 없으면 현재 위치에서 잠자기를 실행한다
        //노숙은 축사보다 피로도 감소폭이 작으며, 스트레스가 줄어들지 않는다

        if( closeShelter != null )
        {
            currentState = "goingToShelter";
            destination = closeShelter.transform.position;
            owner.Move(destination);
        }
        else
        {
            currentState = "sleepGround";
        }

        sleepTime = Random.Range(World.oneDay * 0.25f, World.oneDay * 0.3f);
    }

    public override void Exit()
    {
        //초기값으로 셋팅
        hungerChangeValue = 40f;
        fatigueChangeValue = 40f;

        //타이머 등록해제
    }

    public override void Update()
    {
        base.Update();

        if( currentState == "sleepGround" || currentState == "sleepShelter" )
        {
            //땅바닥이던 축사던, 잔다
            ownerAgent.isStopped = true;
            owner.Sleeping(currentState);
            //랜덤시간 타이머 등록
            WorldTimer.GetInstance().RegisterTimer(World.CurrentGameWorldTimeMS + (int)sleepTime, ( _TimerID ) => owner.ChangeState("Idle"));
        }
        else if( currentState == "goingToShelter" )
        {
            if( owner.Fatigue >= 90.0f || closeShelter == null )
            {
                // 너무 졸립다! 
                currentState = "sleepGround";
                return;
            }
            //가려던 축사가 다 찼으면
            else if( closeShelter.AskEnterable() == false )
            {
                //다른 축사를 찾아본다
                closeShelter = World.GetInstance().FindEnterablePocketBuilding(owner, World.BuildingType.shelter);
                if( closeShelter != null )
                {
                    destination = closeShelter.transform.position;
                    owner.Move(destination);
                }
            }
            else if( ownerAgent.remainingDistance < 0.01f )
            {
                //축사에 도착했따, 
                closeShelter.EnterObject(owner);
                currentState = "sleepShelter";
            }

        }
    }
}