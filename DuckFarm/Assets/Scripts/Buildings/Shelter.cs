using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelter : PocketBuilding
{
    public override void EnterObject( ObjectBase targetObject )
    {
        base.EnterObject(targetObject);

        //오리의 입고시간을 기록한다(축사에서 잠자기 시작한 시간)
        targetObject.GetComponent<Duck>().SomethingStaetTime = System.DateTime.Now.Millisecond;

        //오리가 언제까지 잘지 정하고
        //타이머를 맞추자
    }

    private void Update()
    {
        
    }
}
