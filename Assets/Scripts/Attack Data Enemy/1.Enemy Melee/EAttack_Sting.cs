using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing.Tweening;

public class EAttack_Sting : Attack_Base
{
    [Header("=== Attack Setting ===")]
    private Enemy_Base enemyBase;
    private GameObject ownerObj;
    private int refCount;


    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, GameObject targetObj, bool isExchange, int attackCount)
    {
        owner = attackOwner;

        this.ownerObj = ownerObj;

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
        // ���� �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isSting", true);
        anim.SetFloat("AnimProgress", 0);

        // �� �ٶ󺸱�
        enemyBase.LookAt(target);

        // ���� ���
        Vector3 startPos = ownerObj.transform.position;
        Vector3 endPos = Player_Manager.instnace.player_Turn.exchangePos.position;
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime;
            anim.SetFloat("AnimProgress", EasingFunctions.OutExpo(timer));
            ownerObj.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        ownerObj.transform.position = Vector3.Lerp(startPos, endPos, 1);
        anim.SetFloat("AnimProgress", 1);


        // �ִϸ��̼� ���
        while (anim.GetBool("isSting"))
        {
            yield return null;
        }
        enemyBase.isAttack = false;
        // ���� ��ġ�� ���� -> �̰� ���ݿ� �ִ°� �´°ǰ�? �ƴϸ� ���� ������ ���ʿ��� �����ϴ°� �´°ǰ�?
    }
}
