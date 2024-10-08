using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Turn_Animation : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    #region 기본 애니메이션
    public void SpawnAnim()
    {
        anim.SetBool("isSpawn", false);
    }

    public void ExchangeMoveAnim()
    {
        anim.SetBool("isExchangeMove", false);
    }

    public void HitAnim()
    {
        anim.SetBool("isHit", false);
    }

    public void DieAnim()
    {
        anim.SetBool("isDie", false);
    }
    #endregion


    #region 테스트용 공격
    public void NormalAttackAnim()
    {
        anim.SetBool("isNormalAttack", false);
    }
    #endregion
}
