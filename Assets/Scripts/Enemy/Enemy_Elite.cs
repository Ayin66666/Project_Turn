using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Elite : Enemy_Base
{
    [Header("=== Enemy Setting ===")]
    [SerializeField] private AttackPattern curPattern;
    private enum AttackPattern { None, PatternA, PatternB, PatternC, PatternD, PatternE }


    [Header("===Charged Attack Setting===")]
    [SerializeField] private List<Attack_Pattern> chargedPattern;
    [SerializeField] private int chargedAttackCount;
    private int curChargedAttackCount;


    [Header("===Attack Pattern===")]
    private int[,] patternValueList = new int[3, 3] 
    { { 0, 0, 0 }, { 0, 1, 0 }, { 1, 1, 0 } };

    private int[,] patternValueChargedList = new int[2, 3] 
    { { 2, 1, 0 }, { 2, 1, 1 } };

    delegate void PATTERNFUNC();
    PATTERNFUNC[] PatternFuncs;


    [Header("=== Shot Setting ===")]
    private LineRenderer line;
    [SerializeField] private Transform shotPos;
    [SerializeField] private GameObject[] shot_Prefeb;


    // ����Ʈ ����
    // 1. ��ų ����Ʈ�� �������� Ư�� ����
    // 2. 4�ϸ��� ������ ���
    // 3. ��ų ����Ŭ 5���� �� ����ġ ���� ���� (�Ϲ� 3�� / ���� 2��)

    // ���� A
    // ����� + ����� + �����

    // ���� B
    // ����� + ������ + �����

    // ���� C
    // ������ + ������ + �����


    // ���� D
    // ���� + ������ + �����

    // ���� E
    // ���� + ������ + ������


    // ���� ����

    // 0 : �� �¸� 
    // 1 : �ǰ�
    // 2 : Ʈ���� 1,2
    // 3 : Ʈ���� 3, ���� 1
    // 4 : ���� 2
    // 5 : �߻�
    // 6 : ���


    private void Awake()
    {
        line = GetComponent<LineRenderer>();

        // �������ͽ� ����
        Status_Setting();

        // ��� ���̺� ����
        Drop_Table_Setting();

        // ��������Ʈ ���� ����
        PatternFuncs = new PATTERNFUNC[5] { PatternA, PatternB, PatternC, PatternD, PatternE };
    }


    public override void Turn_AttackSetting(GameObject obj)
    {
        // ���� �ӵ� �Ҵ�
        Slot_SpeedSetting();

        // UI On
        enemyUI.UI_Setting(true);

        // ���� ���� �������� üũ
        if (canAction)
        {
            // Ư�� ���� ���� üũ
            if (curChargedAttackCount >= chargedAttackCount)
            {
                Attack_Weight_Charged();
            }
            else
            {
                curChargedAttackCount++;
                Attack_Weight_Normal();
            }
        }
        else
        {
            // �ൿ �Ҵ� �� -> ��� ���� ������ ���� ä���
            for (int i = 0; i < attack_Slots.Count; i++)
            {

            }
        }
    }

    // ����ġ ���� ���� ���� - �Ϲ� ����
    private void Attack_Weight_Normal()
    {
        // ����ġ ���� ���
        int total = 0;
        foreach (var attack in patternList)
        {
            total += attack.Weight;
        }

        int ran = Random.Range(0, total);
        foreach (var attack in patternList)
        {
            if (ran <= attack.Weight)
            {
                curPattern = (AttackPattern)attack.enumValue;
                PatternFuncs[attack.enumValue]();
                break;
            }
            else
            {
                ran -= attack.Weight;
            }
        }
    }

    // ����ġ ���� ���� ���� - ��ȭ ����
    private void Attack_Weight_Charged()
    {
        // ����ġ ���� ���
        int total = 0;
        foreach (var attack in chargedPattern)
        {
            total += attack.Weight;
        }

        int ran = Random.Range(0, total);
        foreach (var attack in patternList)
        {
            if (ran <= attack.Weight)
            {
                curPattern = (AttackPattern)attack.enumValue;
                PatternFuncs[attack.enumValue]();
                break;
            }
            else
            {
                ran -= attack.Weight;
            }
        }
    }

    /// <summary>
    /// ���� ���� �Լ�
    /// </summary>
    /// <param name="isCharged">���� ���� üũ</param>
    /// <param name="patternIndex">n�������� n2 ��° ���� ���� üũ</param>
    private void SetAttackSlot(bool isCharged, int patternIndex)
    {
        if(isCharged)
        {
            for (int i = 0; i < attack_Slots.Count; i++)
            {
                attack_Slots[i].Attack_Setting(attacklist[patternValueChargedList[patternIndex, i]]);
                attack_Slots[i].Attack_Setting(Attack_Slot.AttackType.Oneside_Attack, Turn_AttackTargetSetting());
                turnManager.Exchange_Setting_Enemy(attack_Slots[i], attack_Slots[i].targetSlot);
            }
        }
        else
        {
            for (int i = 0; i < attack_Slots.Count; i++)
            {
                attack_Slots[i].Attack_Setting(attacklist[patternValueList[patternIndex, i]]);
                attack_Slots[i].Attack_Setting(Attack_Slot.AttackType.Oneside_Attack, Turn_AttackTargetSetting());
                turnManager.Exchange_Setting_Enemy(attack_Slots[i], attack_Slots[i].targetSlot);
            }
        }
    }


    #region Attack Pattern
    // ��� ��� ���
    private void Pattern0()
    {
        // ��� + ��� + ���
        curPattern = AttackPattern.None;

        // ���� ����
        for (int i = 0; i < attack_Slots.Count; i++)
        {
            attack_Slots[i].Attack_Setting(null);
            turnManager.Exchange_Setting_Enemy(attack_Slots[i], null);
        }
    }

    // �Ϲݰ��� ���� 1
    private void PatternA()
    {
        // ����� + ����� + �����
        curPattern = AttackPattern.PatternA;
        SetAttackSlot(false, 0);
    }

    // �Ϲݰ��� ���� 2
    private void PatternB()
    {
        // ����� + ������ + �����
        curPattern = AttackPattern.PatternB;
        SetAttackSlot(false, 1);
    }

    // �Ϲݰ��� ���� 3
    private void PatternC()
    {
        // ������ + ������ + �����
        curPattern = AttackPattern.PatternC;
        SetAttackSlot(false, 2);
    }

    // ������ ���� 1
    private void PatternD()
    {
        // ���� + ������ + �����
        curPattern = AttackPattern.PatternD;
        SetAttackSlot(true, 0);
    }

    // ������ ���� 2
    private void PatternE()
    {
        // ���� + ������ + ������
        curPattern = AttackPattern.PatternE;
        SetAttackSlot(true, 0);
    }
    #endregion


    public override void Spawn()
    {
        StartCoroutine(SpawnCall());
    }

    private IEnumerator SpawnCall()
    {
        isSpawn = true;

        // ���� UI ����Ʈ
        enemyUI.Boss_SpawnUI();

        // UI ȣ��
        enemyUI.UI_Setting(true);

        // �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isSpawn", true);
        while (anim.GetBool("isSpawn"))
        {
            yield return null;
        }

        isSpawn = false;
    }

    public override void HitAnim()
    {
        if (anim != null)
        {
            SoundCall(1);
            anim.SetTrigger("Hit");
        }
    }

    public override void Die()
    {
        StartCoroutine(DieCall());
    }

    private IEnumerator DieCall()
    {
        isDie = true;

        // ����
        SoundCall(6);

        // UI ����
        enemyUI.UI_Setting(false);
        enemyUI.Turn_FightSelect(false);
        enemyUI.TurnFight_ExchanageSummary(false, null);

        // �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isDie", true);
        while (anim.GetBool("isDie"))
        {
            yield return null;
        }
    }

    public void Shot(bool isBigShot, int index)
    {
        (int damage, bool cir) = DamageCal(myAttack, index);
        int damage2 = damage / 3;
        for (int i = 0; i < 3; i++)
        {
            GameObject obj = Instantiate(shot_Prefeb[isBigShot ? 1 : 0], shotPos.position, Quaternion.identity);
            obj.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.ShotType.Sine, Enemy_Bullet.DamageType.Magical, cir, damage2, 15f, Player_Manager.instnace.player_Turn.gameObject);
        }
    }

    public void Line(bool isOn)
    {
        if(isOn)
        {
            line.enabled = true;
            line.SetPosition(0, shotPos.position);
            line.SetPosition(1, Player_Manager.instnace.player_Turn.transform.position);
        }
        else
        {
            line.SetPosition(0, shotPos.position);
            line.SetPosition(1, shotPos.position);
            line.enabled = false;
        }
    }

    public override void Phase2()
    {
        throw new System.NotImplementedException();
    } // �̻��
}
