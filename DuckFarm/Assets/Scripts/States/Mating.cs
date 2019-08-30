using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mating : State
{
    Duck partner;
    string currentState = "";
    bool isReadyMakeEgg = false;
    long matingTimer = 0;

    public Mating( Duck duck ) : base(duck) { }
    public override void Enter( object extraData = null )
    {
        base.Enter(extraData);

        isReadyMakeEgg = false;
        partner = World.GetInstance().FindCloseOppositeSexDuck(owner);
        ChangeMatingState("followingPartner");

        //타이머 등록 : 오리는 발정시 상대 오리를 3일 만 따라다닌다
        if( matingTimer != 0 )
        {
            WorldTimer.GetInstance().UnregisterTimer(matingTimer);
            matingTimer = 0;
        }
        matingTimer = WorldTimer.GetInstance().RegisterTimer(World.CurrentGameWorldTimeMS + World.oneDay * 3, EscapeState);
    }

    void EscapeState( long timerID )
    {
        if( timerID == matingTimer )
        {
            Debug.Log($"{owner.ObjectID} 에잇! 사랑이 식었어!");
            owner.ChangeState("Idle");

            matingTimer = 0;
        }
    }

    public override void Exit()
    {
        if( matingTimer != 0 )
        {
            WorldTimer.GetInstance().UnregisterTimer(matingTimer);
            matingTimer = 0;
        }

        owner.ResetCurrentHeat();
        owner.LastMatingTime = World.CurrentGameWorldTimeMS;
    }

    public bool IsReadyMakeEgg()
    {
        return isReadyMakeEgg;
    }

    public override void Update()
    {
        base.Update();

        if ( "matingWithPartner" == currentState)
        {
            owner.ChangeState("Idle");
            return;
        }

        if ( partner == null )
        {
            owner.ChangeState("Idle");
            return;
        }

        owner.Move(partner.transform.position);

        if ( (partner.transform.position - owner.transform.position).sqrMagnitude < owner.recognitionDistance )
        {
            isReadyMakeEgg = true;
            switch( partner.GetCurrentStateName() )
            {
                case "Idle":
                    //상대오리의 발정확률을 증가시킨다
                    partner.ChangeTargetValue(partner.CurrentHeat, 10.0f);
                    break;
                case "Mating":
                    // 파트너가 메이팅이지만, 아직 준비가 덜 됬을 수도 있다.
                    // 파트너가 알 만들 준비를 끝냈는지? 물어봐야한다.
                    Mating partnerState = partner.GetCurrentState() as Mating;
                    if( partnerState.IsReadyMakeEgg() && isReadyMakeEgg )
                    {
                        //알만들기 한다
                        ChangeMatingState("matingWithPartner");
                    }
                    break;
                default:
                    partner = World.GetInstance().FindCloseOppositeSexDuck(owner);
                    break;
            }
        }
    }

    void ChangeMatingState( string state )
    {
        if( currentState == state )
            return;

        currentState = state;
        if( state == "followingPartner" )
        {

        }
        else if( state == "matingWithPartner" )
        {
            // 메챠쿠챠
            // 알은 암컷만 낳는다
            if( owner.male == false )
            {
                Debug.Log($"{owner.name} 낳았다!");
                var egg = World.GetInstance().LayEgg(owner.transform.position);
                //알을 옮기는건 job..
            }
        }
    }
}
