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
        currentState = "followingPartner";
    }

    public override void Exit()
    {
    }
}
