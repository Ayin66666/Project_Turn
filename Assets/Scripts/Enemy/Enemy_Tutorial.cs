using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Tutorial : Enemy_Base
{
    // 노말 몬스터 셋팅

    // 패턴 종류
    // 일반 1 : 이연타
    // 일반 2 : 찌르기
    // 강공격 : 화염방사

    // 사운드 셋팅
    // 0 : 합 승리 
    // 1 : 피격
    // 2 : 스윙, 이연타12, 삼연타12
    // 3 : 삼연타3
    // 4 : 사망

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
                Debug.Log("차지 공격 / 튜토리얼");
                curPattern = AttackPattern.TripleSwing;
                curChargedAttackCount = 0;
                attack_Slots[0].Attack_Setting(attacklist[2]);
                attack_Slots[0].Attack_Setting(Attack_Slot.AttackType.Oneside_Attack, Turn_AttackTargetSetting());
                turnManager.Exchange_Setting_Enemy(attack_Slots[0], attack_Slots[0].targetSlot);
            }
            else
            {
                // 일반 공격
                Debug.Log("일반 공격 / 튜토리얼");
                curChargedAttackCount++;
                Attack_Weight();
            }
        }
    }

    // 가중치 랜덤 패턴 설정
    private void Attack_Weight()
    {
        // 가중치 랜덤 계산
        int total = 0;
        foreach (var attack in patternList)
        {
            total += attack.Weight;
        }


        int ran = Random.Range(0, total);
        Debug.Log(total + " / 가중치 토탈값 / " + ran + " / 나온 초기 가중치값");
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

        // UI 호출
        enemyUI.UI_Setting(true);

        // 플레이어 바라보기
        LookAt(Player_Manager.instnace.player_Turn.gameObject);

        // 애니메이션
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

    // 사망 동작
    private IEnumerator DieCall()
    {
        isDie = true;
        controller.enabled = false;

        // UI 종료
        enemyUI.UI_Setting(false);
        enemyUI.Turn_FightSelect(false);
        enemyUI.TurnFight_ExchanageSummary(false, null);

        // 사망 사운드
        SoundCall(4);

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


    /// <summary>
    /// 미사용 코드 - 보스용
    /// </summary>
    public override void Phase2()
    {

    }
}
