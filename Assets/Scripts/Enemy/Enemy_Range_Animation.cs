using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Range_Animation : MonoBehaviour
{
    [Header("=== Attack Effects ===")]
    [SerializeField] private GameObject exchangeVFX;
    [SerializeField] private GameObject swingVFX;
    [SerializeField] private GameObject normalShotVFX;
    [SerializeField] private GameObject bigShotVFX;


    [Header("=== Component ===")]
    [SerializeField] private Enemy_Base enemy;
    [SerializeField] private Enemy_Range range;
    private Animator anim;


    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    #region �⺻ ����
    public void HitAnim()
    {
        // anim.SetBool("isHit", false);
    }

    public void SpawnOver()
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

    // �齺�� ����
    public void BackstepOver()
    {
        anim.SetBool("isBackstep", false);
    }
    #endregion

    #region ���� ����
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

    public void ExchangeVFX()
    {
        enemy.SoundCall(0);
        exchangeVFX.SetActive(true);
    }

    // �ֵѱ� ����Ʈ
    public void SwingVFX()
    {
        enemy.SoundCall(3);
        swingVFX.SetActive(true);
    }

    // �ֵѱ� ����
    public void SwingOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isSwing", false);
    }

    // �븻 ��� ����Ʈ
    public void NormalShotVFX(int index)
    {
        normalShotVFX.SetActive(true);
        range.NormalShot(index);
    }

    // �븻 ��� ����
    public void NormalShotOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isNormalShot", false);
    }

    // ���� ����Ʈ
    public void BigShotVFX(int index)
    {
        bigShotVFX.SetActive(true);
        range.ShotGunShot(index);
    }

    // ���� ����
    public void BigshotOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isShotGun", false);
    }

    #endregion
}
