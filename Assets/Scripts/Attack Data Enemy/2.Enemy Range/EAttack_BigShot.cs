using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAttack_BigShot : Attack_Base
{
    [Header("=== Attack Setting ===")]
    [SerializeField] private Enemy_Range range;
    private Enemy_Base enemyBase;
    private int refCount;

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
        range.
        // ���� ��� ���� ȣ��
        StartCoroutine(Attack());
    }

    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, List<GameObject> targetObj, bool isExchange, int attackCount)
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator Attack()
    {
        // �� �ٶ󺸱�
        enemyBase.LookAt(target);

        // �� ���� ����
        enemyBase.Turn_AttackMove();
        while (enemyBase.isExchangeMove)
        {
            Debug.Log("���ʹ� ���� ���� �̵� ���");
            yield return null;
        }

        // �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isShotGun", true);

        // ���� ���
        while(anim.GetBool("isShotGun"))
        {
            yield return null;
        }

        enemyBase.isAttack = false;
    }
}
