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

    #region �⺻ �ִϸ��̼�
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


    #region �׽�Ʈ�� ����
    public void NormalAttackAnim()
    {
        anim.SetBool("isNormalAttack", false);
    }
    #endregion
}
