using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// World 다음으로, 스크립트 업데이트 우선순위가 높아야한다.
public class InputSystemManager : MonoBehaviour
{
    public static InputSystemManager Instance { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        foreach( var callback in reserveMouseDownCallbackList )
        {
            Debug.Log("이벤트 삭제");
            mouseDownCallbackList.Remove(callback);
        }
        reserveMouseDownCallbackList.Clear();

        if ( Input.GetMouseButtonDown(0) )
        {
            foreach ( var callback in mouseDownCallbackList )
            {
                if ( callback.Invoke(Input.mousePosition) )
                    break;
            }
        }
    }

    // 음.. 우선순위.. 필요 읍나.
    public delegate bool MouseDownDelegate( Vector2 position );
    List<MouseDownDelegate> mouseDownCallbackList = new List<MouseDownDelegate>();
    List<MouseDownDelegate> reserveMouseDownCallbackList = new List<MouseDownDelegate>();
    public void RegisterMouseDownEvent( MouseDownDelegate callback, bool topmost = false )
    {
        // delegate에 해당 callback을 등록해둔다.
        if ( topmost )
            mouseDownCallbackList.Insert(0, callback);
        else
            mouseDownCallbackList.Add(callback);
    }

    public void UnregisterMouseDownEvent( MouseDownDelegate callback )
    {
        reserveMouseDownCallbackList.Add(callback);
    }
}
