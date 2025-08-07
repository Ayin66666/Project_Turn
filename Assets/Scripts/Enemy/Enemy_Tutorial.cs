using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Tutorial : Enemy_Base
{
    // �븻 ���� ����

    // ���� ����
    // �Ϲ� 1 : �̿�Ÿ
    // �Ϲ� 2 : ���
    // ������ : ȭ�����

    // ���� ����
    // 0 : �� �¸� 
    // 1 : �ǰ�
    // 2 : ����, �̿�Ÿ12, �￬Ÿ12
    // 3 : �￬Ÿ3
    // 4 : ���

    [Header("=== Attack Status ===")]
    [SerializeField] private AttackPattern curPattern;
    [SerializeField] private int chargedAttackCount;
    private int curChargedAttackCount;
    private enum AttackPattern { None, Swing, DoubleSwing, TripleSwing }


    [Header("=== Effect ===")]
    [SerializeField] private GameObject dieVFX;


    private void Awake()
    {
        Status_Setting();
        Drop_Table_Setting();
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
                // ���� ����
                Debug.Log("���� ���� / Ʃ�丮��");
                curPattern = AttackPattern.TripleSwing;
                curChargedAttackCount = 0;
                attack_Slots[0].Attack_Setting(attacklist[2]);
                attack_Slots[0].Attack_Setting(Attack_Slot.AttackType.Oneside_Attack, Turn_AttackTargetSetting());
                turnManager.Exchange_Setting_Enemy(attack_Slots[0], attack_Slots[0].targetSlot);
            }
            else
            {
                // �Ϲ� ����
                Debug.Log("�Ϲ� ���� / Ʃ�丮��");
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
        Debug.Log(total + " / ����ġ ��Ż�� / " + ran + " / ���� �ʱ� ����ġ��");
        foreach (var attack in patternList)
        {
            if (ran <= attack.Weight)
            {
                curPattern = (AttackPattern)attack.enumValue;
                Debug.Log(ran + curPattern);
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

            case AttackPattern.DoubleSwing:
                attack_Slots[0].Attack_Setting(attacklist[1]);
                attack_Slots[0].Attack_Setting(Attack_Slot.AttackType.Oneside_Attack, Turn_AttackTargetSetting());
                turnManager.Exchange_Setting_Enemy(attack_Slots[0], attack_Slots[0].targetSlot);
                break;
        }
    }

    public override void Spawn()
    {
        StartCoroutine(SpawnCall());
    }

    private IEnumerator SpawnCall()
    {
        isSpawn = true;

        // UI ȣ��
        enemyUI.UI_Setting(true);

        // �÷��̾� �ٶ󺸱�
        LookAt(Player_Manager.instnace.player_Turn.gameObject);

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
        if(anim != null)
        {
            SoundCall(1);
            anim.SetTrigger("Hit");
        }
    }

    public override void Die()
    {
        if (isDie)
        {
            return;
        }
        StopAllCoroutines();
        StartCoroutine(DieCall());
    }

    // ��� ����
    private IEnumerator DieCall()
    {
        isDie = true;
        controller.enabled = false;

        // UI ����
        enemyUI.UI_Setting(false);
        enemyUI.Turn_FightSelect(false);
        enemyUI.TurnFight_ExchanageSummary(false, null);

        // ��� ����
        SoundCall(4);

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


    /// <summary>
    /// �̻�� �ڵ� - ������
    /// </summary>
    public override void Phase2()
    {

    }
}
