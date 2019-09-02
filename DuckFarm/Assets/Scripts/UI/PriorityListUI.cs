using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PriorityListUI : MonoBehaviour
{
    Duck selectedDuck;
    Dictionary<string, KeyValuePair<GameObject, int>> priorityButtons;
    GameObject cell;
    float cellHightSize;

    string selected = null;

    // Start is called before the first frame update
    void Start()
    {
        priorityButtons = new Dictionary<string, KeyValuePair<GameObject, int>>();
        cell = Resources.Load("Prefabs/UI/priorityCell") as GameObject;
        if( cell != null )
            cellHightSize = cell.GetComponent<RectTransform>().rect.height;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPersonalPriorityList( Duck duck )
    {
        foreach(var temp in priorityButtons)
        {
            GameObject.Destroy(temp.Value.Key);
        }
        priorityButtons.Clear();

        int count = 1;
        selectedDuck = duck;

        if( selectedDuck == null )
            return;

        for( int i = 1; i < selectedDuck.priorityLists.Length; i++ )
        {
            foreach( var priority in selectedDuck.priorityLists[i] )
            {
                var newCell = GameObject.Instantiate(cell, gameObject.transform);
                var rectTransform = newCell.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(0, -( ( count - 1 ) * cellHightSize + ( count * 10 ) ));
                newCell.GetComponentInChildren<Text>().text = priority.Key;
                priorityButtons.Add(priority.Key, new KeyValuePair<GameObject, int>(newCell, i));
                ++count;

                var button = newCell.GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    if( selected == null )
                        selected = priority.Key;
                    else if( selected == priority.Key )
                        selected = null;
                    else
                    {
                        SwapPriority(selected, priority.Key);
                    }
                });
                this.gameObject.SetActive(true);
            }
        }
    }
    public void ClosePriorityList()
    {
        foreach( var temp in priorityButtons )
        {
            GameObject.Destroy(temp.Value.Key);
        }
        priorityButtons.Clear();
        this.gameObject.SetActive(false);
    }

    void SwapPriority( string key1, string key2 )
    {
        int key1Rank = priorityButtons[key1].Value;
        int key2Rank = priorityButtons[key2].Value;

        selectedDuck.ChangePriorityRanking(key1, key2Rank);
        selectedDuck.ChangePriorityRanking(key2, key1Rank);

        Vector2 tempPosition = priorityButtons[key2].Key.GetComponent<RectTransform>().anchoredPosition;
        priorityButtons[key2].Key.GetComponent<RectTransform>().anchoredPosition = priorityButtons[key1].Key.GetComponent<RectTransform>().anchoredPosition;
        priorityButtons[key1].Key.GetComponent<RectTransform>().anchoredPosition = tempPosition;

        priorityButtons[key1] = new KeyValuePair<GameObject, int>(priorityButtons[key1].Key, key2Rank);
        priorityButtons[key2] = new KeyValuePair<GameObject, int>(priorityButtons[key2].Key, key1Rank);

        selected = null;
    }
}
