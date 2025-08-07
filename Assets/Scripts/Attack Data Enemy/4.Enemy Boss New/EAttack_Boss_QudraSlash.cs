using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAttack_Boss_QudraSlash : Attack_Base
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
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        enemyBase.isAttack = true;

        // 적 바라보기
        enemyBase.LookAt(target);

        // 이동 위치 셋팅
        Vector3[] movePos = new Vector3[boss.qudraSlashPos.Length];
        for (int i = 0; i < boss.qudraSlashPos.Length; i++)
        {
            movePos[i] = boss.qudraSlashPos[i].position;
        }

        // 날개 이펙트
        boss.wingVFX.SetActive(true);

        // 딜레이
        yield return new WaitForSeconds(0.15f);

        // 공격
        int a = 0;
        Debug.Log(refCount);
        while(a < refCount)
        {
            // 타겟의 사망 체크
            if (Player_Manager.instnace.isDie)
            {
                anim.SetTrigger("Action");
                anim.SetBool("isAttack", false);
                while (anim.GetBool("isQuadraSlash"))
                {
                    yield return null;
                }

                enemyBase.isAttack = false;
                yield break;
            }
            else
            {
                // 텔레포트
                ownerObj.transform.position = movePos[a];
                Instantiate(boss.teleportVFX, ownerObj.transform.position, Quaternion.identity);
                boss.LookAt(target);

                // 공격
                anim.SetTrigger("Action");
                anim.SetBool("isAttack", true);
                anim.SetBool("isQuadraSlash", true);
                while (anim.GetBool("isAttack"))
                {
                    yield return null;
                }
            }
            a++;
        }

        // 날개 이펙트
        boss.wingVFX.SetActive(false);

        // 애니메이션 대기
        anim.SetTrigger("Action");
        while (anim.GetBool("isQuadraSlash"))
        {
            yield return null;
        }

        enemyBase.isAttack = false;
    }
}

