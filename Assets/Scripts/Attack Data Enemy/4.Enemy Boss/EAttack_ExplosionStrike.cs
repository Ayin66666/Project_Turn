using Easing.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAttack_ExplosionStrike : Attack_Base
{
    [Header("=== Attack Setting ===")]
    [SerializeField] private Enemy_Base enemyBase;
    [SerializeField] private Enemy_Boss boss;
    private GameObject ownerObj;
    private int refCount;


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
        StartCoroutine(Attack());
    }

    // 사용 안함
    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, List<GameObject> targetObj, bool isExchange, int attackCount)
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator Attack()
    {
        // 공격 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isExplosionStrike", true);
        anim.SetFloat("AnimProgress", 0);

        // 적 바라보기
        enemyBase.LookAt(target);

        // 돌진 기능
        Vector3 startPos = ownerObj.transform.position;
        Vector3 endPos = Player_Manager.instnace.player_Turn.exchangePos.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            anim.SetFloat("AnimProgress", timer);
            ownerObj.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        ownerObj.transform.position = endPos;
        anim.SetFloat("AnimProgress", 1);

        // 추가타
        boss.ExplosionStrike();

        // 애니메이션 대기
        while (anim.GetBool("isExplosionStrike"))
        {
            yield return null;
        }

        enemyBase.isAttack = false;
    }
}
