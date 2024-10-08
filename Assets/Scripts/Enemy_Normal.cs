using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy_Normal : Enemy_Base
{
    // 노말 몬스터 셋팅

    // 1. 스킬 리스트의 마지막은 특수 공격
    // 2. 4턴마다 강공격 사용
    // 3. 스킬 사이클 5종류 중 가중치 랜덤 셋팅 (일반 3종 / 차지 2종)

    // 패턴 A
    // 약공격 + 약공격 + 대기

    // 패턴 B
    // 약공격 + 강공격 + 약공격

    // 패턴 C
    // 강공격 + 강공격 + 대기

    // 패턴 D
    // 차지 + 강공격 + 약공격

    // 패턴 E
    // 차지 + 강공격 + 강공격


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


    private void Awake()
    {
        Status_Setting();
    }


    // 합 공격 설정
    public override void Turn_AttackSetting()
    {
        // 현제 동작 가능한지 체크
        if (!canAction)
        {
            // 행동 불능 시 -> 모든 공격 슬롯을 대기로 채우기
            for (int i = 0; i < attack_Slots.Count; i++)
            {
                Pattern0();
            }
        }
        else
        {
            // 슬롯 속도 할당
            Slot_SpeedSetting();

            // 테스트용!
            for (int i = 0; i < attack_Slots.Count; i++)
            {
                Debug.Log("현제 데이터 삽입 번째 : " + i + " 번째 공격 슬롯 데이터");
                attack_Slots[i].Attack_Setting(attacklist[1]);

                Attack_Slot targetSlot = Turn_AttackTargetSetting();

                attack_Slots[i].Attack_Setting(Attack_Slot.AttackType.Oneside_Attack, targetSlot);
                turnManager.Exchange_Setting_Enemy(attack_Slots[i], targetSlot);
            }


            /*
            // 특수 공격 조건 체크
            if (curAttackCount >= chargedAttackCount)
            {
                // 차지 공격
                Attack_Weight(PatternType.Charged);
            }
            else
            {
                // 일반 공격
                Attack_Weight(PatternType.Normal);
            }
            */
        }
    }


    #region Attack Pattern

    // 가중치 랜덤 패턴 설정
    private void Attack_Weight(PatternType type)
    {
        // 가중치 랜덤 계산
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

        // 공격 셋팅
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

    // 대기 대기 대기
    private void Pattern0()
    {
        // 대기 + 대기 + 대기
        curPattern = AttackPattern.None;

        // 공격 셋팅
        for (int i = 0; i < attack_Slots.Count; i++)
        {
            attack_Slots[i].Attack_Setting(null);
            turnManager.Exchange_Setting_Enemy(attack_Slots[i], null);
        }
    }

    // 일반공격 패턴 1
    private void PatternA()
    {
        // 약공격 + 약공격 + 대기
        curPattern = AttackPattern.PatternA;

        // 공격 셋팅
        attack_Slots[0].Attack_Setting(attacklist[0]);
        turnManager.Exchange_Setting_Enemy(attack_Slots[0], Turn_AttackTargetSetting());

        attack_Slots[1].Attack_Setting(attacklist[0]);
        turnManager.Exchange_Setting_Enemy(attack_Slots[1], Turn_AttackTargetSetting());

        attack_Slots[2].Attack_Setting(null);
        turnManager.Exchange_Setting_Enemy(attack_Slots[2], Turn_AttackTargetSetting());
    }

    // 일반공격 패턴 2
    private void PatternB()
    {
        // 약공격 + 강공격 + 약공격
        curPattern = AttackPattern.PatternB;

        // 공격 셋팅
        attack_Slots[0].Attack_Setting(attacklist[0]);
        turnManager.Exchange_Setting_Enemy(attack_Slots[0], Turn_AttackTargetSetting());

        attack_Slots[1].Attack_Setting(attacklist[1]);
        turnManager.Exchange_Setting_Enemy(attack_Slots[1], Turn_AttackTargetSetting());

        attack_Slots[2].Attack_Setting(attacklist[0]);
        turnManager.Exchange_Setting_Enemy(attack_Slots[2], Turn_AttackTargetSetting());
    }

    // 일반공격 패턴 3
    private void PatternC()
    {
        // 강공격 + 강공격 + 대기
        curPattern = AttackPattern.PatternC;

        // 공격 셋팅
        attack_Slots[0].Attack_Setting(attacklist[1]);
        turnManager.Exchange_Setting_Enemy(attack_Slots[0], Turn_AttackTargetSetting());

        attack_Slots[1].Attack_Setting(attacklist[1]);
        turnManager.Exchange_Setting_Enemy(attack_Slots[1], Turn_AttackTargetSetting());

        attack_Slots[2].Attack_Setting(null);
        turnManager.Exchange_Setting_Enemy(attack_Slots[2], Turn_AttackTargetSetting());
    }


    // 강공격 패턴 1
    private void PatternD()
    {
        // 차지 + 강공격 + 약공격
        curPattern = AttackPattern.PatternD;

        // 공격 셋팅
        attack_Slots[0].Attack_Setting(attacklist[2]);
        turnManager.Exchange_Setting_Enemy(attack_Slots[0], Turn_AttackTargetSetting());

        attack_Slots[1].Attack_Setting(attacklist[1]);
        turnManager.Exchange_Setting_Enemy(attack_Slots[1], Turn_AttackTargetSetting());

        attack_Slots[2].Attack_Setting(attacklist[0]);
        turnManager.Exchange_Setting_Enemy(attack_Slots[2], Turn_AttackTargetSetting());
    }

    // 강공격 패턴 2
    private void PatternE()
    {
        // 차지 + 강공격 + 강공격
        curPattern = AttackPattern.PatternE;

        // 공격 셋팅
        attack_Slots[0].Attack_Setting(attacklist[2]);
        turnManager.Exchange_Setting_Enemy(attack_Slots[0], Turn_AttackTargetSetting());

        attack_Slots[1].Attack_Setting(attacklist[1]);
        turnManager.Exchange_Setting_Enemy(attack_Slots[1], Turn_AttackTargetSetting());

        attack_Slots[2].Attack_Setting(attacklist[1]);
        turnManager.Exchange_Setting_Enemy(attack_Slots[2], Turn_AttackTargetSetting());
    }
    #endregion


    // 피격 애니메이션 호출 & 동작
    public override void HitAnim()
    {
        // 이거 어짜피 턴제겜인데 따로 isHit 불값있을 필요가 있나?
        if(anim != null)
        {
            anim.SetTrigger("Hit");
        }
    }

    // 사망 호출
    public override void Die()
    {
        StartCoroutine(DieCall());
    }

    // 사망 동작
    private IEnumerator DieCall()
    {
        curPattern = AttackPattern.None;
        isDie = true;

        // 사망 애니메이션
        if(anim != null)
        {
            anim.SetTrigger("Die");
            anim.SetBool("isDie", true);
            while(anim.GetBool("isDie"))
            {
                yield return null;
            }
        }

        // 사망 애니메이션 종료 후 이펙트
        // 디졸브 효과일수도?
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
