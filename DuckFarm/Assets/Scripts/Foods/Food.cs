using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : ObjectBase
{
    //알, 작물, 물고기, 사료, 지렁이
    long productionTime; //생산시간\
    //신선도 (신선 -> 부패)
    float freshness = 100;
    protected float freshDecrement = 10; //신선도 감소폭 : 하루에 신선도 몇씩떨굴까? (사료가 가장 잘 부패하지 않는다)
    //배고픔 감소수치
    protected int fullness = 10;
    public int Fullness { get { return fullness; } }

    public Food()
    {
        productionTime = World.CurrentGameWorldTimeMS;
    }

    private void Update()
    {

    }

    public float GetFreshness()
    {
        long currentTiem = World.CurrentGameWorldTimeMS;
        float downFresh = ( currentTiem - productionTime ) * freshDecrement / World.oneDay;
        if( downFresh >= 100 )
            return 0f;

        freshness = 100 - downFresh;
        return freshness;
    }
}
