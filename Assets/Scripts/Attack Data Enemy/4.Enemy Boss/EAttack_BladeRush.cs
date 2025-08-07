using Easing.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAttack_BladeRush: Attack_Base
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

    // 사용 안함
    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, List<GameObject> targetObj, bool isExchange, int attackCount)
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator Attack()
    {
        int a = 0;

        // 적 바라보기
        enemyBase.LookAt(target);

        // 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isBladeRushReady", true);
        anim.SetBool("isBladeRush", true);
        anim.SetFloat("AnimProgress", 0);

        // 차지 애니메이션 대기
        while(anim.GetBool("isBladeRushReady"))
        {
            yield return null;
        }

        // 1타 공격 - 적 방향 돌진
        Vector3 startPos = enemyBase.gameObject.transform.position;
        Vector3 endPos = Player_Manager.instnace.player_Turn.exchangePos.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 2.5f;
            anim.SetFloat("AnimProgress", timer);
            enemyBase.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        enemyBase.transform.position = endPos;
        a++;

        // 1타 공격
        enemyBase.Attack_Call();


        // 타겟의 사망 체크
        if (Player_Manager.instnace.isDie)
        {
            anim.SetTrigger("Action");
            anim.SetBool("isAttack", false);
            anim.SetBool("isBladeRush", false);
            enemyBase.isAttack = false;
        }
        else
        {
            // 2타 공격 - 360도 베기 공격
            if (a < refCount)
            {
                yield return new WaitForSeconds(0.25f);

                anim.SetTrigger("Action");
                anim.SetBool("isAttack", true);
                while (anim.GetBool("isAttack"))
                {
                    yield return null;
                }
            }
            else
            {

                anim.SetBool("isAttack", false);
                while(anim.GetBool("isBladeRush"))
                {
                    yield return null;
                }
            }

            // 공격 종료 -> while 문 밖
            enemyBase.isAttack = false;
        }
    }
}
