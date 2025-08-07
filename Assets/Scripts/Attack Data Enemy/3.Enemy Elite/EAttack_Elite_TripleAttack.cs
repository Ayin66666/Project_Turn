using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAttack_Elite_TripleAttack : Attack_Base // 이유는 모르겠는데 이슈가 많은 공격;;
{
    [Header("=== Attack Setting ===")]
    private Enemy_Base enemyBase;
    [SerializeField] private int refCount;

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

        // 적 방향 돌진
        enemyBase.Turn_AttackMove();
        while (enemyBase.isExchangeMove)
        {
            yield return null;
        }

        // 공격
        int a = 0;
        while (a < refCount)
        {
            // 타겟의 사망 체크
            if (Player_Manager.instnace.isDie)
            {
                anim.SetTrigger("Action");
                anim.SetBool("isAttack", false);
                while (anim.GetBool("isDoubleStrike"))
                {
                    yield return null;
                }

                enemyBase.isAttack = false;
                yield break;
            }
            else
            {
                anim.SetTrigger("Action");
                anim.SetBool("isAttack", true);
                anim.SetBool("isDoubleStrike", true);
                while (anim.GetBool("isAttack"))
                {
                    yield return null;
                }

                // 공격 횟수 상승
                a++;
            }
        }

        anim.SetTrigger("Action");
        while (anim.GetBool("isDoubleStrike"))
        {
            yield return null;
        }

        // 공격 종료 -> while 문 밖
        enemyBase.isAttack = false;
    }
    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, List<GameObject> targetObj, bool isExchange, int attackCount)
    {
        throw new System.NotImplementedException();
    }
}
