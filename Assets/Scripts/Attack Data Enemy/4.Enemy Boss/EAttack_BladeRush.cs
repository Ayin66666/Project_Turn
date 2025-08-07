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

        // �ִϸ����� �Ҵ�
        anim = enemyBase.anim;
        enemyBase.isAttack = true;

        // Ÿ�� ����
        target = targetObj;

        // ���� ���� Ƚ�� ����
        refCount = attackCount;

        // ���� ��� ���� ȣ��
        StartCoroutine(Attack());
    }

    // ��� ����
    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, List<GameObject> targetObj, bool isExchange, int attackCount)
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator Attack()
    {
        int a = 0;

        // �� �ٶ󺸱�
        enemyBase.LookAt(target);

        // �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isBladeRushReady", true);
        anim.SetBool("isBladeRush", true);
        anim.SetFloat("AnimProgress", 0);

        // ���� �ִϸ��̼� ���
        while(anim.GetBool("isBladeRushReady"))
        {
            yield return null;
        }

        // 1Ÿ ���� - �� ���� ����
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

        // 1Ÿ ����
        enemyBase.Attack_Call();


        // Ÿ���� ��� üũ
        if (Player_Manager.instnace.isDie)
        {
            anim.SetTrigger("Action");
            anim.SetBool("isAttack", false);
            anim.SetBool("isBladeRush", false);
            enemyBase.isAttack = false;
        }
        else
        {
            // 2Ÿ ���� - 360�� ���� ����
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

            // ���� ���� -> while �� ��
            enemyBase.isAttack = false;
        }
    }
}
