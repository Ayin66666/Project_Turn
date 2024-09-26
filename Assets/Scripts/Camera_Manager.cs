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


    // ī�޶� FOV ����
    public void Camera_FOVSetting(float fov, float change_Time)
    {

    }


    // ī�޶��� Follow target ����
    public void Camera_FollowSetting(GameObject target)
    {

    }


    // ī�޶��� LookAt ����
    public void Camera_LookAtSetting(GameObject target)
    {

    }


    // ī�޶� ��鸲 ȿ��
    public void Camera_Shack()
    {

    }
}
