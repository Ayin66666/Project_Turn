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


    #region �⺻ ����
    public void SpawnOver()
    {
        anim.SetBool("isSpawn", false);
    }

    // ��� �ִϸ��̼� ����
    public void DieOver()
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

    // �齺�� ����
    public void BackstepOver()
    {
        anim.SetBool("isBackstep", false);
    }
    #endregion


    #region ���� ����
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


    #region ����Ʈ
    public void ExchangeVFX()
    {
        // ����
        enemy.SoundCall(0);

        exchangeVFX.SetActive(true);
    }

    public void SwingVFX()
    {
        // ����
        enemy.SoundCall(2);

        swingVFX.SetActive(true);
    }

    public void DoubleSwingVFX(int index)
    {
        // ����
        enemy.SoundCall(2);

        doubleSwingVFX[index].SetActive(true);
    }

    public void TripleSwingVFX(int index)
    {
        // ����
        enemy.SoundCall(index == 2 ? 3 : 2);

        tripleSwingVFX[index].SetActive(true);
    }
    #endregion
}
