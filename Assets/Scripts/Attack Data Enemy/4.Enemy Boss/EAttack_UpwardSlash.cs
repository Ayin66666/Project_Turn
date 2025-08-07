using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAttack_UpwardSlash : Attack_Base
{
    [Header("=== Attack Setting ===")]
    private Enemy_Base enemyBase;
    private int refCount;

    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, List<GameObject> targetObj, bool isExchange, int attackCount)
    {
        throw new System.NotImplementedException();
    }

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

    private IEnumerator Attack()
    {
        // �� �ٶ󺸱�
        enemyBase.LookAt(target);

        if (anim != null)
        {
            anim.SetTrigger("Action");
            anim.SetBool("isAttack", true);
            anim.SetBool("isUpwardSlash", true);
            while (anim.GetBool("isAttack"))
            {
                yield return null;
            }
        }

        // �ִϸ��̼� ���
        if (anim != null)
        {
            while (anim.GetBool("isAttack"))
            {
                yield return null;
            }
        }

        // ���� ���� -> while �� ��
        enemyBase.isAttack = false;
    }
}
