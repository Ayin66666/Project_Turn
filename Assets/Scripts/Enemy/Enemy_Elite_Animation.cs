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


    #region �⺻ �̺�Ʈ
    // ���� �ִϸ��̼� ����
    public void SpawnAnim()
    {
        anim.SetBool("isSpawn", false);
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


    #region ���� �̺�Ʈ
    public void AttackCall()
    {
        enemy.Attack_Call();
    }

    // �ϴ� ������ �����Ҷ�
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


    #region ���� ����Ʈ
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
        // ���� ����
        eilte.Line(false);

        // ���
        eilte.Shot(index == 2 ? true : false, index);

        // ����
        eilte.SoundCall(5);

        // ����Ʈ
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
