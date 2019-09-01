using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    public static MainUI Instance { get; private set; }

    Dictionary<string, Text> currentGameData = new Dictionary<string, Text>();
    DuckStateUI duckStatePanel;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        //현재 게임 데이터 리스트 등록
        currentGameData.Add("CurrentDate", GameObject.Find("CurrentDate").GetComponent<Text>());
        currentGameData.Add("DuckCount", GameObject.Find("DuckCount").GetComponent<Text>());
        currentGameData.Add("WoodCount", GameObject.Find("WoodCount").GetComponent<Text>());
        currentGameData.Add("StoneCount", GameObject.Find("StoneCount").GetComponent<Text>());
        currentGameData.Add("FoodCount", GameObject.Find("FoodCount").GetComponent<Text>());

        duckStatePanel = GameObject.Find("DuckStatePanel").GetComponent<DuckStateUI>();
        duckStatePanel.gameObject.SetActive(false);

        //오리 클릭하면 오리 정보 보여주는 이벤트 등록
        InputSystemManager.Instance.RegisterMouseDownEvent(World.GetInstance().OnClickDuck);
    }

    // Update is called once per frame
    void Update()
    {
        currentGameData["CurrentDate"].text = World.GetInstance().GetCurrentGameWorldTimeToString();
        currentGameData["DuckCount"].text = $"오리  {World.GetInstance().GetDuckCount()}마리";
        currentGameData["WoodCount"].text = $"목재  {World.GetInstance().GetWoodCount()}개";
        currentGameData["StoneCount"].text = $"석재  {World.GetInstance().GetStoneCount()}개";
        currentGameData["FoodCount"].text = $"식량(알)  {World.GetInstance().GetFoodCount()}({World.GetInstance().GetEggCount()})개";
    }

    public void ShowDuckState( Duck duck )
    {
        duckStatePanel.SetSelectedDuckData(duck);
        duckStatePanel.gameObject.SetActive(true);
    }
    public void HideDuckState()
    {
        duckStatePanel.gameObject.SetActive(false);
    }

}
