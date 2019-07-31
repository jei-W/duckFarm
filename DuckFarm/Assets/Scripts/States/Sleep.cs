using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleep : State
{
    public Sleep( Duck duck ) : base(duck) { }

    public override void Enter()
    {
        //자는동안 배고픔 증가속도가 줄어든다
        hungerPace = 1f;

        Debug.Log("졸려!");
        //들어갈 수 있는 축사를 찾는다
        //들어갈 수 있는 축사가 없으면 현재 위치에서 잠자기를 실행한다
        //노숙은 축사보다 피로도 감소폭이 작으며, 스트레스가 줄어들지 않는다
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        //노숙을 한다면
        owner.Fatigue = DecreaseTargetValue(owner.Fatigue, 3f);

        if( owner.Fatigue == 0 )
        {
            owner.ChangeState(owner.stateList["Idle"]);
        }
    }

    public float DecreaseTargetValue( float figure, float expirationTime )
    {
        Debug.Log("zz..");
        if( figure <= 0 )
        {
            return 0f;
        }

        figure -= Time.deltaTime * expirationTime;
        return figure;
    }
}
