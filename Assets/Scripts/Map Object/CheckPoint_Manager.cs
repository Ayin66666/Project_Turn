using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint_Manager : MonoBehaviour
{
    public static CheckPoint_Manager instance;

    // 플레이어 스테이터스
    [Header("=== Player Data ===")]
    [SerializeField] private int maxHp;
    [SerializeField] private int curHp;
    [SerializeField] private float physicalDefense;
    [SerializeField] private float magicalDefense;
    [SerializeField] private int physicalDamage;
    [SerializeField] private int magcialDamage;
    [SerializeField] private int criticalChance;
    [SerializeField] private float criticalMultiplier;
    [SerializeField] private int curUndersideGauge;
    [SerializeField] private int maxUndersideGauge;
    [SerializeField] private int maxAttackPoint;
    [SerializeField] private Vector2Int slotSpeed;


    // 스테이지 진행
    [Header("=== Stage Data ===")]
    [SerializeField] private bool isDoor1;
    [SerializeField] private bool isDoor2;
    [SerializeField] private bool isDoor3;
    [SerializeField] private bool isMonster1;
    [SerializeField] private bool isMonster2;
    [SerializeField] private bool isMonster3;


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
    }

    /// <summary>
    /// 체크포인트에서 상호작용하면 현제 플레이어의 데이터를 매니저 스크립트에 저장하는 기능 / 호출 시 자동 동작
    /// </summary>
    public void Player_DataUpdate()
    {

    }


    /// <summary>
    /// 체크포인트에서 상호작용하면 현제 스테이지의 데이터를 매니저 스크립트에 저장하는 기능 / 호출 시 자동 동작
    /// </summary>
    public void Stage_DataUpdate()
    {

    }


    /// <summary>
    /// 플레이어 사망 후 호출 시 플레이어를 최종 세이브포인트에서 부활 / 호출 시 자동 동작
    /// </summary>
    public void Respawn()
    {

    }
}
