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


    #region 기본 이벤트
    // 스폰 애니메이션 종료
    public void SpawnAnim()
    {
        anim.SetBool("isSpawn", false);
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

    #region 공격 이벤트
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

    // 이연타 이펙트
    public void DoubleVFX(int index)
    {
        enemy.SoundCall(2);
        double_strikeVFX[index].SetActive(true);
    }

    // 이연타 종료
    public void DoubleOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isDoubleStrike", false);
    }

    // 합 이펙트
    public void ExchangeVFX()
    {
        enemy.SoundCall(0);
        exchangeVFX.SetActive(true);
    }

    // 찌르기 이펙트
    public void StingVFX()
    {
        enemy.SoundCall(3);
        stingVFX.SetActive(true);
    }

    // 찌르기 종료
    public void StingOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isSting", false);
    }

    // 화염방사 이펙트
    public void FlamethrowerVFX()
    {
        enemy.SoundCall(4);
        flamethrowerVFX.SetActive(!flamethrowerVFX.activeSelf);
    }

    // 화염방사 종료
    public void FlamethrowerOver()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isFlamethrower", false);
    }
    #endregion
}
