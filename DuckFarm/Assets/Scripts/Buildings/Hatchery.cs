using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hatchery : PocketBuilding
{
    //오브젝트(알)의 요청에 따라 들여보낸다
    //들어온 알의 부화 시간을 잰다(28일)
    public override bool EnterObject( ObjectBase targetObject )
    {
        var targetEgg = targetObject.GetComponent<Egg>();

        if( !targetEgg.hatchable )
            return false;

        long currentTime = System.DateTime.Now.Millisecond;
        targetEgg.hatchStartTime = currentTime;

        WorldTimer.GetInstance().RegisterTimer(currentTime + (long)targetEgg.remainingHatchTime * 2000, ( long timerID ) => Hatch(targetObject.ObjectID));

        return base.EnterObject(targetObject);
    }

    //들어있는 알의 남은 부화 시간을 보여줄 수 있다
    public override void ShowObjectsList()
    {
        foreach( var innerObject in Objects )
        {
            var innerEgg = innerObject.Value.GetComponent<Egg>();
            innerEgg.remainingHatchTime -= Time.deltaTime * 0.5f;
           // Debug.Log($"{innerObject.Key} : {(int)innerEgg.remainingHatchTime + 1}일");
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
        ShowObjectsList();
    }
}
