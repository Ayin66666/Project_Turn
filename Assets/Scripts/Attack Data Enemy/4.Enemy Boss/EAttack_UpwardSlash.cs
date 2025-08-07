using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAttack_UpwardSlash : Attack_Base
{
    [Header("=== Attack Setting ===")]
    private Enemy_Base enemyBase;
    private int refCount;

    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, List<GameObject> targetObj, bool isExchange, int attackCount)
    {
        throw new System.NotImplementedException();
    }

    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, GameObject targetObj, bool isExchange, int attackCount)
    {
        owner = attackOwner;

        enemyBase = ownerObj.GetComponent<Enemy_Base>();

        // 애니메이터 할당
        anim = enemyBase.anim;
        enemyBase.isAttack = true;

        // 타겟 셋팅
        target = targetObj;

        // 남은 공격 횟수 셋팅
        refCount = attackCount;

        // 공격 기능 동작 호출
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        // 적 바라보기
        enemyBase.LookAt(target);

        if (anim != null)
        {
            anim.SetTrigger("Action");
            anim.SetBool("isAttack", true);
            anim.SetBool("isUpwardSlash", true);
            while (anim.GetBool("isAttack"))
            {
                yield return null;
            }
        }

        // 애니메이션 대기
        if (anim != null)
        {
            while (anim.GetBool("isAttack"))
            {
                yield return null;
            }
        }

        // 공격 종료 -> while 문 밖
        enemyBase.isAttack = false;
    }
}
