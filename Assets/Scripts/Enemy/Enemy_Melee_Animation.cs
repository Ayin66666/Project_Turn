using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Melee_Animation : MonoBehaviour
{
    [Header("=== Attack Effects ===")]
    [SerializeField] private GameObject[] double_strikeVFX;
    [SerializeField] private GameObject stingVFX;
    [SerializeField] private GameObject flamethrowerVFX;
    [SerializeField] private GameObject exchangeVFX;

    
    [Header("=== Component ===")]
    [SerializeField] private Enemy_Base enemy;
    private Animator anim;


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

    #region ���� �̺�Ʈ
    public void AttackCall()
    {
        // �̸� �����Ͱ� �� �־�߸� ����� ������!
        enemy.Attack_Call();
    }

    // �ϴ� ������ �����Ҷ�
    public void AttackOver()
    {
        anim.SetBool("isAttack", false);
    }

    // �̿�Ÿ ����Ʈ
    public void DoubleVFX(int index)
    {
        enemy.SoundCall(2);
        double_strikeVFX[index].SetActive(true);
    }

    // �̿�Ÿ ����
    public void DoubleOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isDoubleStrike", false);
    }

    // �� ����Ʈ
    public void ExchangeVFX()
    {
        enemy.SoundCall(0);
        exchangeVFX.SetActive(true);
    }

    // ��� ����Ʈ
    public void StingVFX()
    {
        enemy.SoundCall(3);
        stingVFX.SetActive(true);
    }

    // ��� ����
    public void StingOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isSting", false);
    }

    // ȭ����� ����Ʈ
    public void FlamethrowerVFX()
    {
        enemy.SoundCall(4);
        flamethrowerVFX.SetActive(!flamethrowerVFX.activeSelf);
    }

    // ȭ����� ����
    public void FlamethrowerOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isFlamethrower", false);
    }
    #endregion
}
