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


    // 원거리 몬스터 셋팅

    // 패턴 종류
    // 일반 1 : 휘둘기
    // 일반 2 : 기관총
    // 강공격 : 샷건

    // 사운드 셋팅
    // 0 : 합 승리 
    // 1 : 피격
    // 2 : 3연발
    // 3 : 휘두르기
    // 4 : 샷건
    // 5 : 사망


    private void Awake()
    {
        Status_Setting();
        Drop_Table_Setting();
    }

    // 공격 셋팅
    public override void Turn_AttackSetting(GameObject obj)
    {
        // 슬롯 속도 할당
        Slot_SpeedSetting();

        // UI On
        enemyUI.UI_Setting(true);

        // 현제 동작 가능한지 체크
        if (canAction)
        {
            // 특수 공격 조건 체크
            if (curChargedAttackCount >= chargedAttackCount)
            {
                // 차지 공격
                curChargedAttackCount = 0;
                attack_Slots[0].Attack_Setting(attacklist[2]);
                attack_Slots[0].Attack_Setting(Attack_Slot.AttackType.Oneside_Attack, Turn_AttackTargetSetting());
                turnManager.Exchange_Setting_Enemy(attack_Slots[0], attack_Slots[0].targetSlot);
            }
            else
            {
                // 일반 공격
                curChargedAttackCount++;
                Attack_Weight();
            }
        }
    }

    // 가중치 계산
    private void Attack_Weight()
    {
        // 가중치 랜덤 계산
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

        // 공격 셋팅
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

    // 소환 애니메이션 호출
    public override void Spawn()
    {
        StartCoroutine(SpawnCall());
    }

    // 소환 애니메이션 동작
    private IEnumerator SpawnCall()
    {
        isSpawn = true;

        // UI 호출
        enemyUI.UI_Setting(true);

        // 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isSpawn", true);
        while (anim.GetBool("isSpawn"))
        {
            yield return null;
        }

        isSpawn = false;


    }

    // 피격 애니메이션 호출 & 동작
    public override void HitAnim()
    {
        if (anim != null)
        {
            // 사운드
            SoundCall(1);

            anim.SetTrigger("Hit");
        }
    }

    // 사망 호출
    public override void Die()
    {
        if (isDie)
        {
            return;
        }
        StopAllCoroutines();
        StartCoroutine(DieCall());
    }

    // 사망 동작
    private IEnumerator DieCall()
    {
        curPattern = AttackPattern.None;
        isDie = true;
        controller.enabled = false;

        // UI 종료
        enemyUI.UI_Setting(false);
        enemyUI.Turn_FightSelect(false);
        enemyUI.TurnFight_ExchanageSummary(false, null);

        // 사운드
        SoundCall(5);

        // 사망 애니메이션
        if (anim != null)
        {
            anim.SetTrigger("Action");
            anim.SetBool("isDie", true);
            while (anim.GetBool("isDie"))
            {
                yield return null;
            }
        }

        // 사망 애니메이션 종료 후 이펙트
        // 디졸브 효과일수도?
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
        // 사운드
        SoundCall(2);

        // 탄 발사
        (int damage, bool cir) = DamageCal(myAttack, index);
        GameObject obj = Instantiate(bulletVFX, normalShotPos.position, Quaternion.identity);
        obj.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.ShotType.Normal, Enemy_Bullet.DamageType.Physical, cir, damage, 15f, Player_Manager.instnace.player_Turn.gameObject);
    }

    public void ShotGunShot(int index)
    {
        // 사운드
        SoundCall(4);
        (int damage, bool cir) = DamageCal(myAttack, index);
        int damage2 = damage / shotgunPos.Length;
        Player_Manager.instnace.Take_Damage(damage2, Player_Manager.DamageType.Physical, cir);

        // 탄 발사
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
