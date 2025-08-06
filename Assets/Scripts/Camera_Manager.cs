using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Easing.Tweening;
using System;
using Unity.Burst.Intrinsics;


public class Camera_Manager : MonoBehaviour
{
    public static Camera_Manager instance;

    [Header("=== Turn Camera Setting ===")]
    public CameraType camType;
    public enum CameraType 
    { 
        None, Select, ExchangeA, UpDownA, UpDownB, ExplosionA, ExplosionB,
        LaserA, LaserB, LightningA, LightningB, 
        UndersideA, UndersideB, UndersideC
    }

    public enum CameraType_Boss
    {
        None, Combo, BladeRush, Strike, QuadrSlash, UpwardSlash, SuperSlash
    }

    [Header("=== Component ===")]
    [SerializeField] private CinemachineBrain brainCam;
    [SerializeField] private CinemachineFreeLook worldCam;
    private Coroutine shakeCoroutine;


    [Header("=== Player Cameras ===")]
    public GameObject mainCam;
    [SerializeField] private List<CinemachineVirtualCamera> turnEffectCams;
    [SerializeField] private List<CinemachineBasicMultiChannelPerlin> cams;

    [SerializeField] private GameObject cam_None;
    [SerializeField] private GameObject cam_select;
    [SerializeField] private GameObject cam_ExchangeA;

    [SerializeField] private GameObject cam_UpDownA;
    [SerializeField] private GameObject cam_UpDownB;

    [SerializeField] private GameObject cam_UndersideA;
    [SerializeField] private GameObject cam_UndersideB;
    [SerializeField] private GameObject cam_UndersideC;

    [SerializeField] private GameObject cam_ExplosionA;
    [SerializeField] private GameObject cam_ExplosionB;

    [SerializeField] private GameObject cam_LaserA;
    [SerializeField] private GameObject cam_LaserB;

    [SerializeField] private GameObject cam_LightningA;
    [SerializeField] private GameObject cam_LightningB;

    [SerializeField] private List<GameObject> cameraList_Player;
    [SerializeField] private List<GameObject> cameraList_Boss;


    [Header("=== Enemy Cameras ===")]
    [SerializeField] private GameObject[] cam_Combo;
    [SerializeField] private GameObject[] cam_BladeRush;
    [SerializeField] private GameObject[] cam_Strike;
    [SerializeField] private GameObject[] cam_JumpSlash;
    [SerializeField] private GameObject[] cam_QuadrSlash;
    [SerializeField] private GameObject[] cam_UpwardSlash;
    [SerializeField] private GameObject[] cam_SuperSlash;


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

