using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy_Normal : Enemy_Base
{
    // �븻 ���� ����

    // 1. ��ų ����Ʈ�� �������� Ư�� ����
    // 2. 4�ϸ��� ������ ���
    // 3. ��ų ����Ŭ 5���� �� ����ġ ���� ���� (�Ϲ� 3�� / ���� 2��)

    // ���� A
    // ����� + ����� + ���

    // ���� B
    // ����� + ������ + �����

    // ���� C
    // ������ + ������ + ���

    // ���� D
    // ���� + ������ + �����

    // ���� E
    // ���� + ������ + ������


    [Header("=== Enemy Setting ===")]
    [SerializeField] private AttackPattern curPattern;


    [Header("===Charged Attack Setting===")]
    [SerializeField] private List<Attack_Pattern> chargedPattern;
    [SerializeField] private int chargedAttackCount;
    private int curChargedAttackCount;


    private enum AttackPattern { None, PatternA, PatternB, PatternC, PatternD, PatternE }
    private enum PatternType { Normal, Charged }

    [Header("=== Base Effect ===")]
    [SerializeField] private GameObject dieVFX;


    // �� ���� ����
    public override void Turn_AttackSetting()
    {
        // ���� ���� �������� üũ
        if (!canAction)
        {
            // �ൿ �Ҵ� �� -> ��� ���� ������ ���� ä���
            for (int i = 0; i < attack_Slots.Count; i++)
            {
                Pattern0();
            }
        }
        else
        {
            attack_Slots[0].Attack_Setting_Enemy(Trun_TargetSetting(), attacklist[1]);
            attack_Slots[1].Attack_Setting_Enemy(Trun_TargetSetting(), attacklist[1]);
            attack_Slots[2].Attack_Setting_Enemy(Trun_TargetSetting(), attacklist[1]);

            /*
            // Ư�� ���� ���� üũ
            if (curAttackCount >= chargedAttackCount)
            {
                // ���� ����
                Attack_Weight(PatternType.Charged);
            }
            else
            {
                // �Ϲ� ����
                Attack_Weight(PatternType.Normal);
            }
            */
        }
    }


    #region Attack Pattern

    // ����ġ ���� ���� ����
    private void Attack_Weight(PatternType type)
    {
        // ����ġ ���� ���
        switch (type)
        {
            case PatternType.Normal:
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
                    }
                    ran -= attack.Weight;
                }
                break;

            case PatternType.Charged:
                total = 0;
                foreach (var attack in chargedPattern)
                {
                    total += attack.Weight;
                }

                ran = Random.Range(0, total);
                foreach (var attack in chargedPattern)
                {
                    if (ran <= attack.Weight)
                    {
                        curPattern = (AttackPattern)attack.enumValue;
                    }
                    ran -= attack.Weight;
                }
                break;
        }

        // ���� ����
        switch (curPattern)
        {
            case AttackPattern.None:
                Pattern0();
                break;

            case AttackPattern.PatternA:
                PatternA();
                break;

            case AttackPattern.PatternB:
                PatternB();
                break;

            case AttackPattern.PatternC:
                PatternC();
                break;

            case AttackPattern.PatternD:
                PatternD();
                break;

            case AttackPattern.PatternE:
                PatternE();
                break;
        }
    }

    // ��� ��� ���
    private void Pattern0()
    {
        // ����� + ����� + ���
        curPattern = AttackPattern.None;

        // ���� ����
        attack_Slots[0].Attack_Setting_Enemy(Trun_TargetSetting(), null);
        attack_Slots[1].Attack_Setting_Enemy(Trun_TargetSetting(), null);
        attack_Slots[2].Attack_Setting_Enemy(Trun_TargetSetting(), null);
    }

    // �Ϲݰ��� ���� 1
    private void PatternA()
    {
        // ����� + ����� + ���
        curPattern = AttackPattern.PatternA;

        // ���� ����
        attack_Slots[0].Attack_Setting_Enemy(Trun_TargetSetting(), attacklist[1]);
        attack_Slots[1].Attack_Setting_Enemy(Trun_TargetSetting(), attacklist[1]);
        attack_Slots[2].Attack_Setting_Enemy(Trun_TargetSetting(), attacklist[0]);
    }

    // �Ϲݰ��� ���� 2
    private void PatternB()
    {
        // ����� + ������ + �����
        curPattern = AttackPattern.PatternB;

        // ���� ����
        attack_Slots[0].Attack_Setting_Enemy(Trun_TargetSetting(), attacklist[1]);
        attack_Slots[1].Attack_Setting_Enemy(Trun_TargetSetting(), attacklist[2]);
        attack_Slots[2].Attack_Setting_Enemy(Trun_TargetSetting(), attacklist[1]);
    }

    // �Ϲݰ��� ���� 3
    private void PatternC()
    {
        // ������ + ������ + ���
        curPattern = AttackPattern.PatternC;

        // ���� ����
        attack_Slots[0].Attack_Setting_Enemy(Trun_TargetSetting(), attacklist[2]);
        attack_Slots[1].Attack_Setting_Enemy(Trun_TargetSetting(), attacklist[2]);
        attack_Slots[2].Attack_Setting_Enemy(Trun_TargetSetting(), attacklist[0]);
    }


    // ������ ���� 1
    private void PatternD()
    {
        // ���� + ������ + �����
        curPattern = AttackPattern.PatternD;

        // ���� ����
        attack_Slots[0].Attack_Setting_Enemy(Trun_TargetSetting(), attacklist[3]);
        attack_Slots[1].Attack_Setting_Enemy(Trun_TargetSetting(), attacklist[2]);
        attack_Slots[2].Attack_Setting_Enemy(Trun_TargetSetting(), attacklist[1]);
    }

    // ������ ���� 2
    private void PatternE()
    {
        // ���� + ������ + ������
        curPattern = AttackPattern.PatternE;

        // ���� ����
        attack_Slots[0].Attack_Setting_Enemy(Trun_TargetSetting(), attacklist[3]);
        attack_Slots[1].Attack_Setting_Enemy(Trun_TargetSetting(), attacklist[2]);
        attack_Slots[2].Attack_Setting_Enemy(Trun_TargetSetting(), attacklist[2]);
    }
    #endregion


    // �ǰ� �ִϸ��̼� ȣ�� & ����
    public override void HitAnim()
    {
        // �̰� ��¥�� �������ε� ���� isHit �Ұ����� �ʿ䰡 �ֳ�?
        if(anim != null)
        {
            anim.SetTrigger("Hit");
        }
    }

    // ��� ȣ��
    public override void Die()
    {
        StartCoroutine(DieCall());
    }

    // ��� ����
    private IEnumerator DieCall()
    {
        curPattern = AttackPattern.None;
        isDie = true;

        // ��� �ִϸ��̼�
        if(anim != null)
        {
            anim.SetTrigger("Die");
            anim.SetBool("isDie", true);
            while(anim.GetBool("isDie"))
            {
                yield return null;
            }
        }

        // ��� �ִϸ��̼� ���� �� ����Ʈ
        // ������ ȿ���ϼ���?
        if(dieVFX != null)
        {
            dieVFX.SetActive(true);
            while (dieVFX.activeSelf)
            {
                yield return null;
            }
        }

        Destroy(gameObject);
    }
}
