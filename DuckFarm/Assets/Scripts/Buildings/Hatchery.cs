using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hatchery : PocketBuilding
{
    //오브젝트(알)의 요청에 따라 들여보낸다
    //들어온 알의 부화 시간을 잰다(28일)
    public override void EnterObject( ObjectBase targetObject )
    {
        base.EnterObject(targetObject);
        var targetEgg = targetObject.GetComponent<Egg>();

        //알이 부화가 안되는 썩은알이라면 리턴
        if( !targetEgg.hatchable )
            return;

        long currentTime = System.DateTime.Now.Millisecond;
        targetEgg.hatchStartTime = currentTime;
        //알이 들어왔으면 알 부화 타이머를 켠다
        WorldTimer.GetInstance().RegisterTimer(currentTime + (long)targetEgg.remainingHatchTime * 2000, ( long timerID ) => Hatch(targetObject.ObjectID));
    }

    //들어있는 알의 남은 부화 시간을 보여줄 수 있다
    public override void ShowObjectsList()
    {
        foreach( var innerObject in Objects )
        {
            var innerEgg = innerObject.Value.GetComponent<Egg>();
            innerEgg.remainingHatchTime -= Time.deltaTime * 0.5f;
            Debug.Log($"{innerObject.Key} : {(int)innerEgg.remainingHatchTime + 1}일");
        }
    }

    //알을 없애고 월드에 아기오리를 요청한다
    public void Hatch(string objectID)
    {
        if( Objects.ContainsKey(objectID) == false )
            return;

        //월드에 아기오리를 요청한다(objctID)
        World.GetInstance().OnHatchEggInHatchery(this, Objects[objectID]);

        Objects.Remove(objectID);
    }

    private void Update()
    {
        //ShowObjectsList();
    }
}
