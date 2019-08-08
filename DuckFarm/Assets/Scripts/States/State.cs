using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State
{
    protected Duck owner;
    protected NavMeshAgent ownerAgent;
    //하루동안 변화하는 양
    public float hungerChangeValue = 30f;
    public float fatigueChangeValue = 30f;

    public State( Duck duck )
    {
        owner = duck;
        ownerAgent = owner.GetComponent<NavMeshAgent>();
    }

    public virtual void Enter()
    {
        
    }

    public virtual void Exit()
    {
        
    }

    public virtual void FixedUpdate()
    {

    }

    public virtual void Update()
    {
        owner.Hunger = owner.ChangeTargetValue(owner.Hunger, hungerChangeValue); 
        owner.Fatigue = owner.ChangeTargetValue(owner.Fatigue, fatigueChangeValue);
    }
}
