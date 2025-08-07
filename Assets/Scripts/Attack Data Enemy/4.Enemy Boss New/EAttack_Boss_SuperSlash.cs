using Easing.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAttack_Boss_SuperSlash : Attack_Base
{
    [Header("=== Attack Setting ===")]
    [SerializeField] private Enemy_Boss boss;
    private Enemy_Base enemyBase;
    private GameObject ownerObj;
    private int refCount;

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
        boss.isAttack = true;
        int a = 0;

        // 백스탭
        anim.SetTrigger("Action");
        anim.SetBool("isBackstep", true);
        boss.LookAt(target);
        Vector3 startPos = ownerObj.transform.position;
        Vector3 endPos = boss.backstepPos.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 2.5f;
            ownerObj.transform.position = Vector3.Lerp(startPos, endPos, timer);
            yield return null;
        }
        anim.SetBool("isBackstep", false);

        // 사운드
        boss.SoundCall(10);

        // 차징 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isSuperSlashCharge", true);
        anim.SetBool("isSuperSlash", true);

        // 딜레이
        boss.chargeVFX.SetActive(true);
        while(anim.GetBool("isSuperSlashCharge"))
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        boss.chargeVFX.SetActive(false);


        // 1번 공격 - 올려베기
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isSuperSlash", true);
        while (anim.GetBool("isAttack"))
        {
            yield return null;
        }
        a++;


        // 2번 공격 - 검기 난무
        if (a < refCount)
        {
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

            for (int i = 0; i < 4; i++)
            {
                // 타겟의 사망 체크
                if (Player_Manager.instnace.isDie)
                {
                    anim.SetTrigger("Action");
                    anim.SetBool("isAttack", false);
                    while (anim.GetBool("isSuperSlash"))
                    {
                        yield return null;
                    }

                    enemyBase.isAttack = false;
                    yield break;
                }
                else
                {
                    // 텔레포트
                    ownerObj.transform.position = movePos[i];
                    Instantiate(boss.teleportVFX, ownerObj.transform.position, Quaternion.identity);
                    boss.LookAt(target);

                    // 공격
                    anim.SetTrigger("Action");
                    anim.SetBool("isAttack", true);
                    anim.SetBool("isSuperSlash", true);
                    while (anim.GetBool("isAttack"))
                    {
                        yield return null;
                    }

                    a++;
                }
            }
            // 날개 이펙트
            boss.wingVFX.SetActive(false);
        }

        // 3번 공격 - 스트라이크
        if(a < refCount)
        {
            // 적 바라보기
            enemyBase.LookAt(target);

            // 공격 애니메이션
            anim.SetTrigger("Action");
            anim.SetBool("isAttack", true);
            anim.SetBool("isSuperSlashCharge", true);
            anim.SetBool("isSuperSlash", true);
            anim.SetFloat("AnimProgress", 0);

            // 사운드
            boss.SoundCall(10);

            // 준비 이펙트
            boss.wingVFX.SetActive(true);
            boss.chargeVFX.SetActive(true);

            // 딜레이
            while (anim.GetBool("isSuperSlashCharge"))
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
            boss.chargeVFX.SetActive(false);

            // 돌진 기능
            anim.SetTrigger("Action");
            anim.SetBool("isAttack", true);
            anim.SetBool("isSuperSlash", true);
            startPos = ownerObj.transform.position;
            endPos = Player_Manager.instnace.player_Turn.exchangePos.position;
            timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime * 1.5f;
                anim.SetFloat("AnimProgress", EasingFunctions.OutExpo(timer));
                ownerObj.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
                yield return null;
            }
            ownerObj.transform.position = endPos;
            anim.SetFloat("AnimProgress", 1);
            while (anim.GetBool("isSuperSlash"))
            {
                yield return null;
            }

            boss.wingVFX.SetActive(false);
            yield return new WaitForSeconds(0.25f);
        }

        // 공격 종료
        anim.SetTrigger("Action");
        while (anim.GetBool("isSuperSlash"))
        {
            yield return null;
        }

        // 공격 종료 -> while 문 밖
        enemyBase.isAttack = false;
    }
}
