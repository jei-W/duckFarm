using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    // 밀리세컨드 단위에 
    public static long CurrentGameWorldTimeMS = 0;

    public static int oneDay = 10000; //1일 = Millisecond
    public static float reverseOneDay = 1000.0f / oneDay;
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
    public enum BuildingType { mainStorage, hatchery, shelter, feedFactory, pond }
    public enum FoodType { egg, worm, fish, feed, crop }
    public enum ResourceType { wood, stone }


    //빌딩 리스트(key=빌딩ID/ value=gameObject)
    Dictionary<string, BuildingBase> buildingList = new Dictionary<string, BuildingBase>();
    //오리 리스트
    Dictionary<string, Duck> ducksList = new Dictionary<string, Duck>();
    Dictionary<string, Food> foodsList = new Dictionary<string, Food>();
    Dictionary<string, Resource> resourcesList = new Dictionary<string, Resource>();

    // 일거리 타입
    public enum JobType
    {
        CatchFishingInPond, // 연못에서 물고기 잡기
        CarryOnEggToHatchery, // 알을 부화장에 넣어
        CarryOnEggToMainStorage, // 알을 창고에 넣어
        CarrySomethingStopped //뭔가 옮기다 중단됐었다..?
    }
    Dictionary<JobType, Queue<JobInfo>> jobs = new Dictionary<JobType, Queue<JobInfo>>();

    void Start()
    {
        CreateNewGame();

        jobs.Add(JobType.CatchFishingInPond, new Queue<JobInfo>());
        jobs.Add(JobType.CarryOnEggToHatchery, new Queue<JobInfo>());
        jobs.Add(JobType.CarryOnEggToMainStorage, new Queue<JobInfo>());
        jobs.Add(JobType.CarrySomethingStopped, new Queue<JobInfo>());
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
        PocketBuilding hatch = BuildBuilding(BuildingType.hatchery, new Vector3(6.5f, 0, 6.5f)) as PocketBuilding;
        //축사 생성
        PocketBuilding shelter1 = BuildBuilding(BuildingType.shelter, new Vector3(-4, 0, 4.5f)) as PocketBuilding;
        PocketBuilding shelter2 = BuildBuilding(BuildingType.shelter, new Vector3(4, 0, -4.5f)) as PocketBuilding;
        
        if( hatch != null )
        {
            //알 2개 생성(성별 다름)
            var egg1 = LayEgg(Vector3.zero, true);
            var egg2 = LayEgg(Vector3.zero, false);

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
    public BuildingBase BuildBuilding( BuildingType type, Vector3 position )
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

    // 사전 설치 오브젝트 생성.. 반투명하게 하고시픈뎅..
    public Transform BuildPreInstallBuilding( BuildingType type )
    {
        var resource = Resources.Load($"Prefabs/Building/{type.ToString()}_preInstall");
        if ( resource == null )
        {
            Debug.Log($"{type.ToString()}가 없어..");
            return null;
        }

        GameObject building = Instantiate(resource) as GameObject;

        return building.GetComponent<Transform>();
    }
    //환경 오브젝트 생성??

    //알 생성(성별랜덤)
    public Egg LayEgg( Vector3 position )
    {
        var resource = Resources.Load("Prefabs/Food/egg");
        if( resource == null )
        {
            Debug.Log("알이 없어..");
            return null;
        }
        GameObject objectBase = Instantiate(resource, position, Quaternion.identity) as GameObject;

        string objID = $"egg_{s_uniqueID++}";
        Egg egg = objectBase.GetComponent<Egg>();
        egg.ObjectID = objID;

        int r = GlobalRandom.GetRandom(0, 10);
        Debug.Log($"랜덤 {r}");
        bool male = r < 5;
        egg.male = male;

        foodsList.Add(objID, egg);
        eggCount++;

        return egg;
    }
    // 알 생성(성별지정)
    public Egg LayEgg( Vector3 position, bool male )
    {
        var egg = LayEgg(position);
        egg.male = male;

        return egg;
    }

    //오리 생성함수
    public void OnHatchEggInHatchery(Hatchery hatchery, Egg egg)
    {
        Debug.Log("삐약");

        GameObject objectBase = Instantiate(Resources.Load("Prefabs/duck"), hatchery.transform.position, Quaternion.identity) as GameObject;
        string objectID = $"duck_{s_uniqueID++}";
        Duck duckling = objectBase.GetComponent<Duck>();
        duckling.ObjectID = objectID;
        duckling.male = egg.male;
        duckling.name = objectID;

        ducksList.Add(objectID, duckling);

        foodsList.Remove(egg.ObjectID);
        GameObject.Destroy(egg.gameObject);
        eggCount--;
    }

    public void OnDuckDied( Duck duck )
    {
        string duckID = duck.ObjectID;
        ducksList.Remove(duckID);
        Debug.Log($"{duckID} : 오리가 죽었다.");
    }

    public Food ProduceFood(FoodType foodType)
    {
        string objID = $"{foodType.ToString()}_{s_uniqueID++}";

        var resource = Resources.Load($"Prefabs/Food/{foodType.ToString()}");
        if( resource == null )
        {
            Debug.Log($"{foodType.ToString()}가 없어..");
            return null;
        }

        var newObj = Instantiate(resource) as GameObject;
        var newFood = newObj.GetComponent<Food>();

        newFood.ObjectID = objID;

        foodsList.Add(objID, newFood);

        return newFood;
    }

    public void DuckAteFood(Food food)
    {
        foodsList.Remove(food.ObjectID);
        GameObject.Destroy(food.gameObject);

        if( food is Egg)
            eggCount--;
    }

    public void RotAwayFood( Food food )
    {
        //월드에 푸드가 없어졌음을 알린다
        // = 오리가 푸드를 먹은것과 같은 기능을 한다
        Debug.Log($"{food.name}이 썩어 없어졌다");
        DuckAteFood(food);
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
    public BuildingBase FindMainStorage()
    {
        BuildingBase mainStorage = null;
        foreach( var building in buildingList )
        {
            if( building.Value.buildingType == BuildingType.mainStorage )
            {
                mainStorage = building.Value;
                break;
            }
        }
        return mainStorage;
    }

    public PocketBuilding FindEnterablePocketBuilding( ObjectBase finder, BuildingType buildingType )
    {
        float minDistance = float.MaxValue;
        PocketBuilding find = null;

        foreach( var building in buildingList )
        {
            PocketBuilding pocketBuilding = building.Value as PocketBuilding;
            if( pocketBuilding == null )
                continue;

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

    public BuildingBase FindFoodLeftRestaurant( ObjectBase finder )
    {
        float minDistance = float.MaxValue;
        BuildingBase find = null;

        //사료가 남아있는 사료공장 중, 가까이 있는 거를 찾는다
        foreach( var building in buildingList )
        {
            var restaurant = building.Value as FeedFactory;

            if( restaurant == null)
            {
                continue;
            }
            else //빌딩 == 사료공장
            {
                if( restaurant.FoodIsEmpty() )
                    continue;

                float distansce = ( restaurant.transform.position - finder.transform.position ).sqrMagnitude;
                if( minDistance >= distansce )
                {
                    minDistance = distansce;
                    find = restaurant;
                }
            }
        }

        // 사료제공 가능한 사료공장이 없으면 저장고로 간다
        if( find == null )
            find = FindMainStorage();

        return find;
    }

    public Duck FindCloseOppositeSexDuck(Duck soloDuck)
    {
        float minDistance = float.MaxValue;
        Vector3 soloDuckPos = soloDuck.transform.position;
        Duck partner = null;

        List<Duck> oppositSexDucks = new List<Duck>();

        foreach( var duck in ducksList )
        {
            if( duck.Value.male != soloDuck.male )
            {
                string duckState = duck.Value.GetCurrentStateName();
                if( duckState == "Idle" || duckState == "Mating" )
                    oppositSexDucks.Add(duck.Value);
            }
        }

        foreach( var otherDuck in oppositSexDucks )
        {
            float betweenDistance = Vector3.SqrMagnitude(soloDuckPos - otherDuck.transform.position);
            if( betweenDistance < minDistance )
            {
                partner = otherDuck;
                minDistance = betweenDistance;
            }
        }

        return partner;
    }

    public Egg FindEggAtGround()
    {
        foreach( var food in foodsList )
        {
            if ( food.Value is Egg && food.Value.transform.parent == null )
            {
                return food.Value as Egg;
            }
        }

        return null;
    }
    public Food FindFoodAtGroundNotEgg()
    {
        foreach( var food in foodsList )
        {
            if( food.Value is Egg == false && food.Value.transform.parent == null )
            {
                return food.Value;
            }
        }

        return null;
    }

    #region Job Queue
    public void RequestCatchFish( BuildingBase pond )
    {
        jobs[JobType.CatchFishingInPond].Enqueue(new JobInfo()
        {
            targetBuilding = pond
        });
    }

    public void RequestCarryOnEggToHatchery( Egg egg)
    {
        jobs[JobType.CarryOnEggToHatchery].Enqueue(new JobInfo()
        {
            targetObject = egg
        }
        );
    }
    public void RequestCarryOnEggToMainStorage( Egg egg )
    {
        jobs[JobType.CarryOnEggToMainStorage].Enqueue(new JobInfo()
        {
            targetObject = egg
        }
        );
    }
    public void RequestCarrySomethingStopped( ObjectBase something, BuildingBase building )
    {
        jobs[JobType.CarrySomethingStopped].Enqueue(new JobInfo()
        {
            targetObject = something,
            targetBuilding = building
        }
        );
    }

    public bool IsJobEmpty(JobType type)
    {
        return jobs[type].Count == 0;
    }

    public JobInfo GetFirstJob(JobType type)
    {
        if ( jobs[type].Count == 0 )
            return null;

        return jobs[type].Dequeue();
    }
    #endregion

    #region MainUI Data
    int woodCount = 0;
    int stoneCount = 0;
    int eggCount = 0;

    public string GetCurrentGameWorldTimeToString()
    {
        int days = Convert.ToInt32(CurrentGameWorldTimeMS / oneDay);
        int time = Convert.ToInt32(CurrentGameWorldTimeMS % oneDay);
        float oneMinute = oneDay / 1440.0f;
        int timeToMinute = Convert.ToInt32(time / oneMinute);

        return string.Format("{0}일째 {1,2:D2}시 {2,2:D2}분", days, timeToMinute / 60, timeToMinute % 60);
    }
    public int GetDuckCount()
    {
        return ducksList.Count;
    }
    public int GetWoodCount()
    {
        return woodCount;
    }
    public int GetStoneCount()
    {
        return stoneCount;
    }
    public int GetFoodCount()
    {
        return foodsList.Count;
    }
    public int GetEggCount()
    {
        return eggCount;
    }
    #endregion
}

public class JobInfo
{
    public BuildingBase targetBuilding = null;
    public ObjectBase targetObject = null;
}
