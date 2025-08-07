using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAttack_BigShot : Attack_Base
{
    [Header("=== Attack Setting ===")]
    [SerializeField] private Enemy_Range range;
    private Enemy_Base enemyBase;
    private int refCount;

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
        range.
        // 공격 기능 동작 호출
        StartCoroutine(Attack());
    }

    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, List<GameObject> targetObj, bool isExchange, int attackCount)
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator Attack()
    {
        // 적 바라보기
        enemyBase.LookAt(target);

        // 적 방향 돌진
        enemyBase.Turn_AttackMove();
        while (enemyBase.isExchangeMove)
        {
            Debug.Log("에너미 샷건 공격 이동 대기");
            yield return null;
        }

        // 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isShotGun", true);

        // 공격 대기
        while(anim.GetBool("isShotGun"))
        {
            yield return null;
        }

        enemyBase.isAttack = false;
    }
}
