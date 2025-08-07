using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAttack_Elite_TripleShot : Attack_Base
{
    [Header("=== Attack Setting ===")]
    [SerializeField] private Enemy_Elite elite;
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
        anim.SetInteger("Count", 0);

        // 공격
        int a = 0;
        while (a < refCount)
        {
            // 타겟의 사망 체크
            if (Player_Manager.instnace.isDie)
            {
                // 애니메이션
                anim.SetTrigger("Action");
                anim.SetBool("isAttack", false);
                while (anim.GetBool("isTripleShot"))
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
                anim.SetBool("isTripleShot", true);
                anim.SetInteger("Count", a);


                // 애니메이션 호출
                while (anim.GetBool("isAttack"))
                {
                    yield return null;
                }

                // 공격 횟수 상승
                a++;
            }

            // 공격 딜레이
            yield return new WaitForSeconds(0.15f);
        }

        // 공격 딜레이
        yield return new WaitForSeconds(0.1f);

        // 조준 라인
        elite.Line(false);

        anim.SetTrigger("Action");
        while (anim.GetBool("isTripleShot"))
        {
            yield return null;
        }

        // 공격 종료 -> while 문 밖
        enemyBase.isAttack = false;
    }
}
