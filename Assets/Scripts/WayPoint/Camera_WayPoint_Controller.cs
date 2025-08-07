using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_WayPoint_Controller : Camera_WayPoint_Base
{
    private void Awake()
    {
        WayPoint_Setting();
    }

    private void Update()
    {
        if (data.wayPoints != null && data.wayPoints.Count > 0)
        {
            UpdateUI();
        }
    }

    public void WayPoint_Setting()
    {
        data.wayPoints.Clear();
        data.worldWayPoints = GameObject.Find("WorldWayPoints");
    }

    private void UpdateUI()
    {
        foreach (WayPoint_Controller wayPoint in data.wayPoints)
        {
            wayPoint.wayPoint_Base.image.transform.position = UI_Image_Position(wayPoint.wayPoint_Base);
            wayPoint.wayPoint_Base.text.text = WayPointDistance(wayPoint.wayPoint_Base) + "M";
        }    
    }
}
