using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    //public enum 건물타입
    public enum BuildingType { mainStorage, hatchery, shelter, feedFactory }

    //빌딩 리스트(key=빌딩ID/ value=gameObject)
    Dictionary<string, GameObject> buildingList = new Dictionary<string, GameObject>();
    //오리 리스트
    Dictionary<string, GameObject> ducksList = new Dictionary<string, GameObject>();


    void Update()
    {
        //worldTimer.Update(시간);
        WorldTimer.GetInstance().Update(시간);
    }

    void CreateNewGame()
    {
        //중앙창고건물 생성
        BuildBuilding(BuildingType.mainStorage, Vector3.zero);
        //축사 생성
        BuildBuilding(BuildingType.hatchery, new Vector3(30, 0, 30));
        //알 2개 생성(성별 다름)
    }

    //건물생성함수(건물타입, 생성위치) : 매개변수를 토대로 프리팹을 가져와서 인스턴스화 한다
    //건물생성함수는 빌딩ID("건물타입_시간")를 생성한다
    void BuildBuilding( BuildingType type, Vector3 position )
    {
        string objID = type.ToString() + "_" + "시간";
        GameObject building = Instantiate(Resources.Load("Prefabs/Building/" + type.ToString()), position, Quaternion.identity) as GameObject;
        building.GetComponent<ObjectBase>().ObjectID = objID;
        buildingList.Add(objID, building);
    }

    //환경 오브젝트 생성??

    //오리 생성함수
    void CreateDuckling( Vector3 position )
    {
        GameObject duckling = Instantiate(Resources.Load("Prefabs/duck"), position, Quaternion.identity) as GameObject;
        ducksList.Add("duck_" + "시간", duckling);
    }
}
