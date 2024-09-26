using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Camera_Manager : MonoBehaviour
{
    public static Camera_Manager instance;

    [Header("===Camera Setting===")]
    [SerializeField] private GameObject cameraObject;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // 카메라 FOV 설정
    public void Camera_FOVSetting(float fov, float change_Time)
    {

    }


    // 카메라의 Follow target 셋팅
    public void Camera_FollowSetting(GameObject target)
    {

    }


    // 카메라의 LookAt 셋팅
    public void Camera_LookAtSetting(GameObject target)
    {

    }


    // 카메라 흔들림 효과
    public void Camera_Shack()
    {

    }
}
