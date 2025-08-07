using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Manager : MonoBehaviour
{
    public static Player_Manager instnace;

    // 액션으로 제어
    // 턴제 턴을 플레이어 매니저가 관리
    // 현제 턴 + 버프지속 턴 계산으로 지속시간 관리
    // 합연산으로 버프 관리
    public System.Action buffAction;

    public enum State { World, Turn }
    public enum DamageType { Physical, Magical }
    public enum HitEffect { None, Groggy }
    public enum Status { MaxHp, CurHp, PhysicalDefense, MagicDefense, PhysicalDamage, MagcialDamage, CriticalChance, CriticalMultiplier, AttackPoint, SlotSpeed }
    
    [Header("=== Component ===")]
    public Camera_Manager cam_Manager; 
    public Player_World player_World;
    public Player_Turn player_Turn;
    public Player_UI player_UI;
    public TurnFight_Manager turnManger = null;
    public Camera_WayPoint_Controller waypointController;
    [SerializeField] private Turn_Sound sound_Turn;
    [SerializeField] private Turn_Sound sound_UI;


    // 데미지 UI
    [SerializeField] private GameObject damageObj;
    [SerializeField] private Collider damageSpawnPosCollider;
    [SerializeField] private GameObject hitVFX;
    [SerializeField] private Collider hitVFXCollier;


    [Header("=== State ===")]
    public State state;
    public bool isAttack;
    public bool canMove;
    public bool isDie;
    public bool isUndersideOn = false;



    [Header("=== Status ===")]
    // 체력 & 방어력
    public int maxHp;
    public int curHp;
    [SerializeField] private float physicalDefense;
    [SerializeField] private float magicalDefense;

    // 공격력
    [SerializeField] private int physicalDamage;
    [SerializeField] private int magcialDamage;

    // 치명타
    [SerializeField] private int criticalChance;
    [SerializeField] private float criticalMultiplier;

    // 이면 게이지
    [SerializeField] private int curUndersideGauge;
    [SerializeField] private int maxUndersideGauge;

    // 공격 포인트
    [SerializeField] private int attackPointRecovery;
    public int curAttackPoint;
    public int maxAttackPoint;

    // 슬롯 속도
    [SerializeField] private Vector2Int slotSpeed;


    [Header("===Cursor Setting===")]
    private bool isCursorLook = true;


    #region Property
    public float PhysicalDefense
    {
        get { return physicalDefense; }
        private set { physicalDefense = value; }
    }
    public float MagicDefense
    {
        get { return magicalDefense; }
        private set { magicalDefense = value; }
    }

    public int PhysicalDamage
    {
        get { return physicalDamage; }
        private set { physicalDamage = value; }
    }
    public int MagcialDamage
    {
        get { return magcialDamage; }
        private set { magcialDamage = value; }
    }

    public int CriticalChance
    {
        get { return criticalChance; }
        private set { criticalChance = value; }
    }
    public float CriticalMultiplier
    {
        get { return criticalMultiplier; }
        private set { criticalMultiplier = value; }
    }


    public Vector2Int SlotSpeed
    {
        get { return slotSpeed; }
        private set { slotSpeed = value; }
    }


    public int CurUndersideGauge
    {
        get { return curUndersideGauge; }
        private set { curUndersideGauge = value; }
    }
    public int MaxUndersideGauge
    {
        get { return maxUndersideGauge; }
        private set { maxUndersideGauge = value; }
    }


    #endregion


    public void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instnace != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instnace = this;
        }

        Cursor_Setting(isCursorLook);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            isCursorLook = !isCursorLook;
            Cursor_Setting(isCursorLook);
        }
    }

    /// <summary>
    /// 사운드 호출 - 턴 전투
    /// </summary>
    /// <param name="index"></param>
    public void Turn_Sound_Call(int index)
    {
        sound_Turn.SoundCall(index);
    }

    /// <summary>
    /// 사운드 호출 - UI
    /// </summary>
    /// <param name="index"></param>
    public void UI_Sound_Call(int index)
    {
        sound_UI.SoundCall(index);
    }

    public void Cursor_Setting(bool isLook)
    {
        // 이동 제어
        canMove = isLook;

        // 카메라 제어
        player_World.freeLookCam.m_XAxis.m_MaxSpeed = isLook ? 300 : 0f;
        player_World.freeLookCam.m_YAxis.m_MaxSpeed = isLook ? 2 : 0f;

        // 커서 제어
        Cursor.lockState = isLook ? CursorLockMode.Locked : CursorLockMode.None;
    }

    /// <summary>
    /// 플레이어 스테이터스 변화용
    /// </summary>
    /// <param name="status">스테이터스 종류</param>
    /// <param name="value">변화시킬 값</param>
    public void Status_Set(Status status, float value)
    {
        switch (status)
        {
            case Status.MaxHp:
                maxHp += (int)value;
                break;

            case Status.CurHp:
                curHp += (int)value;
                if (curHp >= maxHp) curHp = maxHp;
                break;

            case Status.PhysicalDefense:
                physicalDefense += value;
                break;

            case Status.MagicDefense:
                magicalDefense += value;
                break;

            case Status.PhysicalDamage:
                physicalDamage += (int)value;
                break;

            case Status.MagcialDamage:
                magcialDamage += (int)value;
                break;

            case Status.CriticalChance:
                criticalChance += (int)value;
                break;

            case Status.CriticalMultiplier:
                criticalMultiplier += value;
                break;

            case Status.AttackPoint: // 이거 cur max 나눠서 동작해야 하는 부분!
                curAttackPoint += (int)value;
                break;

            case Status.SlotSpeed:
                slotSpeed += new Vector2Int((int)value, (int)value);
                break;
        }
    }


    // 데미지 계산 -> 튜플 사용 (1개 이상의 값을 반환할 때 / C# 7.0 이상부터 가능!)
    public (int, bool) DamageCal(Attack_Base attack, int count)
    {
        // 데미지 공식 (기초 데미지 * 버프1 * 버프2 ... ) * 공격 배율 * 치명타 배율


        // 물 & 마 데미지 인풋
        int baseDamage = attack.damageType[count] == Attack_Base.DamageType.physical ? physicalDamage : magcialDamage;

        // 버프 계산


        // 공격 배율 계산
        float valueDamage = baseDamage * Random.Range(attack.damageValue[count].x, attack.damageValue[count].y);

        // 크리티컬 데미지 계산
        int ran = Random.Range(0, 100);
        valueDamage = ran <= criticalChance ? valueDamage *= criticalMultiplier : valueDamage;

        // 데미지 & 크리티컬 여부 반환
        return ((int)valueDamage, ran <= criticalChance ? true : false);
    }

    // 공격 포인트 회복
    public void Turn_AttackPointRecovery()
    {
        // 공격 포인트 회복
        curAttackPoint += attackPointRecovery;
        if(curAttackPoint > maxAttackPoint)
        {
            curAttackPoint = maxAttackPoint;
        }

        // UI 최신화
        Player_UI.instance.Turn_AttackPoint();
    }

    // 공격 포인트 소모 & 추가
    public void AttackPointAdd(int count)
    {
        // 어택 포인트 최신화
        curAttackPoint += count;
        if(curAttackPoint > maxAttackPoint)
        {
            curAttackPoint = maxAttackPoint;
        }

        // 에러 대비용
        if(curAttackPoint < 0)
        {
            curAttackPoint = 0;
            Debug.Log("뭔가 호출 값에 문제가 있음! / 계산 결과 0 이하가 나옴!");
        }

        // UI 최신화
        Player_UI.instance.Turn_AttackPoint();
    }


    // 필살기 게이지 추가 & 버튼 활성화
    public void UnderSideGaugeAdd(int addGauge)
    { 
        // 게이지 추가
        curUndersideGauge += addGauge;
        if(curUndersideGauge >= maxUndersideGauge)
        {
            curUndersideGauge = maxUndersideGauge;
        }
        
        if(curUndersideGauge < 0)
        {
            curUndersideGauge = 0;
        }

        // 게이지 UI 동작
        Player_UI.instance.UndersideGaugeUI(curUndersideGauge);

        // 버튼 활성화
        if (curUndersideGauge >= maxUndersideGauge && !isUndersideOn)
        {
            UnderSideButtonOn();
        }
    }

    public void UnderSideButtonOn()
    {
        if (curUndersideGauge >= maxUndersideGauge && !isUndersideOn)
        {
            isUndersideOn = true;
            Player_UI.instance.Turn_UndersideButtonOn(true);
        }
    }

    /// <summary>
    /// 필살기 활성화 버튼 클릭 시 동작 -> 게이지 초기화 / 스킬 지급 / 버튼 비활성화
    /// </summary>
    public void UnderisdeButtonClick()
    {
        isUndersideOn = false;
        UnderSideGaugeAdd(-999);

        Player_UI.instance.Turn_UndersideSkillOn(true);
        Player_UI.instance.Turn_UndersideButtonOn(false);
    }

    /// <summary>
    /// 공격 계수 중간 기준 데미지 산출 기능 - 1번 중간 2번 최소 3번 최대
    /// </summary>
    /// <param name="attackData">플레이어 공격 데이터</param>
    public (int,int,int) Turn_WinningCheck(Attack_Base attackData)
    {
        // 최소 데미지 계산
        int minDamage =
            (int)(attackData.damageValue[0].x *
            (attackData.damageType[0] == Attack_Base.DamageType.physical ? physicalDamage : magcialDamage));

        // 최대 데미지 계산
        int maxDamage =
            (int)(attackData.damageValue[^1].y *
            (attackData.damageType[^1] == Attack_Base.DamageType.physical ? physicalDamage : magcialDamage));

        // 중간 데미지 계산
        int mmDamage = (int)Mathf.Lerp(minDamage, maxDamage, 0.5f);

        return (mmDamage, minDamage, maxDamage);
    }

    // 피격 데미지
    public void Take_Damage(int damage, DamageType type, bool isCritical)
    {
        // 사망 체크
        if(isDie)
        {
            return;
        }

        // 카메라 흔들림 효과
        Camera_Manager.instance.Turn_CameraShack(5f, 0.3f);

        // 데미지 계산 & UI 호출
        int calDamage = type == DamageType.Physical ? (int)(damage * physicalDefense) : (int)(damage * magicalDefense);
        Turn_HitDamageUI(isCritical, calDamage);
        curHp -= calDamage;

        // 체력바 UI
        player_UI.HpBar();

        // 피격 이펙트
        HitVFX();

        // 피격 아웃라인 효과
        Player_UI.instance.TurnFight_Ouutline_SpeedUp();

        // 사망 체크
        if (curHp <= 0 && !isDie)
        {
            isDie = true;
            //player_Turn.Turn_End(Player_Turn.EndType.Lose);
        }
        else
        {
            // 피격 이펙트 -> 이거 공격에 따라 타입 나누는것도?
            HitAnimation();
        }
    }
    private void HitAnimation()
    {
        // Sound
        Turn_Sound_Call(5);

        // Animation
        player_Turn.anim.SetTrigger("Hit");
    }

    /// <summary>
    /// 피격 이펙트
    /// </summary>
    /// <param name="type"></param>
    private void HitVFX()
    {
        Instantiate(hitVFX, HitVFXPos(), Quaternion.identity);
    }

    // 피격 이펙트 소환 위치 계산
    private Vector3 HitVFXPos()
    {
        Vector3 originPosition = hitVFXCollier.transform.position;

        // 콜라이더의 사이즈를 가져오는 bound.size 사용
        float range_X = hitVFXCollier.bounds.size.x;
        float range_Y = hitVFXCollier.bounds.size.y;

        range_X = Random.Range((range_X / 2) * -1, range_X / 2);
        range_Y = Random.Range((range_Y / 2) * -1, range_Y / 2);
        Vector3 RandomPostion = new Vector3(range_X, range_Y);

        Vector3 respawnPosition = originPosition + RandomPostion;
        return respawnPosition;
    }

    // 피격 데미지 UI
    public void Turn_HitDamageUI(bool isCritical, int damage)
    {
        GameObject obj = Instantiate(damageObj, DamageUIPos(), Quaternion.identity);
        obj.GetComponent<Damage_UI>().DamageUI_Setting(isCritical ? Damage_UI.DamageType.Critical : Damage_UI.DamageType.Normal, damage);
    }

    // 피격 데미지 UI 소환 위치 계산
    private Vector3 DamageUIPos()
    {
        Vector3 originPosition = damageSpawnPosCollider.transform.position;

        // 콜라이더의 사이즈를 가져오는 bound.size 사용
        float range_X = damageSpawnPosCollider.bounds.size.x;
        float range_Y = damageSpawnPosCollider.bounds.size.y;

        range_X = Random.Range((range_X / 2) * -1, range_X / 2);
        range_Y = Random.Range((range_Y / 2) * -1, range_Y / 2);
        Vector3 RandomPostion = new Vector3(range_X, range_Y);

        Vector3 respawnPosition = originPosition + RandomPostion;
        return respawnPosition;
    }


    // 전투 시작 이동 & 턴제 플레이어 On
    public void Turn_Fight_Start(Transform movePos, TurnFight_Manager manager)
    {
        // 전투 시작 스테이터스 초기화
        // curUndersideGauge = 0; // -> 이 부분 초기화 할지 말지는 이야기 해볼 것!
        curAttackPoint = 8;
        maxAttackPoint = 8;

        turnManger = manager;
        player_Turn.gameObject.transform.position = movePos.position;
        player_World.gameObject.SetActive(false);
        player_Turn.gameObject.SetActive(true);
    }


    // 전투 종료 이동 & 월드 플레이어 On
    public void Turn_Fight_End()
    {
        player_World.gameObject.SetActive(true);
        player_Turn.gameObject.SetActive(false);
    }


    /// <summary>
    /// 플레이어 위치 셋팅
    /// </summary>
    /// <param name="pos">이동할 위치</param>
    public void World_PlayerPos_Setting(Vector3 pos)
    {
        player_World.transform.position = pos;
    }
}
