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
        anim.SetBool("isExplosionStrike", true);
        anim.SetFloat("AnimProgress", 0);

        // �� �ٶ󺸱�
        enemyBase.LookAt(target);

        // ���� ���
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

        // �߰�Ÿ
        boss.ExplosionStrike();

        // �ִϸ��̼� ���
        while (anim.GetBool("isExplosionStrike"))
        {
            yield return null;
        }

        enemyBase.isAttack = false;
    }
}
