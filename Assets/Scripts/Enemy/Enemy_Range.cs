using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class Enemy_Range : Enemy_Base
{
    [Header("=== Enemy Range Setting ===")]
    [SerializeField] private AttackPattern curPattern;
    [SerializeField] private int chargedAttackCount;
    [SerializeField] private int refShotCount;
    [SerializeField] private int curChargedAttackCount;
    private enum AttackPattern { None, Swing, NormalShot, BigShot }

    [SerializeField] private Transform normalShotPos;
    [SerializeField] private Transform[] shotgunPos;

    [Header("=== VFX ===")]
    [SerializeField] private GameObject bulletVFX;
    [SerializeField] private GameObject dieVFX;


    // ���Ÿ� ���� ����

    // ���� ����
    // �Ϲ� 1 : �ֵѱ�
    // �Ϲ� 2 : �����
    // ������ : ����

    // ���� ����
    // 0 : �� �¸� 
    // 1 : �ǰ�
    // 2 : 3����
    // 3 : �ֵθ���
    // 4 : ����
    // 5 : ���


    private void Awake()
    {
        Status_Setting();
        Drop_Table_Setting();
    }

    // ���� ����
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
                attack_Slots[0].Attack_Setting(attacklist[2]);
                attack_Slots[0].Attack_Setting(Attack_Slot.AttackType.Oneside_Attack, Turn_AttackTargetSetting());
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

    // ����ġ ���
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

    // ��ȯ �ִϸ��̼� ����
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
        if (anim != null)
        {
            // ����
            SoundCall(1);

            anim.SetTrigger("Hit");
        }
    }

    // ��� ȣ��
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


    public void NormalShot(int index)
    {
        // ����
        SoundCall(2);

        // ź �߻�
        (int damage, bool cir) = DamageCal(myAttack, index);
        GameObject obj = Instantiate(bulletVFX, normalShotPos.position, Quaternion.identity);
        obj.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.ShotType.Normal, Enemy_Bullet.DamageType.Physical, cir, damage, 15f, Player_Manager.instnace.player_Turn.gameObject);
    }

    public void ShotGunShot(int index)
    {
        // ����
        SoundCall(4);
        (int damage, bool cir) = DamageCal(myAttack, index);
        int damage2 = damage / shotgunPos.Length;
        Player_Manager.instnace.Take_Damage(damage2, Player_Manager.DamageType.Physical, cir);

        // ź �߻�
        for (int i = 0; i < shotgunPos.Length; i++)
        {
            GameObject obj = Instantiate(bulletVFX, shotgunPos[i].position, Quaternion.identity);
            Vector3 shotPos = Player_Manager.instnace.player_Turn.gameObject.transform.position - shotgunPos[i].transform.position;

            obj.GetComponent<Rigidbody>().velocity = shotPos.normalized * 15f;
        }
    }

    public override void Phase2()
    {
        throw new System.NotImplementedException();
    }
}
