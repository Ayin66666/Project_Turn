using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing.Tweening;

public class EAttack_Boss_TripleSlash : Attack_Base
{
    [Header("=== Setting ===")]
    [SerializeField] private Enemy_Base enemyBase;
    [SerializeField] private Enemy_Boss boss;
    [SerializeField] private int refCount;
    private GameObject ownerObj;
    private Coroutine attackCoroutine;

    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, List<GameObject> targetObj, bool isExchange, int attackCount)
    {
        throw new System.NotImplementedException();
    }

    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, GameObject targetObj, bool isExchange, int attackCount)
    {
        owner = attackOwner;

        this.ownerObj = ownerObj;

        enemyBase = ownerObj.GetComponent<Enemy_Base>();

        // 애니메이터 할당
        anim = enemyBase.anim;
        enemyBase.isAttack = true;

        // 타겟 셋팅
        target = targetObj;

        // 남은 공격 횟수 셋팅
        refCount = attackCount;

        // 공격 기능 동작 호출

        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);

        attackCoroutine = StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        enemyBase.isAttack = true;

        // 적 바라보기
        enemyBase.LookAt(target);

        // 이동 호출
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
                while (anim.GetBool("isTripleCombo"))
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
                anim.SetBool("isTripleCombo", true);

                // 애니메이션 호출
                while (anim.GetBool("isAttack"))
                {
                    yield return null;
                }

                // 공격 횟수 상승
                a++;
            }
        }

        // 애니메이션 대기
        while (anim.GetBool("isTripleCombo"))
        {
            yield return null;
        }

        enemyBase.isAttack = false;
    }
}
