using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Enemy_Elite_Animation : MonoBehaviour
{
    [Header("=== Component ===")]
    [SerializeField] private Enemy_Elite eilte;
    [SerializeField] private Enemy_Base enemy;
    private Animator anim;


    [Header("=== VFX ===")]
    [SerializeField] private GameObject exchangeVFX;
    [SerializeField] private GameObject[] tripleAttackVFX;
    [SerializeField] private GameObject[] tripleShotVFX;
    [SerializeField] private GameObject[] doubleStrikeVFX;


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


    #region 공격 이벤트
    public void AttackCall()
    {
        enemy.Attack_Call();
    }

    // 일단 공격이 종료할때
    public void AttackOver()
    {
        anim.SetBool("isAttack", false);
    }

    public void TripleAttackOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isTripleCombo", false);
    }

    public void TripleShotOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isTripleShot", false);
    }

    public void DoubleStrikeOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isDoubleStrike", false);
    }
    #endregion


    #region 공격 이펙트
    public void ExchangeVFX()
    {
        eilte.SoundCall(0);
        exchangeVFX.SetActive(true);
    }

    public void TripleAttackVFX(int index)
    {
        eilte.SoundCall(index == 2 ? 3 : 2);
        tripleAttackVFX[index].SetActive(true);
    }

    private void TripleShotLineVFX()
    {
        eilte.Line(true);
    }

    public void TripleShotVFX(int index)
    {
        // 조준 라인
        eilte.Line(false);

        // 사격
        eilte.Shot(index == 2 ? true : false, index);

        // 사운드
        eilte.SoundCall(5);

        // 이펙트
        if (tripleShotVFX[index].activeSelf)
            tripleShotVFX[index].SetActive(false);

        tripleShotVFX[index].SetActive(true);
    }

    public void DoubleStrikeVFX(int index)
    {
        doubleStrikeVFX[index].SetActive(true);
        eilte.SoundCall(index == 0 ? 3 : 4);
    }
    #endregion
}
