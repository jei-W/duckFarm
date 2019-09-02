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
        World.GetInstance().RequestCarryOnEggToHatchery();
    }

    public void OnCarryOnEggToMainStorage()
    {
        World.GetInstance().RequestCarryOnEggToMainStorage();
    }
}
