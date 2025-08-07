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


    #region �⺻ �̺�Ʈ
    // ���� �ִϸ��̼� ����
    public void SpawnAnim()
    {
        anim.SetBool("isSpawn", false);
    }

    // ������ 2 �ִϸ��̼� ����
    public void Phase2Anim()
    {
        anim.SetBool("isPhase2", false);
    }

    // �ǰ� �ִϸ��̼� ����
    public void HitAnim()
    {
        anim.SetBool("isHit", false);
    }

    // ��� �ִϸ��̼� ����
    public void DieAnim()
    {
        anim.SetBool("isDie", false);
    }

    // �� �̵� �غ� ����
    public void ExchangeMoveReadyAnim()
    {
        anim.SetBool("isExchangeMoveReady", false);
    }

    // �� ���� �� �ִϸ��̼� ����
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


    #region ����


    // �ϴ� ������ �����Ҷ�
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
        // �̸� �����Ͱ� �� �־�߸� ����� ������!
        enemy.Attack_Call();
    }

    #endregion


    #region ����Ʈ
    public void ExchangeVFX(int index)
    {
        // ����
        boss.SoundCall(0);

        if (exchangeVFX[index].activeSelf)
        {
            exchangeVFX[index].SetActive(false);
        }
        exchangeVFX[index].SetActive(true);
    }


    public void TripVFX(int index)
    {
        // ����
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
        // ����
        boss.SoundCall(3);

        bladeRushVFX[0].SetActive(true);
        bladeRushVFX[2].SetActive(true);
    }

    public void BladeRushBVFX()
    {
        // ����
        boss.SoundCall(4);
     
        bladeRushVFX[1].SetActive(true);
    }


    public void StrikeVFX()
    {
        // ����
        boss.SoundCall(7);

        strikeVFX.SetActive(true);
    }
    public void StrikeVFX2()
    {
        // ����
        boss.SoundCall(11);

        boss.StrikeExplosion();
    }


    public void JumpSlashVFX(int index)
    {
        // ����
        boss.SoundCall(5);

        jumpSlashVFX[index].SetActive(true);
        boss.JumpSlashSwordAura();
    }


    public void QuadrVFX(int index)
    {
        // ����
        if (index == 0 || index == 3)
        {
            boss.SoundCall(6);
        }
        else
        {
            boss.SoundCall(6);
        }

        // ����Ʈ
        quadraVFX[index].SetActive(true);

        // �˱� �߻�
        boss.QuadrSwordAura(index);
    }

    public void UpwardVFX()
    {
        // ����
        boss.SoundCall(5);

        upwardVFX.SetActive(true);
        boss.UpwardExplosion();
    }


    public void ExplosionStrikeVFX()
    {
        // ����
        //boss.SoundCall(10);

        explosionStrikeVFX.SetActive(true);
        boss.ExplosionStrike();
    }
    #endregion
}
