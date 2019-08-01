﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 오브젝트를 보관할 수 있는 건물들..
public class PocketBuilding : BuildingBase
{
    // 최대 수용량...
    int MaximumCapacity = 10;

    // 현재 수용량
    int CurrentCapacity = 0;

    protected Dictionary<string, ObjectBase> Objects = new Dictionary<string, ObjectBase>();
    
    // 오브젝트를 들여보낸다?
    public bool AskEnterable()
    {
        if( MaximumCapacity <= CurrentCapacity )
            return false;
        else return true;
    }

    public virtual void EnterObject( ObjectBase targetObject )
    {
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

        var meshRenderer = targetObject.GetComponent<MeshRenderer>() as MeshRenderer;
        if ( meshRenderer != null )
        {
            meshRenderer.enabled = true;
        }

        CurrentCapacity--;
    }

    public virtual void ShowObjectsList()
    {
        foreach(var innerObject in Objects)
        {
            Debug.Log(innerObject.Key);
        }
    }
}
