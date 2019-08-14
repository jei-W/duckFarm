using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State
{
    protected Duck owner;
    protected NavMeshAgent ownerAgent;
    //하루동안 변화하는 양
    public float hungerChangeValue = 10f;
    public float fatigueChangeValue = 5f;

    public State( Duck duck )
    {
        owner = duck;
        ownerAgent = owner.GetComponent<NavMeshAgent>();
    }

    // extraData : State가 시작할 때, 필요한 정보가 있다면 넘긴다.
    // 각 스테이트에서 알아서 추론해서 사용할 것,
    public virtual void Enter( object extraData = null)
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
