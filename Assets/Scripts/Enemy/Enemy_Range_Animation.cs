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


    #region 기본 동작
    public void HitAnim()
    {
        // anim.SetBool("isHit", false);
    }

    public void SpawnOver()
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

    // 백스텝 종료
    public void BackstepOver()
    {
        anim.SetBool("isBackstep", false);
    }
    #endregion

    #region 공격 동작
    public void AttackCall()
    {
        // 미리 데이터가 들어가 있어야만 제대로 동작함!
        enemy.Attack_Call();
    }

    // 일단 공격이 종료할때
    public void AttackOver()
    {
        anim.SetBool("isAttack", false);
    }

    public void ExchangeVFX()
    {
        enemy.SoundCall(0);
        exchangeVFX.SetActive(true);
    }

    // 휘둘기 이펙트
    public void SwingVFX()
    {
        enemy.SoundCall(3);
        swingVFX.SetActive(true);
    }

    // 휘둘기 종료
    public void SwingOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isSwing", false);
    }

    // 노말 사격 이펙트
    public void NormalShotVFX(int index)
    {
        normalShotVFX.SetActive(true);
        range.NormalShot(index);
    }

    // 노말 사격 종료
    public void NormalShotOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isNormalShot", false);
    }

    // 샷건 이펙트
    public void BigShotVFX(int index)
    {
        bigShotVFX.SetActive(true);
        range.ShotGunShot(index);
    }

    // 샷건 종료
    public void BigshotOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isShotGun", false);
    }

    #endregion
}
