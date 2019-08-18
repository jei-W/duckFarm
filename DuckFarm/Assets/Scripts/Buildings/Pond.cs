using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 연못 
public class Pond : BuildingBase, IFoodConsumeableBuilding
{
    // 연못은 최대 물고기 생산량이 있다
    //일정시간마다 자동으로 물고기가 생긴다
    // ㅇㅇ 바부가 한다.
    
    // Start is called before the first frame update
    void Start()
    {
        // 아직 연못을 생성하는게 없으므로..
        // 마우스로 직접 pond를 생성할 건데, start에서 world에 job을 만들게 하자
        // 오리가 있든, 없든, 대기상태의 오리가 있든 없든 일단 큐에 담아두겟지.
        World.GetInstance().RequestCatchFish(this);
        recognitionDistance = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool FoodIsFull()
    {
        // 일단 항상 물고기는 풀.
        return true;
    }

    public bool FoodIsEmpty()
    {
        // 일단 항상 물고기는 있다.
        return false;
    }

    public void InputFood( Food targetObject )
    {
        return;
    }

    public Food GetFood()
    {
        // 즉석에서 물고기 생성하여 반환
        return World.GetInstance().ProduceFood(World.FoodType.fish);
    }
}
