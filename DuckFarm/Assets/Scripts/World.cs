﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    protected World()
    {
        _instance = this;
    }
    private static World _instance = null;
    public static World GetInstance()
    {
        if( _instance == null )
            _instance = new World();

        return _instance;
    }

    //public enum 건물타입
    public enum BuildingType { mainStorage, hatchery, shelter, feedFactory }

    //빌딩 리스트(key=빌딩ID/ value=gameObject)
    Dictionary<string, BuildingBase> buildingList = new Dictionary<string, BuildingBase>();
    //오리 리스트
    Dictionary<string, GameObject> ducksList = new Dictionary<string, GameObject>();

    // 밀리세컨드 단위에 
    static long CurrentGameWorldTimeMS = 0;

    void Start()
    {
        CreateNewGame();
    }

    // 스크립트 실행 순서에서 항상 World가 먼저 실행되도록 설정하여야 한다.
    // 그래야 월드 타임이 올바르게, 모두에게 전달될 수 있다.
    void Update()
    {
        CurrentGameWorldTimeMS += (long)(Time.deltaTime * 1000.0f);
        
        //worldTimer.Update(시간);
        WorldTimer.GetInstance().Update(CurrentGameWorldTimeMS);
    }

    static long s_uniqueID = 0;
    void CreateNewGame()
    {
        //중앙창고건물 생성
        BuildBuilding(BuildingType.mainStorage, Vector3.zero);
        //부화장 생성
        PocketBuilding hatch = BuildBuilding(BuildingType.hatchery, new Vector3(3, 0, 3)) as PocketBuilding;
        //축사 생성
        PocketBuilding shelter1 = BuildBuilding(BuildingType.shelter, new Vector3(-3, 0, 3)) as PocketBuilding;
        PocketBuilding shelter2 = BuildBuilding(BuildingType.shelter, new Vector3(3, 0, -3)) as PocketBuilding;


        if( hatch != null )
        {
            //알 2개 생성(성별 다름)
            var egg1 = LayEgg(Vector3.zero);
            var egg2 = LayEgg(Vector3.zero);

            hatch.EnterObject(egg1);
            hatch.EnterObject(egg2);
        }
        else
        {
            Debug.Log("Pocket이 안되");
        }
    }

    //건물생성함수(건물타입, 생성위치) : 매개변수를 토대로 프리팹을 가져와서 인스턴스화 한다
    //건물생성함수는 빌딩ID("건물타입_시간")를 생성한다
    BuildingBase BuildBuilding( BuildingType type, Vector3 position )
    {
        // 이 때의 시간은 WorldTime이 아닌, 
        // Unix time stamp를 써야한다. 
        string objID = $"{type.ToString()}_{s_uniqueID++}";

        var resource = Resources.Load($"Prefabs/Building/{type.ToString()}");
        if ( resource == null )
        {
            Debug.Log($"{type.ToString()}가 없어..");
            return null;
        }

        GameObject building = Instantiate(resource, position, Quaternion.identity) as GameObject;

        BuildingBase objectBase = building.GetComponent<BuildingBase>();
        objectBase.ObjectID = objID;
        objectBase.buildingType = type;

        buildingList.Add(objID, objectBase);

        return objectBase;
    }

    //환경 오브젝트 생성??

    //알 생성
    ObjectBase LayEgg( Vector3 position )
    {
        var resource = Resources.Load("Prefabs/egg");
        if( resource == null )
        {
            Debug.Log("알이 없어..");
            return null;
        }
        GameObject egg = Instantiate(resource, position, Quaternion.identity) as GameObject;

        string objID = $"egg_{s_uniqueID++}";
        ObjectBase objectBase = egg.GetComponent<ObjectBase>();
        objectBase.ObjectID = objID;

        return objectBase;
    }

    //오리 생성함수
    public void OnHatchEggInHatchery(Hatchery hatchery, ObjectBase egg)
    {
        Debug.Log("삐약");

        GameObject duckling = Instantiate(Resources.Load("Prefabs/duck"), hatchery.transform.position, Quaternion.identity) as GameObject;
        string objectID = $"duck_{s_uniqueID++}";
        ObjectBase objectBase = duckling.GetComponent<ObjectBase>();
        objectBase.ObjectID = objectID;

        ducksList.Add(objectID, duckling);

        GameObject.Destroy(egg.gameObject);
    }

    public void OnDuckDied( GameObject duck )
    {
        string duckID = duck.GetComponent<ObjectBase>().ObjectID;
        ducksList.Remove(duckID);
        Debug.Log($"{duckID} : 오리가 죽었다.");
    }

    //가까이에 있는 건물를 알려주자
    public BuildingBase FindCloseBuilding(ObjectBase finder, BuildingType buildingType)
    {
        float minDistance = float.MaxValue;
        BuildingBase find = null;

        foreach( var building in buildingList )
        {
            if( building.Value.buildingType == buildingType )
            {
                float distansce = ( building.Value.transform.position - finder.transform.position ).sqrMagnitude;
                if( minDistance >= distansce )
                {
                    minDistance = distansce;
                    find = building.Value;
                }
            }
        }

        return find;
    }

    public PocketBuilding FindEnterablePocketBuilding( ObjectBase finder, BuildingType buildingType )
    {
        float minDistance = float.MaxValue;
        PocketBuilding find = null;

        foreach( var building in buildingList )
        {
            PocketBuilding pocketBuilding = building.Value as PocketBuilding;

            if( pocketBuilding.buildingType == buildingType && pocketBuilding.AskEnterable() )
            {
                float distansce = ( building.Value.transform.position - finder.transform.position ).sqrMagnitude;
                if( minDistance >= distansce )
                {
                    minDistance = distansce;
                    find = pocketBuilding;
                }
            }
        }

        return find;
    }
}
