using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Easing.Tweening;


public class EAttack_DoubleStrike : Attack_Base
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
        // �� �ٶ󺸱�
        enemyBase.LookAt(target);

        // �� ���� ����
        enemyBase.Turn_AttackMove();
        while(enemyBase.isExchangeMove)
        {
            yield return null;
        }

        // ����
        int a = 0;
        while (a < refCount)
        {
            // Ÿ���� ��� üũ
            if (Player_Manager.instnace.isDie)
            {
                anim.SetTrigger("Action");
                anim.SetBool("isAttack", false);
                anim.SetBool("isDoubleStrike", false);
                while (anim.GetBool("isDoubleStrike"))
                {
                    yield return null;
                }

                enemyBase.isAttack = false;
                yield break;
            }
            else
            {
                anim.SetTrigger("Action");
                anim.SetBool("isAttack", true);
                anim.SetBool("isDoubleStrike", true);

                // �ִϸ��̼� ȣ��
                while (anim.GetBool("isAttack"))
                {
                    yield return null;
                }

                // ���� Ƚ�� ���
                a++;
            }
        }
        anim.SetTrigger("Action");
        anim.SetBool("isDoubleStrike", false);
        while (anim.GetBool("isDoubleStrike"))
        {
            yield return null;
        }

        // ���� ���� -> while �� ��
        enemyBase.isAttack = false;
    }
}
