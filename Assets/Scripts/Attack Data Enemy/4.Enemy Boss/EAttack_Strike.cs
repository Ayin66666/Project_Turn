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
        // �� �ٶ󺸱�
        enemyBase.LookAt(target);

        // ���� �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isStrikeReady", true);
        anim.SetBool("isStrike", true);
        anim.SetFloat("AnimProgress", 0);

        // �غ� ����Ʈ
        enemyBoss.chargeVFX.SetActive(true);

        // �غ� �ִϸ��̼�
        while (anim.GetBool("isStrikeReady"))
        {
            yield return null;
        }

        // �غ� ����Ʈ
        enemyBoss.chargeVFX.SetActive(false);

        // �Ͻ� ���
        yield return new WaitForSeconds(0.1f);
        anim.SetTrigger("Action");

        // ���� ���
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

        // �ִϸ��̼� ���
        while (anim.GetBool("isStrike"))
        {
            yield return null;
        }

        enemyBase.isAttack = false;
    }
}
