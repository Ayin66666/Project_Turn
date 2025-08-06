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

    [Header("=== 스테이지 셋팅 ===")]
    [SerializeField] private bool haveStartDialog;
    [SerializeField] private bool haveStartWayPoint;
    [SerializeField] private bool haveNextStage;
    [SerializeField] private bool haveStageBGM;


    [Header("=== 스테이지 이름 ===")]
    [SerializeField] private string curStageName;
    [SerializeField] private string nextStageName;


    [Header("=== 스테이지 사운드 ===")]
    [SerializeField] private AudioSource backgroundAudio;
    [SerializeField] private AudioClip[] backgroundSounds;


    [Header("===스테이지 데이터===")]
    [SerializeField] private List<RoomData> roomDatas;


    [Header("=== 프리팹 ===")]
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

        // 웨이포인트 재설정
    }

    private void Start()
    {
        // 씬 시작 셋팅 동작
        Start_Setting();
    }

    private void Start_Setting()
    {
        // 플레이어 중력 동작
        Player_Manager.instnace.player_World.useGravity = true;

        // 페이드 종료
        Player_UI.instance.Stage_StartEnd(true, curStageName);

        // 웨이포인트 셋팅
        Player_Manager.instnace.waypointController.WayPoint_Setting();

        // 다이얼로그
        if (haveStartDialog)
        {
            Dialog_Manager.instance.Dialog(0);
        }

        // 웨이포인트
        if (haveStartWayPoint)
        {
            Waypoint_Manager.instance.Waypoint_Setting(true, 0);
        }

        // 사운드
        if(haveStageBGM)
        {
            backgroundAudio.clip = backgroundSounds[0];
            backgroundAudio.Play();
        }

        Player_Manager.instnace.canMove = true;
    }


    /// <summary>
    /// BGM 변경 기능
    /// </summary>
    /// <param name="index">변경할 BGM 인덱스</param>
    public void BGM_Setting(int index)
    {
        backgroundAudio.clip = backgroundSounds[index];
        backgroundAudio.Play();
    }

    /// <summary>
    /// 사운드 ON/Off
    /// </summary>
    /// <param name="isOn">사운드 ON/Off</param>
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
    /// Object_Wall 에 Use 를 호출하여 문을 여는 기능 / 도어 배치 시 콜라이더랑 컨트롤 패널 비활성화 할것!
    /// </summary>
    /// <param name="doorIndex">문 인덱스</param>
    public void DoorOpen(int doorIndex)
    {
        door_Prefabs[doorIndex].Use();
    }


    /// <summary>
    /// 룸 클리어 여부 체크
    /// </summary>
    /// <param name="roomIndex">룸 인덱스</param>
    public void Room_Check(int roomIndex)
    {
        roomDatas[roomIndex].isClear = true;
    }


    /// <summary>
    /// 사망 시 체크포인트로 돌아가는 기능 - 미구현
    /// </summary>
    public void Stage_Reset()
    {
        // 가지고 있는 데이터 체크 후 동작
    }


    /// <summary>
    /// 다음 스테이지로 넘어가는 or 게임 종료 포탈
    /// </summary>
    /// <param name="isOn">On/Off</param>
    public void NextStagePortal(bool isOn)
    {
        portal_Prefab.SetActive(isOn);
    }
}
