using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    protected Duck owner;
    protected float hungerPace = 2f;
    protected float fatiguePace = 2f;
    public float HungerPace { get { return hungerPace; } }
    public float FatiguePace { get { return fatiguePace; } }

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
    }
}
