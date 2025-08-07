using Easing.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAttack_JumpSlash : Attack_Base
{
    [Header("=== Attack Setting ===")]
    private Enemy_Base enemyBase;
    private Enemy_Boss enemyBoss;
    private GameObject ownerObj;
    private int refCount;


    // 사용 안함
    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, List<GameObject> targetObj, bool isExchange, int attackCount)
    {
        throw new System.NotImplementedException();
    }

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

    private IEnumerator Attack()
    {
        // 바라보기
        enemyBase.LookAt(target);

        // 공격 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isJumpSlashReady", true);
        anim.SetBool("isJumpSlash", true);
        anim.SetFloat("AnimProgress", 0);

        // 준비 애니메이션 대기
        while (anim.GetBool("isJumpSlashReady"))
        {
            yield return null;
        }

        // 돌진
        Vector3 startPos = ownerObj.transform.position;
        Vector3 endPos = Player_Manager.instnace.player_Turn.exchangePos.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.5f;
            anim.SetFloat("AnimProgress", timer);
            //ownerObj.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            ownerObj.transform.position = Vector3.Lerp(startPos, endPos, timer);
            yield return null;
        }
        ownerObj.transform.position = endPos;

        anim.SetFloat("AnimProgress", 1);
        anim.SetTrigger("Action");

        // 애니메이션 대기
        while (anim.GetBool("isJumpSlash"))
        {
            yield return null;
        }

        enemyBase.isAttack = false;
    }
}
