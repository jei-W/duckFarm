using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggSelectMenu : MonoBehaviour
{
    Egg targetEgg = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCarryOnEggToHatchery()
    {
        // 임시로 버튼을 누를 때 마다, 아무 알이나 선택. 
        Egg groundEgg = World.GetInstance().FindEggAtGround();
        if( groundEgg == null )
            return;

        World.GetInstance().RequestCarryOnEggToHatchery(groundEgg);
    }

    public void OnCarryOnEggToMainStorage()
    {
        // 임시로 버튼을 누를 때 마다, 아무 알이나 선택. 
        Egg groundEgg = World.GetInstance().FindEggAtGround();
        if( groundEgg == null )
            return;

        World.GetInstance().RequestCarryOnEggToMainStorage(groundEgg);
    }
}
