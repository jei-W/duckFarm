using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTimer
{
    private static WorldTimer _instance = null;
    private static long s_uniqueID = 0;

    //생성자 protected라고 명시적 표기
    protected WorldTimer() { }

    public static WorldTimer GetInstance()
    {
        if( _instance == null )
            _instance = new WorldTimer();

        return _instance;
    }

    class TimerData
    {
        //타이머 고유 아이디
        public long TimerID = 0;
        //콜백이 호출될 월드 타임
        public long Time = 0;
        //콜백함수. timerID를 매개변수로 갖는다
        public Action<long> Callback = null;
    }

    //실행될 타이머들의 리스트
    private Dictionary<long, TimerData> _timer = new Dictionary<long, TimerData>();

    //콜백이 호출 될 월드타임 기준 시간을 입력받는다
    public long RegisterTimer(long time, Action<long> callback)
    {
        //등록 실패
        if( callback == null )
            return -1;

        _timer.Add(s_uniqueID, new TimerData
        {
            TimerID = s_uniqueID,
            Time = time,
            Callback = callback
        }
        );

        return s_uniqueID++;
    }

    public void UnregisterTimer( long id )
    {
        if( _timer.ContainsKey(id) )
            _timer.Remove(id);
    }

    public void Update( long time )
    {
        long currentWorldTime = time;

        List<long> reserveToDeleteTimerList = new List<long>();
        foreach( var timer in _timer )
        {
            if( timer.Value.Time < currentWorldTime )
            {
                timer.Value.Callback?.Invoke(timer.Value.TimerID);
                reserveToDeleteTimerList.Add(timer.Key);
            }
        }

        foreach( var id in reserveToDeleteTimerList )
        {
            _timer.Remove(id);
        }
    }
}
