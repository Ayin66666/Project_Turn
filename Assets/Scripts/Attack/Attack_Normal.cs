using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Normal : Attack_Base
{
    [Header("=== Attack Setting ===")]
    [SerializeField] private Animator anim = null;
    [SerializeField] private Enemy_Base enemy = null;
    private GameObject target;
    private int refCount;

    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, GameObject targetObj, int attackCount)
    {
        // 애니메이터 할당
        switch (attackOwner)
        {
            case AttackOwner.Player:
                anim = Player_Manager.instnace.player_Turn.anim;
                Player_Manager.instnace.isAttack = true;
                break;

            case AttackOwner.Enemy:
                enemy = ownerObj.GetComponent<Enemy_Base>();
                anim = enemy.anim;
                break;
        }

        // 타겟 셋팅
        target = targetObj;


        // 남은 공격 횟수 셋팅
        refCount = attackCount;

        // 공격 기능 동작 호출
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        Debug.Log("공격 호출!!!!");

        Player_Manager.instnace.isAttack = true;

        // 남은 공격 횟수 체크 후 동작
        for (int i = 0; i < attackCount - refCount; i++)
        {
            if(anim != null)
            {
                anim.SetTrigger("Attack");
                anim.SetBool("isNormalAttack", true);
                while (anim.GetBool("isNormalAttack"))
                {
                    yield return null;
                }
            }
            Debug.Log("일반 공격 " + i + " 회 !");
            yield return new WaitForSeconds(0.1f);
        }

        Player_Manager.instnace.isAttack = false;
    }
}
