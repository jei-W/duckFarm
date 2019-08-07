using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    protected Duck owner;
    //하루동안 변화하는 양
    public float hungerChangeValue = 40f;
    public float fatigueChangeValue = 40f;

    public State( Duck duck )
    {
        owner = duck;
    }

    public virtual void Enter()
    {
        
    }

    public virtual void Exit()
    {
        
    }

    public virtual void Update()
    {
        owner.Hunger = owner.ChangeTargetValue(owner.Hunger, hungerChangeValue); 
        owner.Fatigue = owner.ChangeTargetValue(owner.Fatigue, fatigueChangeValue);
    }
}
