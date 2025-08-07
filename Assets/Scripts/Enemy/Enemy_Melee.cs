using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy_Melee : Enemy_Base
{
    // �븻 ���� ����

    // ���� ����
    // �Ϲ� 1 : �̿�Ÿ
    // �Ϲ� 2 : ���
    // ������ : ȭ�����

    // ���� ����
    // 0 : �� �¸� 
    // 1 : �ǰ�
    // 2 : 2��Ÿ
    // 3 : ���
    // 4 : ȭ�����
    // 5 : ���

    [Header("=== Attack Status ===")]
    [SerializeField] private AttackPattern curPattern;
    [SerializeField] private int chargedAttackCount;
    private int curChargedAttackCount;
    private enum AttackPattern { None, Swing, NormalShot, ShotGun }


    [Header("=== Effect ===")]
    [SerializeField] private GameObject dieVFX;


    private void Awake()
    {
        Status_Setting();
        Drop_Table_Setting();
    }


    // �� ���� ����
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
                // ���� ����
                curChargedAttackCount = 0;
                attack_Slots[2].Attack_Setting(attacklist[2]);
                attack_Slots[2].Attack_Setting(Attack_Slot.AttackType.Oneside_Attack, Turn_AttackTargetSetting());
                turnManager.Exchange_Setting_Enemy(attack_Slots[0], attack_Slots[0].targetSlot);
            }
            else
            {
                // �Ϲ� ����
                curChargedAttackCount++;
                Attack_Weight();
            }
        }
    }


    // ����ġ ���� ���� ����
    private void Attack_Weight()
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
                break;
            }
            else
            {
                ran -= attack.Weight;
            }
        }

        // ���� ����
        switch (curPattern)
        {
            case AttackPattern.None:
                break;

            case AttackPattern.Swing:
                attack_Slots[0].Attack_Setting(attacklist[0]);
                attack_Slots[0].Attack_Setting(Attack_Slot.AttackType.Oneside_Attack, Turn_AttackTargetSetting());
                turnManager.Exchange_Setting_Enemy(attack_Slots[0], attack_Slots[0].targetSlot);
                break;

            case AttackPattern.NormalShot:
                attack_Slots[0].Attack_Setting(attacklist[1]);
                attack_Slots[0].Attack_Setting(Attack_Slot.AttackType.Oneside_Attack, Turn_AttackTargetSetting());
                turnManager.Exchange_Setting_Enemy(attack_Slots[0], attack_Slots[0].targetSlot);
                break;
        }
    }


    // ��ȯ �ִϸ��̼� ȣ��
    public override void Spawn()
    {
        StartCoroutine(SpawnCall());
    }


    private IEnumerator SpawnCall()
    {
        isSpawn = true;

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


    // �ǰ� �ִϸ��̼� ȣ�� & ����
    public override void HitAnim()
    {
        if(anim != null)
        {
            anim.SetTrigger("Hit");
        }
    }


    // ��� ȣ��
    public override void Die()
    {
        if(isDie)
        {
            return;
        }
        StopAllCoroutines();
        StartCoroutine(DieCall());
    }


    // ��� ����
    private IEnumerator DieCall()
    {
        curPattern = AttackPattern.None;
        isDie = true;
        controller.enabled = false;

        // UI ����
        enemyUI.UI_Setting(false);
        enemyUI.Turn_FightSelect(false);
        enemyUI.TurnFight_ExchanageSummary(false, null);

        // ����
        SoundCall(5);

        // ��� �ִϸ��̼�
        if (anim != null)
        {
            anim.SetTrigger("Action");
            anim.SetBool("isDie", true);
            while (anim.GetBool("isDie"))
            {
                yield return null;
            }
        }

        // ��� �ִϸ��̼� ���� �� ����Ʈ
        // ������ ȿ���ϼ���?
        if (dieVFX != null)
        {
            dieVFX.SetActive(true);
            while (dieVFX.activeSelf)
            {
                yield return null;
            }
        }
    }

    public override void Phase2()
    {
        throw new System.NotImplementedException();
    }
}
