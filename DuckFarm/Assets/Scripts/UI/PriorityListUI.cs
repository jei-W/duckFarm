using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PriorityListUI : MonoBehaviour
{
    Duck selectedDuck;
    Dictionary<string, GameObject> priorityButtons;
    GameObject cell;
    float cellHightSize;

    string selected = null;

    // Start is called before the first frame update
    void Awake()
    {
        priorityButtons = new Dictionary<string, GameObject>();
        cell = Resources.Load("Prefabs/UI/priorityCell") as GameObject;
        if( cell != null )
            cellHightSize = cell.GetComponent<RectTransform>().rect.height;
    }

    // Update is called once per frame
    void Update()
    {
        if( selectedDuck == null )
            ClosePriorityList();
    }

    public void ShowPersonalPriorityList( Duck duck )
    {
        //남아있는 기존 오리의 우선순위UI 제거
        foreach( var temp in priorityButtons )
        {
            GameObject.Destroy(temp.Value);
        }
        priorityButtons.Clear();

        int count = 1;
        selectedDuck = duck;

        if( selectedDuck == null )
            return;

        for( int i = 1; i < selectedDuck.priorityLists.Count; i++ )
        {
            //우선순위 버튼 생성
            var newCell = GameObject.Instantiate(cell, gameObject.transform);
            var rectTransform = newCell.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, -( ( count - 1 ) * cellHightSize + ( count * 10 ) ));

            //우선순위의 key=이름을 받아온다
            string key = selectedDuck.priorityLists[i].Key;
            newCell.GetComponentInChildren<Text>().text = key;
            priorityButtons.Add(key, newCell);
            ++count;

            var button = newCell.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                if( selected == null ) //다른 선택된 버튼이 없으면 선택된 버튼은 지금 눌린 버튼이다
                    selected = key;
                else if( selected == key ) //이미 선택된 버튼과 같은 버튼이 눌렸으면 선택된것을 없애고 끝낸다
                    selected = null;
                else // 이미 선택된 버튼과 다른 버튼이 눌렸으면 서로 바꾼다
                    SwapPriority(selected, key);
            });
        }

        this.gameObject.SetActive(true);
    }

    public void ClosePriorityList()
    {
        foreach( var temp in priorityButtons )
        {
            GameObject.Destroy(temp.Value);
        }
        priorityButtons.Clear();
        this.gameObject.SetActive(false);
    }

    void SwapPriority( string key1, string key2 )
    {
        selectedDuck.SwapPriorityRanking(key1, key2);

        //선택된 버튼의 위치Position을 바꾼다
        Vector2 tempPosition = priorityButtons[key2].GetComponent<RectTransform>().anchoredPosition;
        priorityButtons[key2].GetComponent<RectTransform>().anchoredPosition = priorityButtons[key1].GetComponent<RectTransform>().anchoredPosition;
        priorityButtons[key1].GetComponent<RectTransform>().anchoredPosition = tempPosition;

        selected = null;
    }
}
