using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelter : PocketBuilding
{
    public override void EnterObject( ObjectBase targetObject )
    {
        Duck targetDuck = targetObject as Duck;
        base.EnterObject(targetObject);

        //오리의 입고시간을 기록한다(축사에서 잠자기 시작한 시간)
        long currentTime = World.CurrentGameWorldTimeMS;
        targetObject.GetComponent<Duck>().SomethingStartTime = currentTime;

        //오리가 언제까지 잘지 정하고
        int sleepingTime = (int)Random.Range(World.oneDay * 0.25f, World.oneDay * 0.3f);
        //타이머를 맞추자
        WorldTimer.GetInstance().RegisterTimer(currentTime + sleepingTime, ( timerID ) => targetDuck.ChangeState(targetDuck.stateList["Idle"]));
    }
}
