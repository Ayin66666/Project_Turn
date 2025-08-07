using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Tutorial_Animation : MonoBehaviour
{
    [Header("=== Setting ===")]
    [SerializeField] private Enemy_Base enemy;
    private Animator anim;


    [Header("=== VFX ===")]
    [SerializeField] private GameObject exchangeVFX;
    [SerializeField] private GameObject swingVFX;
    [SerializeField] private GameObject[] doubleSwingVFX;
    [SerializeField] private GameObject[] tripleSwingVFX;


    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    #region 기본 동작
    public void SpawnOver()
    {
        anim.SetBool("isSpawn", false);
    }

    // 사망 애니메이션 종료
    public void DieOver()
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

    // 백스텝 종료
    public void BackstepOver()
    {
        anim.SetBool("isBackstep", false);
    }
    #endregion


    #region 공격 동작
    public void AttackCall()
    {
        enemy.Attack_Call();
    }

    public void AttackOver()
    {
        anim.SetBool("isAttack", false);
    }

    public void SwingOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isSwing", false);
    }

    public void DoubleSwingOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isDoubleStrike", false);
    }

    public void TripleSwingOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isTripleCombo", false);
    }
    #endregion


    #region 이펙트
    public void ExchangeVFX()
    {
        // 사운드
        enemy.SoundCall(0);

        exchangeVFX.SetActive(true);
    }

    public void SwingVFX()
    {
        // 사운드
        enemy.SoundCall(2);

        swingVFX.SetActive(true);
    }

    public void DoubleSwingVFX(int index)
    {
        // 사운드
        enemy.SoundCall(2);

        doubleSwingVFX[index].SetActive(true);
    }

    public void TripleSwingVFX(int index)
    {
        // 사운드
        enemy.SoundCall(index == 2 ? 3 : 2);

        tripleSwingVFX[index].SetActive(true);
    }
    #endregion
}
