using Easing.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAttack_Strike : Attack_Base
{
    [Header("=== Attack Setting ===")]
    private Enemy_Base enemyBase;
    private Enemy_Boss enemyBoss;
    private GameObject ownerObj;
    private int refCount;


    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, GameObject targetObj, bool isExchange, int attackCount)
    {
        owner = attackOwner;

        this.ownerObj = ownerObj;

        enemyBase = ownerObj.GetComponent<Enemy_Base>();
        enemyBoss = ownerObj.GetComponent<Enemy_Boss>();

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

        // 공격 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isStrikeReady", true);
        anim.SetBool("isStrike", true);
        anim.SetFloat("AnimProgress", 0);

        // 준비 이펙트
        enemyBoss.chargeVFX.SetActive(true);

        // 준비 애니메이션
        while (anim.GetBool("isStrikeReady"))
        {
            yield return null;
        }

        // 준비 이펙트
        enemyBoss.chargeVFX.SetActive(false);

        // 일시 대기
        yield return new WaitForSeconds(0.1f);
        anim.SetTrigger("Action");

        // 돌진 기능
        Vector3 startPos = ownerObj.transform.position;
        Vector3 endPos = Player_Manager.instnace.player_Turn.exchangePos.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 2.2f;
            anim.SetFloat("AnimProgress", timer);
            ownerObj.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        ownerObj.transform.position = endPos;

        anim.SetFloat("AnimProgress", 1);
        anim.SetTrigger("Action");

        // 애니메이션 대기
        while (anim.GetBool("isStrike"))
        {
            yield return null;
        }

        enemyBase.isAttack = false;
    }
}
