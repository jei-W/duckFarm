using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingShop : MonoBehaviour
{
    Transform preInstallBuilding = null;
    World.BuildingType preInstallBuildingType;
    bool validInstallPosition = false; // 설치유효한 자리에 있는가?

    // Start is called before the first frame update
    void Start()
    {
        
    }

    const string GroundTag = "Ground";
    // Update is called once per frame
    void Update()
    {
        if ( preInstallBuilding != null )
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if ( Physics.Raycast(ray, out hit) && hit.transform.tag == GroundTag )
            {
                preInstallBuilding.position = hit.point;
                preInstallBuilding.gameObject.SetActive(true);

                validInstallPosition = true;
            }
            else
            {
                preInstallBuilding.gameObject.SetActive(false);

                validInstallPosition = false;
            }
        }
    }

    public bool OnMouseDown(Vector2 position)
    {
        if ( validInstallPosition == false )
            return false;

        World.GetInstance().BuildBuilding(preInstallBuildingType, preInstallBuilding.position);

        preInstallBuilding = null;
        InputSystemManager.Instance.UnregisterMouseDownEvent(OnMouseDown);
        return true;
    }
    
    public void OnClickCreateBuilding( string buildingType )
    {
        if ( Enum.TryParse<World.BuildingType>(buildingType, out preInstallBuildingType) == false)
        {
            Debug.LogError("BuildingType을 잘 못 입력했음.");
            return;
        }

        // 사전설치용 GameObject를 만든다.
        if ( preInstallBuilding != null )
            Destroy(preInstallBuilding.gameObject);

        preInstallBuilding = World.GetInstance().BuildPreInstallBuilding(preInstallBuildingType);

        if ( preInstallBuilding == null )
        {
            Debug.LogError("사전 설치 건물을 생성하지 못했음");
            return;
        }
        // 아마도 월드 어디 구석에서 생겼을 테니, 안보이게 숨긴다.
        // 이후 업데이트에서, 마우스가 지형에 피킹되면 보이게 한다.
        preInstallBuilding.gameObject.SetActive(false);
        
        // 최우선 순위로 마우스 다운 이벤트를 등록한다.
        InputSystemManager.Instance.RegisterMouseDownEvent(OnMouseDown, true);
    }
}
