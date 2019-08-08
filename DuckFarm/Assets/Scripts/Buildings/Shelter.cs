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

    }
}
