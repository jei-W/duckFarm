using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : Food
{
    const int freshTime = 28; //알이 완전히 썩기까지 시간
    public bool hatchable = true;
    public long hatchStartTime = 0;
    public float remainingHatchTime = 3; //단위=1일, 1일=2초
    //알 나이(5일이 지나면 부화불가) -> 신선도 몇 이상? (임시)60 아래 부화불가
    //먹었을 때 스트레스 수치를 증가시킨다

    public Egg()
    {
        freshDecrement = 100 / freshTime;
    }

    private void Update()
    {
        if( GetFreshness() < 60 )
            hatchable = false;
    }
}
