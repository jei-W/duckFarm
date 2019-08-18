using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    Text currentDate;
    Text duckCount;
    Text woodCount;
    Text stoneCount;
    Text foodCount;

    // Start is called before the first frame update
    void Start()
    {
        currentDate = GameObject.Find("CurrentDate").GetComponent<Text>();
        duckCount = GameObject.Find("DuckCount").GetComponent<Text>();
        woodCount = GameObject.Find("WoodCount").GetComponent<Text>();
        stoneCount = GameObject.Find("StoneCount").GetComponent<Text>();
        foodCount = GameObject.Find("FoodCount").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        currentDate.text = World.GetInstance().GetCurrentGameWorldTimeToString();
        duckCount.text = $"오리  {World.GetInstance().GetDuckCount()}마리";
        woodCount.text = $"목재  {World.GetInstance().GetWoodCount()}개";
        stoneCount.text = $"석재  {World.GetInstance().GetStoneCount()}개";
        foodCount.text = $"식량(알)  {World.GetInstance().GetFoodCount()}({World.GetInstance().GetEggCount()})개";
    }
}
