using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mating : State
{
    Duck partner;
    string currentState = "";

    public Mating( Duck duck ) : base(duck) { }
    public override void Enter( object extraData = null )
    {
        partner = World.GetInstance().FindCloseOppositeSexDuck(owner);
        ChangeMatingState("followingPartner");
    }

    public override void Exit()
    {
        owner.ResetCurrentHeat();
    }

    public override void Update()
    {
        base.Update();

        if( partner == null )
            return;

        owner.Move(partner.transform.position);

        if( ownerAgent.remainingDistance < 0.3f )
        {
            switch( partner.GetCurrentState() )
            {
                case "Idle":
                    //상대오리의 발정확률을 증가시킨다
                    partner.ChangeTargetValue(partner.CurrentHeat, 10.0f);
                    break;
                case "Mating":
                    //알만들기 한다
                    ChangeMatingState("matingWithPartner");
                    owner.ChangeState("Idle");
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
