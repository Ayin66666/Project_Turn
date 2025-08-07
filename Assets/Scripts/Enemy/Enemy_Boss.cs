using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy_Boss : Enemy_Base
{
    private enum Pattern_Phase1 { PatternA1, PatternB1, PatternC1, PatternD1 }
    private enum Pattern_Phase2 { PatternA2, PatternB2, PatternC2, PatternD2 }


    [Header("=== Boss Setting ===")]
    [SerializeField] private Pattern_Phase1 pattern_Phase1;
    [SerializeField] private Pattern_Phase2 pattern_Phase2;


    [Header("=== VFX ===")]
    public GameObject wingVFX;
    public GameObject teleportVFX;
    public GameObject chargeVFX;


    [Header("=== Phase 2 ===")]

    // Effect
    [SerializeField] private GameObject phase2VFX;

    // Boss
    [SerializeField] private SkinnedMeshRenderer swordRenderer;
    [SerializeField] private SkinnedMeshRenderer hair;
    [SerializeField] private SkinnedMeshRenderer wing;
    [SerializeField] private SkinnedMeshRenderer head;
    [SerializeField] private SkinnedMeshRenderer[] dress;
    [SerializeField] private Material hairMat;
    [SerializeField] private Material dressMat;
    [SerializeField] private Material swordMat;
    [SerializeField] private Material wingMat;
    [SerializeField] private Material headMat;

    // Map
    [SerializeField] private MeshRenderer mapRenderer;
    [SerializeField] private Material phase2MapMaterial;
    [SerializeField] private Material skyBox;


    // 스킬 6종 필살기 1종

    // 3연 베기, 내려찍기, 돌진, 점프베어- 1 페이즈
    // 4연 베기, 올려베기 - 2 페이즈
    // 스트라이크 - 필살기


    // 페이즈 1

    // 3연 - 내려찍기 - 돌진 - 내려찍기
    // 3연 - 돌진 - 점프 베어 - 내려찍기
    // 내려찍기 - 내려찍기 - 돌진- 점프 베어
    // 3연 - 돌진 - 내려찍기 - 점프 베어


    // 페이즈 2

    // 4연 - 내려찍기 - 올려베기 - 내려찍기
    // 4연 - 돌진 - 점프 베어 - 내려찍기
    // 4연 - 내려찍기 - 내려찍기 - 점프 베어
    // 4연 - 4연 - 내려찍기 - 점프 베어


    // 차지 패턴
    // 4연 - 돌진 - 내려찍기 - 스트라이크


    // 사운드 셋팅
    // 0 : 합 승리 
    // 1 : 피격
    // 2 : 3연베기 1
    // 3 : 3연베기 2
    // 4 : 3연베기 3 , 돌진 2
    // 5 : 내려찍기
    // 6 : 돌진 1
    // 7 : 점프베어
    // 8 : 4연베기 4
    // 9 : 올려베기
    // 10 : 스트라이크
    // 11 : 폭발
    // 12 : 사망

    delegate void PATTERNFUNC();
    PATTERNFUNC[] PatternFuncsA;
    PATTERNFUNC[] PatternFuncsB;


    [Header("=== Phase Attack Value ===")]
    private int[,,] valueList = new int[2,4,4]
    {
        { 
           { 0, 2, 1, 2 },
           { 0, 1, 3, 2 },
           { 2, 2, 1, 3 }, 
           { 0, 1, 2, 3 }
        },
        { 
           { 4, 2, 2, 3 }, 
           { 4, 1, 3, 2 },
           { 4, 2, 5, 3 },
           { 4, 5, 2, 3 }
        },
    };


    [Header("===Charged Attack Setting===")]
    [SerializeField] private List<Attack_Pattern> chargedPattern;
    [SerializeField] private int chargedAttackCount;
    private int curChargedAttackCount;


    [Header("=== Attack Pos Setting ===")]
    [SerializeField] private Transform[] explosionStrikePosA;
    [SerializeField] private Transform[] explosionStrikePosB;
    [SerializeField] private Transform[] explosionStrikePosC;
    [SerializeField] private Transform[] strikeExplosionPos;
    [SerializeField] private Transform[] upwardExplosionPos;


    [Header("=== 신규 포지션 ===")]
    [SerializeField] private Transform shotPos;
    public Transform comboPos;
    public Transform[] jumpSlashPos;
    public Transform[] qudraSlashPos;
    public Transform backstepPos;


    [Header("=== VFX ===")]
    [SerializeField] private GameObject explosionStrikeVFX;
    [SerializeField] private GameObject stirkeExplosionVFX;
    [SerializeField] private GameObject upwardExplosionVFX;
    [SerializeField] private GameObject jumpSlashSwordAuraVFX;
    [SerializeField] private GameObject quadrSwordAuraVFX;



    private void Awake()
    {
        // 스테이터스 셋팅
        Status_Setting();

        // 델리게이트 패턴 셋팅
        PatternFuncsA = new PATTERNFUNC[4] { PatternA1, PatternB1, PatternC1, PatternD1 };
        PatternFuncsB = new PATTERNFUNC[4] { PatternA2, PatternB2, PatternC2, PatternD2 };
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
                ChargePattern();
            }
            else
            {
                curChargedAttackCount++;

                // 페이즈에 따른 공격 셋팅
                if (isPhase2)
                {
                    Attack_Weight_Phase2();
                }
                else
                {
                    Attack_Weight_Phase1();
                }
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


    /// <summary>
    /// 공격 셋팅 함수
    /// </summary>
    /// <param name="phaseIndex">1,2페이즈 체크 값</param>
    /// <param name="patternIndex">n페이즈의 n2 번째 공격 패턴 체크</param>
    void SetAttackSlot(int phaseIndex, int patternIndex)
    {
        for (int i = 0; i < attack_Slots.Count; i++)
        {
            attack_Slots[i].Attack_Setting(attacklist[valueList[phaseIndex, patternIndex, i]]);
            attack_Slots[i].Attack_Setting(Attack_Slot.AttackType.Oneside_Attack, Turn_AttackTargetSetting());
            turnManager.Exchange_Setting_Enemy(attack_Slots[i], attack_Slots[i].targetSlot);
        }
    }

    #region Attack Pattern - Phase 1

    private void Attack_Weight_Phase1()
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
                pattern_Phase1 = (Pattern_Phase1)attack.enumValue;
                PatternFuncsA[attack.enumValue]();
                Debug.Log("현재 패턴 : " + pattern_Phase1);
                break;
            }
            else
            {
                ran -= attack.Weight;
            }
        }
    }

    // 3연 - 내려찍기 - 돌진 - x
    private void PatternA1()
    {
        SetAttackSlot(0, 0);
    }

    // 3연 - 돌진 - 점프 베어 - x
    private void PatternB1()
    {
        SetAttackSlot(0, 1);
    }

    // 내려찍기 - 내려찍기 - 점프 베어 - x
    private void PatternC1()
    {
        SetAttackSlot(0, 2);
    }

    // 3연 - 3연 - 내려찍기 - 점프 베어
    private void PatternD1()
    {
        SetAttackSlot(0, 3);
    }
    #endregion


    #region Attack Pattern - Phase 2
    private void Attack_Weight_Phase2()
    {
        // 가중치 랜덤 계산
        int total = 0;
        foreach (var attack in (patternList_Phase2))
        {
            total += attack.Weight;
        }

        int ran = Random.Range(0, total);
        foreach (var attack in patternList)
        {
            if (ran <= attack.Weight)
            {
                pattern_Phase2 = (Pattern_Phase2)attack.enumValue;
                PatternFuncsB[attack.enumValue]();
                break;
            }
            else
            {
                ran -= attack.Weight;
            }
        }
    }

    // 4연 - 내려찍기 - 올려베기 - 내려찍기
    private void PatternA2()
    {
        SetAttackSlot(1, 0);
    }

    // 4연 - 돌진 - 점프 베어 - 내려찍기
    private void PatternB2()
    {
        SetAttackSlot(1, 1);
    }

    // 4연 - 내려찍기 - 내려찍기 - 점프 베어
    private void PatternC2()
    {
        SetAttackSlot(1, 2);
    }

    // 4연 - 4연 - 내려찍기 - 점프 베어
    private void PatternD2()
    {
        SetAttackSlot(1, 3);
    }
    #endregion


    #region Attack Pattern - Charged

    // 4연 - 스톰핑 - 내려찍기 - 스트라이크
    private void ChargePattern()
    {
        curChargedAttackCount = 0;

        // 1번
        attack_Slots[0].Attack_Setting(attacklist[4]);
        attack_Slots[0].Attack_Setting(Attack_Slot.AttackType.Oneside_Attack, Turn_AttackTargetSetting());
        turnManager.Exchange_Setting_Enemy(attack_Slots[0], attack_Slots[0].targetSlot);

        // 2번
        attack_Slots[1].Attack_Setting(attacklist[1]);
        attack_Slots[1].Attack_Setting(Attack_Slot.AttackType.Oneside_Attack, Turn_AttackTargetSetting());
        turnManager.Exchange_Setting_Enemy(attack_Slots[1], attack_Slots[1].targetSlot);

        // 3번
        attack_Slots[2].Attack_Setting(attacklist[5]);
        attack_Slots[2].Attack_Setting(Attack_Slot.AttackType.Oneside_Attack, Turn_AttackTargetSetting());
        turnManager.Exchange_Setting_Enemy(attack_Slots[2], attack_Slots[2].targetSlot);

        // 4번
        attack_Slots[3].Attack_Setting(attacklist[6]);
        attack_Slots[3].Attack_Setting(Attack_Slot.AttackType.Oneside_Attack, Turn_AttackTargetSetting());
        turnManager.Exchange_Setting_Enemy(attack_Slots[3], attack_Slots[3].targetSlot);
    }


    // 필살기 추가 공격 기능
    public void ExplosionStrike()
    {
        StartCoroutine(ExplosionStrikeCall());
    }


    private IEnumerator ExplosionStrikeCall()
    {
        // 1번 폭발
        //Player_Manager.instnace.Take_Damage(30, Player_Manager.DamageType.Magical, false);
        for (int i2 = 0; i2 < explosionStrikePosA.Length; i2++)
        {
            GameObject obj = Instantiate(explosionStrikeVFX, explosionStrikePosA[i2].position, Quaternion.identity);
        }

        yield return new WaitForSeconds(0.25f);

        // 2번 폭발
        //Player_Manager.instnace.Take_Damage(30, Player_Manager.DamageType.Magical, false);
        for (int i2 = 0; i2 < explosionStrikePosB.Length; i2++)
        {
            GameObject obj = Instantiate(explosionStrikeVFX, explosionStrikePosB[i2].position, Quaternion.identity);
        }

        yield return new WaitForSeconds(0.25f);

        // 3번 폭발
        //Player_Manager.instnace.Take_Damage(30, Player_Manager.DamageType.Magical, false);
        for (int i2 = 0; i2 < explosionStrikePosC.Length; i2++)
        {
            GameObject obj = Instantiate(explosionStrikeVFX, explosionStrikePosC[i2].position, Quaternion.identity);
        }
    }
    #endregion


    public void StrikeExplosion()
    {
        for (int i = 0; i < strikeExplosionPos.Length; i++)
        {
            // 사운드
            SoundCall(8);

            Instantiate(stirkeExplosionVFX, strikeExplosionPos[i].position, Quaternion.identity);
        }
    }

    public void UpwardExplosion()
    {
        StartCoroutine(UpwardExplosionCall());
    }

    private IEnumerator UpwardExplosionCall()
    {
        for (int i = 0; i < upwardExplosionPos.Length; i++)
        {
            // 사운드
            SoundCall(9);

            Instantiate(upwardExplosionVFX, upwardExplosionPos[i].position, Quaternion.identity);
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void JumpSlashSwordAura()
    {
        (int damage, bool cir) = DamageCal(myAttack, 0);

        GameObject obj = Instantiate(jumpSlashSwordAuraVFX, shotPos.position, Quaternion.identity);
        obj.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.ShotType.Normal, Enemy_Bullet.DamageType.Magical, cir, damage, 20, Player_Manager.instnace.player_Turn.gameObject);
    }

    public void QuadrSwordAura(int attackCount)
    {
        (int damage, bool cir) = DamageCal(myAttack, attackCount);
        GameObject obj = Instantiate(quadrSwordAuraVFX, shotPos.position, Quaternion.identity);
        obj.GetComponent<Enemy_Bullet>().Bullet_Setting(Enemy_Bullet.ShotType.Normal, Enemy_Bullet.DamageType.Magical, cir, damage, 20, Player_Manager.instnace.player_Turn.gameObject);
    }

    public override void Phase2()
    {
        StartCoroutine(Phase2Call());
    }

    private IEnumerator Phase2Call()
    {
        isPhase2 = true;
        isCutscene = true;

        // 페이즈 전환 컷씬 On
        enemyUI.Phase2Cutscene();

        // 맵 변경
        MapChange();

        // 보스 변경
        BossChange();

        // 사운드 종료
        Stage_Manager.instance.BGM_OnOff(false);

        // 사운드 변경
        Stage_Manager.instance.BGM_Setting(1);

        // 비디오 종료 대기
        while (enemyUI.isVideoOn)
        {
            yield return null;
        }
        isCutscene = false;

        // 사운드
        SoundCall(10);

        // 이펙트
        phase2VFX.SetActive(true);

        // 다이얼로그
        Dialog_Manager.instance.Dialog(3);

        // 페이즈 2 변환 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isPhase2", true);
        while(anim.GetBool("isPhase2"))
        {
            yield return null;
        }
    }

    public void MapChange()
    {
        if(mapRenderer != null)
        {
            RenderSettings.skybox = skyBox;
            mapRenderer.material = phase2MapMaterial;
        }
    }

    private void BossChange()
    {
        swordRenderer.material = swordMat;
        head.material = headMat;
        hair.material = hairMat;
        dress[0].material = dressMat;
        dress[1].material = dressMat;
        wing.material = wingMat;
    }


    public override void HitAnim()
    {
        SoundCall(1);
        anim.SetTrigger("Hit");
    }


    public override void Spawn()
    {
        StartCoroutine(SpawnCall());
    }

    private IEnumerator SpawnCall()
    {
        isSpawn = true;
        isCutscene = true;

        // 영상 재생
        enemyUI.SpawnCutscene();

        yield return new WaitForSeconds(1f);
        while(enemyUI.isVideoOn)
        {
            yield return null;
        }

        // 등장 UI 이펙트
        enemyUI.Boss_SpawnUI();

        // 사운드
        Stage_Manager.instance.BGM_Setting(0);

        // 시작 다이얼로그 체크
        if (haveStartDialog)
        {
            Dialog_Manager.instance.Dialog(1);
        }

        // 다이얼로그 체크
        if (haveDialog)
        {
            dialogCheckCoroutine = StartCoroutine(Dialog_Check());
        }

        // 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isSpawn", true);

        while (anim.GetBool("isSpawn"))
        {
            yield return null;
        }

        isSpawn = false;
        isCutscene = false;
    }


    public override void Die()
    {
        StartCoroutine(DieCall());
    }

    private IEnumerator DieCall()
    {
        isDie = true;
        isCutscene = true;

        // 사운드
        SoundCall(2);

        // 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isDie", true);

        while (anim.GetBool("isDie"))
        {
            yield return null;
        }

        // 영상 재생
        enemyUI.DieCutscene();
        while (enemyUI.isVideoOn)
        {
            yield return null;
        }

        isCutscene = false;
    }
}
