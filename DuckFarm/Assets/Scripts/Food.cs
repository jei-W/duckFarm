using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : ObjectBase
{
    float timeBase = 0;
    float expirationTime = 2f; //신선도가 떨어지는데 걸리는 시간
    //알, 작물, 물고기, 사료, 지렁이
    //신선도 (신선 -> 부패)
    //사료가 가장 잘 부패하지 않는다
    //배고픔 감소수치
    protected int freshness = 100;
    int fullness;

    private void Update()
    {
        timeBase += Time.deltaTime;
        if( timeBase >= expirationTime )
        {
            timeBase = 0;
            freshness--;
            //Debug.Log($"Freshness : {freshness}");
        }
    }
}
