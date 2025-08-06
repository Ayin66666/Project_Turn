using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomData
{
    public int roomIndex;
    public int bgmIndex;
    public bool isClear;
    public TurnFight_Manager roomManager;
}


public class Stage_Manager : MonoBehaviour
{
    public static Stage_Manager instance;

    [Header("=== �������� ���� ===")]
    [SerializeField] private bool haveStartDialog;
    [SerializeField] private bool haveStartWayPoint;
    [SerializeField] private bool haveNextStage;
    [SerializeField] private bool haveStageBGM;


    [Header("=== �������� �̸� ===")]
    [SerializeField] private string curStageName;
    [SerializeField] private string nextStageName;


    [Header("=== �������� ���� ===")]
    [SerializeField] private AudioSource backgroundAudio;
    [SerializeField] private AudioClip[] backgroundSounds;


    [Header("===�������� ������===")]
    [SerializeField] private List<RoomData> roomDatas;


    [Header("=== ������ ===")]
    [SerializeField] private GameObject portal_Prefab;
    [SerializeField] private Object_Wall[] door_Prefabs;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        backgroundAudio = GetComponent<AudioSource>();

        // ��������Ʈ �缳��
    }

    private void Start()
    {
        // �� ���� ���� ����
        Start_Setting();
    }

    private void Start_Setting()
    {
        // �÷��̾� �߷� ����
        Player_Manager.instnace.player_World.useGravity = true;

        // ���̵� ����
        Player_UI.instance.Stage_StartEnd(true, curStageName);

        // ��������Ʈ ����
        Player_Manager.instnace.waypointController.WayPoint_Setting();

        // ���̾�α�
        if (haveStartDialog)
        {
            Dialog_Manager.instance.Dialog(0);
        }

        // ��������Ʈ
        if (haveStartWayPoint)
        {
            Waypoint_Manager.instance.Waypoint_Setting(true, 0);
        }

        // ����
        if(haveStageBGM)
        {
            backgroundAudio.clip = backgroundSounds[0];
            backgroundAudio.Play();
        }

        Player_Manager.instnace.canMove = true;
    }


    /// <summary>
    /// BGM ���� ���
    /// </summary>
    /// <param name="index">������ BGM �ε���</param>
    public void BGM_Setting(int index)
    {
        backgroundAudio.clip = backgroundSounds[index];
        backgroundAudio.Play();
    }

    /// <summary>
    /// ���� ON/Off
    /// </summary>
    /// <param name="isOn">���� ON/Off</param>
    public void BGM_OnOff(bool isOn)
    {
        if(isOn)
        {
            backgroundAudio.Play();

        }
        else
        {
            backgroundAudio.Pause();

        }
    }

    /// <summary>
    /// Object_Wall �� Use �� ȣ���Ͽ� ���� ���� ��� / ���� ��ġ �� �ݶ��̴��� ��Ʈ�� �г� ��Ȱ��ȭ �Ұ�!
    /// </summary>
    /// <param name="doorIndex">�� �ε���</param>
    public void DoorOpen(int doorIndex)
    {
        door_Prefabs[doorIndex].Use();
    }


    /// <summary>
    /// �� Ŭ���� ���� üũ
    /// </summary>
    /// <param name="roomIndex">�� �ε���</param>
    public void Room_Check(int roomIndex)
    {
        roomDatas[roomIndex].isClear = true;
    }


    /// <summary>
    /// ��� �� üũ����Ʈ�� ���ư��� ��� - �̱���
    /// </summary>
    public void Stage_Reset()
    {
        // ������ �ִ� ������ üũ �� ����
    }


    /// <summary>
    /// ���� ���������� �Ѿ�� or ���� ���� ��Ż
    /// </summary>
    /// <param name="isOn">On/Off</param>
    public void NextStagePortal(bool isOn)
    {
        portal_Prefab.SetActive(isOn);
    }
}
