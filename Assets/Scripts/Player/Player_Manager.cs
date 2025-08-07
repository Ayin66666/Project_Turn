using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Manager : MonoBehaviour
{
    public static Player_Manager instnace;

    // �׼����� ����
    // ���� ���� �÷��̾� �Ŵ����� ����
    // ���� �� + �������� �� ������� ���ӽð� ����
    // �տ������� ���� ����
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


    // ������ UI
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
    // ü�� & ����
    public int maxHp;
    public int curHp;
    [SerializeField] private float physicalDefense;
    [SerializeField] private float magicalDefense;

    // ���ݷ�
    [SerializeField] private int physicalDamage;
    [SerializeField] private int magcialDamage;

    // ġ��Ÿ
    [SerializeField] private int criticalChance;
    [SerializeField] private float criticalMultiplier;

    // �̸� ������
    [SerializeField] private int curUndersideGauge;
    [SerializeField] private int maxUndersideGauge;

    // ���� ����Ʈ
    [SerializeField] private int attackPointRecovery;
    public int curAttackPoint;
    public int maxAttackPoint;

    // ���� �ӵ�
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
    /// ���� ȣ�� - �� ����
    /// </summary>
    /// <param name="index"></param>
    public void Turn_Sound_Call(int index)
    {
        sound_Turn.SoundCall(index);
    }

    /// <summary>
    /// ���� ȣ�� - UI
    /// </summary>
    /// <param name="index"></param>
    public void UI_Sound_Call(int index)
    {
        sound_UI.SoundCall(index);
    }

    public void Cursor_Setting(bool isLook)
    {
        // �̵� ����
        canMove = isLook;

        // ī�޶� ����
        player_World.freeLookCam.m_XAxis.m_MaxSpeed = isLook ? 300 : 0f;
        player_World.freeLookCam.m_YAxis.m_MaxSpeed = isLook ? 2 : 0f;

        // Ŀ�� ����
        Cursor.lockState = isLook ? CursorLockMode.Locked : CursorLockMode.None;
    }

    /// <summary>
    /// �÷��̾� �������ͽ� ��ȭ��
    /// </summary>
    /// <param name="status">�������ͽ� ����</param>
    /// <param name="value">��ȭ��ų ��</param>
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

            case Status.AttackPoint: // �̰� cur max ������ �����ؾ� �ϴ� �κ�!
                curAttackPoint += (int)value;
                break;

            case Status.SlotSpeed:
                slotSpeed += new Vector2Int((int)value, (int)value);
                break;
        }
    }


    // ������ ��� -> Ʃ�� ��� (1�� �̻��� ���� ��ȯ�� �� / C# 7.0 �̻���� ����!)
    public (int, bool) DamageCal(Attack_Base attack, int count)
    {
        // ������ ���� (���� ������ * ����1 * ����2 ... ) * ���� ���� * ġ��Ÿ ����


        // �� & �� ������ ��ǲ
        int baseDamage = attack.damageType[count] == Attack_Base.DamageType.physical ? physicalDamage : magcialDamage;

        // ���� ���


        // ���� ���� ���
        float valueDamage = baseDamage * Random.Range(attack.damageValue[count].x, attack.damageValue[count].y);

        // ũ��Ƽ�� ������ ���
        int ran = Random.Range(0, 100);
        valueDamage = ran <= criticalChance ? valueDamage *= criticalMultiplier : valueDamage;

        // ������ & ũ��Ƽ�� ���� ��ȯ
        return ((int)valueDamage, ran <= criticalChance ? true : false);
    }

    // ���� ����Ʈ ȸ��
    public void Turn_AttackPointRecovery()
    {
        // ���� ����Ʈ ȸ��
        curAttackPoint += attackPointRecovery;
        if(curAttackPoint > maxAttackPoint)
        {
            curAttackPoint = maxAttackPoint;
        }

        // UI �ֽ�ȭ
        Player_UI.instance.Turn_AttackPoint();
    }

    // ���� ����Ʈ �Ҹ� & �߰�
    public void AttackPointAdd(int count)
    {
        // ���� ����Ʈ �ֽ�ȭ
        curAttackPoint += count;
        if(curAttackPoint > maxAttackPoint)
        {
            curAttackPoint = maxAttackPoint;
        }

        // ���� ����
        if(curAttackPoint < 0)
        {
            curAttackPoint = 0;
            Debug.Log("���� ȣ�� ���� ������ ����! / ��� ��� 0 ���ϰ� ����!");
        }

        // UI �ֽ�ȭ
        Player_UI.instance.Turn_AttackPoint();
    }


    // �ʻ�� ������ �߰� & ��ư Ȱ��ȭ
    public void UnderSideGaugeAdd(int addGauge)
    { 
        // ������ �߰�
        curUndersideGauge += addGauge;
        if(curUndersideGauge >= maxUndersideGauge)
        {
            curUndersideGauge = maxUndersideGauge;
        }
        
        if(curUndersideGauge < 0)
        {
            curUndersideGauge = 0;
        }

        // ������ UI ����
        Player_UI.instance.UndersideGaugeUI(curUndersideGauge);

        // ��ư Ȱ��ȭ
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
    /// �ʻ�� Ȱ��ȭ ��ư Ŭ�� �� ���� -> ������ �ʱ�ȭ / ��ų ���� / ��ư ��Ȱ��ȭ
    /// </summary>
    public void UnderisdeButtonClick()
    {
        isUndersideOn = false;
        UnderSideGaugeAdd(-999);

        Player_UI.instance.Turn_UndersideSkillOn(true);
        Player_UI.instance.Turn_UndersideButtonOn(false);
    }

    /// <summary>
    /// ���� ��� �߰� ���� ������ ���� ��� - 1�� �߰� 2�� �ּ� 3�� �ִ�
    /// </summary>
    /// <param name="attackData">�÷��̾� ���� ������</param>
    public (int,int,int) Turn_WinningCheck(Attack_Base attackData)
    {
        // �ּ� ������ ���
        int minDamage =
            (int)(attackData.damageValue[0].x *
            (attackData.damageType[0] == Attack_Base.DamageType.physical ? physicalDamage : magcialDamage));

        // �ִ� ������ ���
        int maxDamage =
            (int)(attackData.damageValue[^1].y *
            (attackData.damageType[^1] == Attack_Base.DamageType.physical ? physicalDamage : magcialDamage));

        // �߰� ������ ���
        int mmDamage = (int)Mathf.Lerp(minDamage, maxDamage, 0.5f);

        return (mmDamage, minDamage, maxDamage);
    }

    // �ǰ� ������
    public void Take_Damage(int damage, DamageType type, bool isCritical)
    {
        // ��� üũ
        if(isDie)
        {
            return;
        }

        // ī�޶� ��鸲 ȿ��
        Camera_Manager.instance.Turn_CameraShack(5f, 0.3f);

        // ������ ��� & UI ȣ��
        int calDamage = type == DamageType.Physical ? (int)(damage * physicalDefense) : (int)(damage * magicalDefense);
        Turn_HitDamageUI(isCritical, calDamage);
        curHp -= calDamage;

        // ü�¹� UI
        player_UI.HpBar();

        // �ǰ� ����Ʈ
        HitVFX();

        // �ǰ� �ƿ����� ȿ��
        Player_UI.instance.TurnFight_Ouutline_SpeedUp();

        // ��� üũ
        if (curHp <= 0 && !isDie)
        {
            isDie = true;
            //player_Turn.Turn_End(Player_Turn.EndType.Lose);
        }
        else
        {
            // �ǰ� ����Ʈ -> �̰� ���ݿ� ���� Ÿ�� �����°͵�?
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
    /// �ǰ� ����Ʈ
    /// </summary>
    /// <param name="type"></param>
    private void HitVFX()
    {
        Instantiate(hitVFX, HitVFXPos(), Quaternion.identity);
    }

    // �ǰ� ����Ʈ ��ȯ ��ġ ���
    private Vector3 HitVFXPos()
    {
        Vector3 originPosition = hitVFXCollier.transform.position;

        // �ݶ��̴��� ����� �������� bound.size ���
        float range_X = hitVFXCollier.bounds.size.x;
        float range_Y = hitVFXCollier.bounds.size.y;

        range_X = Random.Range((range_X / 2) * -1, range_X / 2);
        range_Y = Random.Range((range_Y / 2) * -1, range_Y / 2);
        Vector3 RandomPostion = new Vector3(range_X, range_Y);

        Vector3 respawnPosition = originPosition + RandomPostion;
        return respawnPosition;
    }

    // �ǰ� ������ UI
    public void Turn_HitDamageUI(bool isCritical, int damage)
    {
        GameObject obj = Instantiate(damageObj, DamageUIPos(), Quaternion.identity);
        obj.GetComponent<Damage_UI>().DamageUI_Setting(isCritical ? Damage_UI.DamageType.Critical : Damage_UI.DamageType.Normal, damage);
    }

    // �ǰ� ������ UI ��ȯ ��ġ ���
    private Vector3 DamageUIPos()
    {
        Vector3 originPosition = damageSpawnPosCollider.transform.position;

        // �ݶ��̴��� ����� �������� bound.size ���
        float range_X = damageSpawnPosCollider.bounds.size.x;
        float range_Y = damageSpawnPosCollider.bounds.size.y;

        range_X = Random.Range((range_X / 2) * -1, range_X / 2);
        range_Y = Random.Range((range_Y / 2) * -1, range_Y / 2);
        Vector3 RandomPostion = new Vector3(range_X, range_Y);

        Vector3 respawnPosition = originPosition + RandomPostion;
        return respawnPosition;
    }


    // ���� ���� �̵� & ���� �÷��̾� On
    public void Turn_Fight_Start(Transform movePos, TurnFight_Manager manager)
    {
        // ���� ���� �������ͽ� �ʱ�ȭ
        // curUndersideGauge = 0; // -> �� �κ� �ʱ�ȭ ���� ������ �̾߱� �غ� ��!
        curAttackPoint = 8;
        maxAttackPoint = 8;

        turnManger = manager;
        player_Turn.gameObject.transform.position = movePos.position;
        player_World.gameObject.SetActive(false);
        player_Turn.gameObject.SetActive(true);
    }


    // ���� ���� �̵� & ���� �÷��̾� On
    public void Turn_Fight_End()
    {
        player_World.gameObject.SetActive(true);
        player_Turn.gameObject.SetActive(false);
    }


    /// <summary>
    /// �÷��̾� ��ġ ����
    /// </summary>
    /// <param name="pos">�̵��� ��ġ</param>
    public void World_PlayerPos_Setting(Vector3 pos)
    {
        player_World.transform.position = pos;
    }
}
