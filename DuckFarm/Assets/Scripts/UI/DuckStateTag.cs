using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DuckStateTag : MonoBehaviour
{
    Duck owner;

    private GameObject stateBar;
    private TextMesh stateBarText;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        owner = GetComponent<Duck>();
        mainCamera = Camera.main;

        stateBar = Instantiate(Resources.Load("Prefabs/StateBar")) as GameObject;
        stateBar.transform.parent = owner.transform;
        stateBar.transform.localPosition = Vector3.zero + Vector3.up * 2.0f;

        stateBarText = stateBar.GetComponent<TextMesh>();
        if( stateBarText == null )
            return;
    }

    // Update is called once per frame
    void Update()
    {
        stateBar.transform.LookAt(stateBar.transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
        stateBarText.text = $"{owner.name} : {owner.GetCurrentStateName()}" + $"\n허기 : {(int)owner.Hunger}/100" + $"\n피로 : {(int)owner.Fatigue}/100";
    }
}
