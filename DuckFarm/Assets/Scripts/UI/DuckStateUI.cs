using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DuckStateUI : MonoBehaviour
{
    public Text selectedDuckName;
    public Text curretnDuckState;
    public Image hungerStateBar;
    public Image fatigueStateBar;

    Vector3 stateBarScale;

    Duck selecedDuck;
    bool active;

    // Start is called before the first frame update
    void Start()
    {
        stateBarScale = hungerStateBar.transform.localScale;
        active = false;
    }

    // Update is called once per frame
    void Update()
    {
        if( active )
        {
            curretnDuckState.text = selecedDuck.GetCurrentStateName();
            hungerStateBar.transform.localScale = new Vector3(selecedDuck.Hunger * 0.01f, stateBarScale.y, stateBarScale.z);
            fatigueStateBar.transform.localScale = new Vector3(selecedDuck.Fatigue * 0.01f, stateBarScale.y, stateBarScale.z);

        }


    }

    public void SetSelectedDuckData(Duck duck)
    {
        string sex = duck.male ? "♂" : "♀";
        selectedDuckName.text = $"▶ {duck.name} ({sex})";
        selecedDuck = duck;
        this.gameObject.SetActive(true);
        active = true;
    }
    public void CloseDuckDataPopup()
    {
        this.gameObject.SetActive(false);
        active = false;
    }
}
