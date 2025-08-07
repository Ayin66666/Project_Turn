using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing.Tweening;

public class EAttack_Flamethrower : Attack_Base
{
    [Header("=== Attack Setting ===")]
    private Enemy_Base enemyBase;
    private GameObject ownerObj;
    private int count; // 화방 공격 남은 횟수 -> 이건 이벤트말고 타이머로 호출!
    private int refCount;

    [SerializeField] private GameObject attackVFX;

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
        // 적 바라보기
        enemyBase.LookAt(target);

        // 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isFlamethrower", true);
        anim.SetFloat("AnimProgress", 0);

        // 데미지
        StartCoroutine(Damage());

        // 애니메이션 동작
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime;
            anim.SetFloat("AnimProgress", timer);
            yield return null;
        }
        anim.SetFloat("AnimProgress", 1);

        // 애니메이션 대기
        while(anim.GetBool("isFlamethrower"))
        {
            yield return null;
        }

        // 공격 종료 -> while 문 밖
        enemyBase.isAttack = false;
    }

    IEnumerator Damage()
    {
        // 데미지 부여
        for (int i = 0; i < refCount; i++)
        {
            // 데미지 계산
            (int pdamage, bool isCritical) = enemyBase.DamageCal(this, i);

            // 데미지 전달
            Player_Manager.instnace.Take_Damage(pdamage, damageType[i] == DamageType.physical ? Player_Manager.DamageType.Physical : Player_Manager.DamageType.Magical, isCritical);

            yield return new WaitForSeconds(1 / refCount);
        }
    }
}
