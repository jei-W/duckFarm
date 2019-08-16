using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 오브젝트를 보관할 수 있는 건물들..
public class PocketBuilding : BuildingBase
{
    // 최대 수용량...
    int MaximumCapacity = 10;

    // 현재 수용량
    int CurrentCapacity = 0;

    // PocketBuilding은 문의 위치를 알리는 빈 오브젝트가 필수.
    public Transform Door = null;

    protected Dictionary<string, ObjectBase> Objects = new Dictionary<string, ObjectBase>();

    public void Start()
    {
        if ( Door == null )
        {
            Door = transform.Find("Door");
        }

        if ( Door == null )
        {
            Debug.LogError("PocketBuilding은 Door위치 오브젝트가 필수!");
        }
    }
    // 오브젝트를 들여보낸다?
    public bool AskEnterable()
    {
        if( MaximumCapacity <= CurrentCapacity )
            return false;
        else return true;
    }

    public virtual void EnterObject( ObjectBase targetObject )
    {
        if( AskEnterable() == false )
            return;

        if ( Objects.ContainsKey(targetObject.ObjectID))
        {
            Debug.Log($"{ObjectID} 이미 들어 왔는데? : {targetObject.ObjectID }");
        }
        Objects.Add(targetObject.ObjectID, targetObject);

        Debug.Log("Enter");
        targetObject.transform.parent = this.transform;
        var meshRenderer = targetObject.GetComponentInChildren<MeshRenderer>() as MeshRenderer;
        if( meshRenderer != null )
        {
            meshRenderer.enabled = false;
        }

        CurrentCapacity++;
    }

    public void ExitObject(string objectID)
    {
        if ( Objects.ContainsKey(objectID) == false )
            return;

        var targetObject = Objects[objectID];
        targetObject.transform.parent = null;

        var meshRenderer = targetObject.GetComponentInChildren<MeshRenderer>() as MeshRenderer;
        if ( meshRenderer != null )
        {
            meshRenderer.enabled = true;
        }

        CurrentCapacity--;
        Objects.Remove(objectID);
    }

    public virtual void ShowObjectsList()
    {
        foreach(var innerObject in Objects)
        {
            Debug.Log(innerObject.Key);
        }
    }
}