        // ī�޶� ����
        for (int i = 0; i < turnEffectCams.Count; i++)
        {
            cams.Add(turnEffectCams[i].GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>());
        }
    }

    /// <summary>
    /// ī�޶� ��ȯ �ӵ� ����
    /// </summary>
    /// <param name="changeSpeed">��ȯ �ӵ��� - ����Ʈ 0.5f</param>
    public void BrainCam_Setting(float changeSpeed)
    {
        brainCam.m_DefaultBlend.m_Time = changeSpeed;
    }

    /// <summary>
    /// �÷��̾�� ķ ����
    /// </summary>
    /// <param name="type"></param>
    /// <param name="changeSpeed"></param>
    public void Camera_Setting(CameraType type, float changeSpeed)
    {
        // ��ȯ �ӵ� ����
        brainCam.m_DefaultBlend.m_Time = changeSpeed;

        // ��ü ī�޶� ����
        for (int i = 0; i < cameraList_Player.Count; i++)
        {
            cameraList_Player[i].SetActive(false);
        }

        // ���� ī�޶� Ȱ��ȭ
        switch (type)
        {
            case CameraType.None:
                cam_None.SetActive(true);
                break;

            case CameraType.Select:
                cam_select.SetActive(true);
                break;

            case CameraType.ExchangeA:
                cam_ExchangeA.SetActive(true);
                break;

            case CameraType.UpDownA:
                cam_UpDownA.SetActive(true);
                break;

            case CameraType.UpDownB:
                cam_UpDownB.SetActive(true);
                break;

            case CameraType.ExplosionA:
                cam_ExplosionA.SetActive(true);
                break;

            case CameraType.ExplosionB:
                cam_ExplosionB.SetActive(true);
                break;

            case CameraType.LaserA:
                cam_LaserA.SetActive(true);
                break;

            case CameraType.LaserB:
                cam_LaserB.SetActive(true);
                break;

            case CameraType.LightningA:
                cam_LightningA.SetActive(true);
                break;

            case CameraType.LightningB:
                cam_LightningB.SetActive(true);
                break;

            case CameraType.UndersideA:
                cam_UndersideA.SetActive(true);
                break;

            case CameraType.UndersideB:
                cam_UndersideB.SetActive(true);
                break;

            case CameraType.UndersideC:
                cam_UndersideC.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// ������ ķ ����
    /// </summary>
    /// <param name="type">��ȯ Ÿ��</param>
    /// <param name="index">��ȯ Ÿ���� �ε���</param>
    /// <param name="changeSpeed">��ȯ �ӵ�</param>
    public void Camera_Setting(CameraType_Boss type, int index, float changeSpeed)
    {
        // ��ȯ �ӵ� ����
        brainCam.m_DefaultBlend.m_Time = changeSpeed;

        // ��ü ī�޶� ����
        for (int i = 0; i < cameraList_Player.Count; i++)
        {
            cameraList_Player[i].SetActive(false);
        }

        for (int i = 0; i < cameraList_Boss.Count; i++)
        {
            cameraList_Boss[i].SetActive(false);
        }

        switch (type)
        {
            case CameraType_Boss.None:
                cam_None.SetActive(true);
                break;

            case CameraType_Boss.Combo:
                cam_Combo[index].SetActive(true);
                break;

            case CameraType_Boss.BladeRush:
                cam_BladeRush[index].SetActive(true);
                break;

            case CameraType_Boss.Strike:
                cam_Strike[index].SetActive(true);
                break;

            case CameraType_Boss.QuadrSlash:
                cam_QuadrSlash[index].SetActive(true);
                break;

            case CameraType_Boss.UpwardSlash:
                cam_UpwardSlash[index].SetActive(true);
                break;

            case CameraType_Boss.SuperSlash:
                cam_SuperSlash[index].SetActive(true);
                break;
        }
    }

    /*
    // ī�޶� FOV ���� -> �ٽ� �۾� �ʿ���
    public void Camera_FOVSetting(float targetFOV, float changeSpeed)
    {
        if(fovCoroutine != null)
        {
            StopCoroutine(fovCoroutine);
        }
        fovCoroutine = StartCoroutine(Camera_FOVSettingCall(targetFOV, changeSpeed));
    }

    // ī�޶� FOV
    private IEnumerator Camera_FOVSettingCall(float targetFOV, float changeSpeed)
    {
        float start = cinemachine.m_Lens.FieldOfView;
        float end = targetFOV;
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * changeSpeed;
            cinemachine.m_Lens.FieldOfView = Mathf.Lerp(start, end, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        cinemachine.m_Lens.FieldOfView = targetFOV;
    }
    */
    
    // ī�޶� ��鸲 ȿ��
    public void Turn_CameraShack(float intensity, float time)
    {

        if(shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            for (int i = 0; i < cams.Count; i++)
            {
                cams[i].m_AmplitudeGain = 0;
            }
        }

        shakeCoroutine = StartCoroutine(Turn_CameraShake(intensity, time));
    }
    
    /// <summary>
    /// ī�޶� ���� ȿ��
    /// </summary>
    /// <param name="intensity">��鸲 ����</param>
    /// <param name="time">��鸱 �ð�</param>
    /// <returns></returns>
    private IEnumerator Turn_CameraShake(float intensity, float time)
    {
        float power = intensity;
        float timer = time;

        // Shake Cam
        while (power > 0)
        {
            for (int i = 0; i < cams.Count; i++)
            {
                cams[i].m_AmplitudeGain = power;
            }

            power -= Time.deltaTime / timer;
            yield return null;
        }

        // Reset
        for (int i = 0; i < cams.Count; i++)
        {
            cams[i].m_AmplitudeGain = 0;
        }
    }
}
