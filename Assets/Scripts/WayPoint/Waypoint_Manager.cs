using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Waypoint_Manager : MonoBehaviour
{
    public static Waypoint_Manager instance;

    public Camera_WayPoint_Base camera_WayPoint_Base;

    [System.Serializable]
    struct Data
    {
        public int totalWayPoints;
        public GameObject waypointCanvas;
        public GameObject worldWaypoints;

        public List<WayPoint_Controller> wayPoint_Controller;
    }
    
    [SerializeField] Data data;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //camera_WayPoint_Base = Player_Manager.instnace.camera_WayPoint_Base;
        camera_WayPoint_Base = GameObject.Find("Main Camera").GetComponentInChildren<Camera_WayPoint_Base>();
    }

    /// <summary>
    /// 지정한 인덱스의 웨이포인트를 활성화하고, 나머지 웨이포인트를 비활성화 하는 기능
    /// </summary>
    /// <param name="isOn">활성화 여부</param>
    /// <param name="index">활성화 할 웨이포인트의 인덱스</param>
    public void Waypoint_Setting(bool isOn, int index)
    {
        if(isOn)
        {
            camera_WayPoint_Base.data.wayPoints.Add(data.wayPoint_Controller[index]);

            for (int i = 0; i < data.wayPoint_Controller.Count; i++)
            {
                data.wayPoint_Controller[i].gameObject.SetActive(false);
                data.wayPoint_Controller[i].wayPoint_UI.gameObject.SetActive(false);

                data.wayPoint_Controller[i].wayPoint_UI.gameObject.transform.SetParent(data.wayPoint_Controller[i].gameObject.transform);
            }

            data.wayPoint_Controller[index].gameObject.SetActive(true);
            data.wayPoint_Controller[index].wayPoint_UI.gameObject.SetActive(true);
        }
        else
        {
            for (int i = 0; i < data.wayPoint_Controller.Count; i++)
            {
                data.wayPoint_Controller[i].gameObject.SetActive(false);
                data.wayPoint_Controller[i].wayPoint_UI.gameObject.SetActive(false);

                data.wayPoint_Controller[i].wayPoint_UI.gameObject.transform.SetParent(data.wayPoint_Controller[i].gameObject.transform);
            }
        }

    }
}
