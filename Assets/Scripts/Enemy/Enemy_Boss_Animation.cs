using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boss_Animation : MonoBehaviour
{
    [Header("=== Setting ===")]
    [SerializeField] private Enemy_Base enemy;
    [SerializeField] private Enemy_Boss boss;
    private Animator anim;
    

    [Header("=== VFX ===")]
    [SerializeField] private GameObject[] exchangeVFX;
    [SerializeField] private GameObject[] tripleVFX;
    [SerializeField] private GameObject[] bladeRushVFX;
    [SerializeField] private GameObject strikeVFX;
    [SerializeField] private GameObject[] jumpSlashVFX;
    [SerializeField] private GameObject[] quadraVFX;
    [SerializeField] private GameObject upwardVFX;
    [SerializeField] private GameObject explosionStrikeVFX;



    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    #region 기본 이벤트
    // 스폰 애니메이션 종료
    public void SpawnAnim()
    {
        anim.SetBool("isSpawn", false);
    }

    // 페이즈 2 애니메이션 종료
    public void Phase2Anim()
    {
        anim.SetBool("isPhase2", false);
    }

    // 피격 애니메이션 종료
    public void HitAnim()
    {
        anim.SetBool("isHit", false);
    }

    // 사망 애니메이션 종료
    public void DieAnim()
    {
        anim.SetBool("isDie", false);
    }

    // 합 이동 준비 종료
    public void ExchangeMoveReadyAnim()
    {
        anim.SetBool("isExchangeMoveReady", false);
    }

    // 합 종료 시 애니메이션 종료
    public void ExchangeEnd()
    {
        anim.SetBool("isExchangeWin", false);
        anim.SetBool("isExchangeLose", false);
    }

    public void BackstepOver()
    {
        anim.SetBool("isBackstep", false);
    }
    #endregion


    #region 공격


    // 일단 공격이 종료할때
    public void AttackOver()
    {
        anim.SetBool("isAttack", false);
    }


    public void TripleOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isTripleCombo", false);
    }

    public void BladeRushReadyOver()
    {
        anim.SetBool("isBladeRushReady", false);
    }

    public void BladeRushOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isBladeRush", false);
    }

    public void StirkeReadyOver()
    {
        anim.SetBool("isStrikeReady", false);
    }

    public void StirkeMoveReady()
    {
        anim.SetBool("isStirkeMoveReady", false);
    }

    public void StikeOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isStrike", false);
    }

    public void JumpSlashReadyOver()
    {
        anim.SetBool("isJumpSlashReady", false);
    }
    public void JumpSlashOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isJumpSlash", false);
    }

    public void QuadrOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isQuadraSlash", false);
    }

    public void UpwardOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isUpwardSlash", false);
    }

    public void ExplosionStrikeOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isExplosionStrike", false);
    }

    public void SuperChargeOver()
    {
        anim.SetBool("isSuperSlashCharge", false);
    }

    public void SuperSlashOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isSuperSlash", false);
    }

    public void AttackCall()
    {
        // 미리 데이터가 들어가 있어야만 제대로 동작함!
        enemy.Attack_Call();
    }

    #endregion


    #region 이펙트
    public void ExchangeVFX(int index)
    {
        // 사운드
        boss.SoundCall(0);

        if (exchangeVFX[index].activeSelf)
        {
            exchangeVFX[index].SetActive(false);
        }
        exchangeVFX[index].SetActive(true);
    }


    public void TripVFX(int index)
    {
        // 사운드
        if(index == 2)
        {
            boss.SoundCall(4);
        }
        else
        {
            boss.SoundCall(3);
        }

        tripleVFX[index].SetActive(true);
    }


    public void BladeRushFVFX()
    {
        // 사운드
        boss.SoundCall(3);

        bladeRushVFX[0].SetActive(true);
        bladeRushVFX[2].SetActive(true);
    }

    public void BladeRushBVFX()
    {
        // 사운드
        boss.SoundCall(4);
     
        bladeRushVFX[1].SetActive(true);
    }


    public void StrikeVFX()
    {
        // 사운드
        boss.SoundCall(7);

        strikeVFX.SetActive(true);
    }
    public void StrikeVFX2()
    {
        // 사운드
        boss.SoundCall(11);

        boss.StrikeExplosion();
    }


    public void JumpSlashVFX(int index)
    {
        // 사운드
        boss.SoundCall(5);

        jumpSlashVFX[index].SetActive(true);
        boss.JumpSlashSwordAura();
    }


    public void QuadrVFX(int index)
    {
        // 사운드
        if (index == 0 || index == 3)
        {
            boss.SoundCall(6);
        }
        else
        {
            boss.SoundCall(6);
        }

        // 이펙트
        quadraVFX[index].SetActive(true);

        // 검기 발사
        boss.QuadrSwordAura(index);
    }

    public void UpwardVFX()
    {
        // 사운드
        boss.SoundCall(5);

        upwardVFX.SetActive(true);
        boss.UpwardExplosion();
    }


    public void ExplosionStrikeVFX()
    {
        // 사운드
        //boss.SoundCall(10);

        explosionStrikeVFX.SetActive(true);
        boss.ExplosionStrike();
    }
    #endregion
}
