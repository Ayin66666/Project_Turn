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


    // 엘리트 셋팅
    // 1. 스킬 리스트의 마지막은 특수 공격
    // 2. 4턴마다 강공격 사용
    // 3. 스킬 사이클 5종류 중 가중치 랜덤 셋팅 (일반 3종 / 차지 2종)

    // 패턴 A
    // 약공격 + 약공격 + 약공격

    // 패턴 B
    // 약공격 + 강공격 + 약공격

    // 패턴 C
    // 강공격 + 강공격 + 약공격


    // 패턴 D
    // 차지 + 강공격 + 약공격

    // 패턴 E
    // 차지 + 강공격 + 강공격


    // 사운드 셋팅

    // 0 : 합 승리 
    // 1 : 피격
    // 2 : 트리플 1,2
    // 3 : 트리플 3, 더블 1
    // 4 : 더블 2
    // 5 : 발사
    // 6 : 사망


    private void Awake()
    {
        line = GetComponent<LineRenderer>();

        // 스테이터스 셋팅
        Status_Setting();

        // 드랍 테이블 셋팅
        Drop_Table_Setting();

        // 델리게이트 패턴 셋팅
        PatternFuncs = new PATTERNFUNC[5] { PatternA, PatternB, PatternC, PatternD, PatternE };
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
            // 행동 불능 시 -> 모든 공격 슬롯을 대기로 채우기
            for (int i = 0; i < attack_Slots.Count; i++)
            {

            }
        }
    }

    // 가중치 랜덤 패턴 설정 - 일반 패턴
    private void Attack_Weight_Normal()
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
                PatternFuncs[attack.enumValue]();
                break;
            }
            else
            {
                ran -= attack.Weight;
            }
        }
    }

    // 가중치 랜덤 패턴 설정 - 강화 패턴
    private void Attack_Weight_Charged()
    {
        // 가중치 랜덤 계산
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
    /// 공격 셋팅 함수
    /// </summary>
    /// <param name="isCharged">패턴 종류 체크</param>
    /// <param name="patternIndex">n페이즈의 n2 번째 공격 패턴 체크</param>
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
        // 약공격 + 약공격 + 약공격
        curPattern = AttackPattern.PatternA;
        SetAttackSlot(false, 0);
    }

    // 일반공격 패턴 2
    private void PatternB()
    {
        // 약공격 + 강공격 + 약공격
        curPattern = AttackPattern.PatternB;
        SetAttackSlot(false, 1);
    }

    // 일반공격 패턴 3
    private void PatternC()
    {
        // 강공격 + 강공격 + 약공격
        curPattern = AttackPattern.PatternC;
        SetAttackSlot(false, 2);
    }

    // 강공격 패턴 1
    private void PatternD()
    {
        // 차지 + 강공격 + 약공격
        curPattern = AttackPattern.PatternD;
        SetAttackSlot(true, 0);
    }

    // 강공격 패턴 2
    private void PatternE()
    {
        // 차지 + 강공격 + 강공격
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

        // 등장 UI 이펙트
        enemyUI.Boss_SpawnUI();

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

        // 사운드
        SoundCall(6);

        // UI 종료
        enemyUI.UI_Setting(false);
        enemyUI.Turn_FightSelect(false);
        enemyUI.TurnFight_ExchanageSummary(false, null);

        // 애니메이션
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
    } // 미사용
}
