using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAttack_NormalShot : Attack_Base
{
    [Header("=== Attack Setting ===")]
    [SerializeField] private Enemy_Range enemy;
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

    // 사용 안함
    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, List<GameObject> targetObj, bool isExchange, int attackCount)
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator Attack()
    {
        // 적 바라보기
        enemyBase.LookAt(target);

        // 남은 공격 횟수 체크 후 동작
        int a = 0;
        while (a < refCount)
        {
            anim.SetTrigger("Action");
            anim.SetBool("isAttack", true);
            anim.SetBool("isNormalShot", true);
            while (anim.GetBool("isAttack"))
            {
                yield return null;
            }

            // 타겟의 사망 체크
            if (Player_Manager.instnace.isDie)
            {
                // 애니메이션 종료
                anim.SetTrigger("Action");
                anim.SetBool("isAttack", false);
                anim.SetBool("isNormalShot", false);
                while (anim.GetBool("isNormalShot"))
                {
                    yield return null;
                }

                // 공격 종료
                enemyBase.isAttack = false;
                yield break;
            }

            // 애니메이션 대기
            while (anim.GetBool("isAttack"))
            {
                yield return null;
            }

            Debug.Log("일반 공격 " + a + " 회 !");
            a++;

            // 다음 공격 일시 대기
            yield return new WaitForSeconds(0.15f);
        }

        // 애니메이션 종료
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", false);
        anim.SetBool("isNormalShot", false);
        while (anim.GetBool("isNormalShot"))
        {
            yield return null;
        }

        // 공격 종료 -> while 문 밖
        enemyBase.isAttack = false;
    }
}
