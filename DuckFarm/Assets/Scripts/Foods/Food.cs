using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : ObjectBase
{
    //알, 작물, 물고기, 사료, 지렁이
    long productionTime; //생산시간
    //신선도 (신선 -> 부패)
    float freshness = 100;
    protected float freshDecrement = 3; //신선도 감소폭 : 하루에 신선도 몇씩떨굴까? (사료가 가장 잘 부패하지 않는다)
    //배고픔 감소수치
    protected int fullness = 50;
    public int Fullness { get { return fullness; } }

    public Food()
    {
        productionTime = World.CurrentGameWorldTimeMS;
    }

    private void Update()
    {
        freshness -= Time.deltaTime / World.oneDay * freshDecrement;
        if( freshness < 0 )
        {
            //이 푸드가 어딘가에 소속되어있다
            if( this.transform.parent != null )
            {
                var foodOwner = this.transform.parent;
                var isDuck = foodOwner.GetComponent<Duck>();
                var isBuilding = foodOwner.GetComponent<IFoodConsumeableBuilding>();
                if( isDuck != null ) //이 푸드를 들고 있는게 오리다.
                {
                    isDuck.ChangeState("Idle");
                }
                else if( isBuilding != null ) //이 푸드를 들고 있는게 건물이다.
                {
                    isBuilding.BecameRottenFood(this);
                }
            }
            //이 푸드가 길바닥에 있다
            else
            {
                World.GetInstance().RotAwayFood(this);
            }
        }
    }

    public float GetFreshness()
    {
        long currentTiem = World.CurrentGameWorldTimeMS;
        float downFresh = ( currentTiem - productionTime ) * freshDecrement / World.oneDay;
        if( downFresh >= 100 )
            return 0f;

        freshness = 100 - downFresh;
        return freshness;
    }

    public void ResetFreshness()
    {
        freshness = 100;
    }
}
