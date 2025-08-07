using Easing.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAttack_QuadraCombo : Attack_Base
{
    [Header("=== Attack Setting ===")]
    [SerializeField] private Enemy_Base enemyBase;
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
        // �� �ٶ󺸱�
        enemyBase.LookAt(target);

        // �̵� ȣ��
        enemyBase.Turn_AttackMove();
        while (enemyBase.isExchangeMove)
        {
            yield return null;
        }

        int a = 0;
        while (a < refCount)
        {
            // Ÿ���� ��� üũ
            if (Player_Manager.instnace.isDie)
            {
                anim.SetTrigger("Action");
                anim.SetBool("isAttack", false);
                anim.SetBool("isQuadraCombo", false);
                while (anim.GetBool("isQuadraCombo"))
                {
                    yield return null;
                }

                enemyBase.isAttack = false;
                yield break;
            }

            // �ִϸ��̼� ȣ��
            anim.SetTrigger("Action");
            anim.SetBool("isAttack", true);
            anim.SetBool("isQuadraCombo", true);

            // �ִϸ��̼� ���
            while (anim.GetBool("isAttack"))
            {
                yield return null;
            }

            // ���� Ƚ�� ���
            a++;
        }

        // ���� ����
        anim.SetTrigger("Action");
        anim.SetBool("isQuadraCombo", false);
        while (anim.GetBool("isQuadraCombo"))
        {
            yield return null;
        }

        // ���� ���� -> while �� ��
        enemyBase.isAttack = false;
    }
}
