using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing.Tweening;

public class EAttack_Boss_TripleSlash : Attack_Base
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

        // �ִϸ����� �Ҵ�
        anim = enemyBase.anim;
        enemyBase.isAttack = true;

        // Ÿ�� ����
        target = targetObj;

        // ���� ���� Ƚ�� ����
        refCount = attackCount;

        // ���� ��� ���� ȣ��

        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);

        attackCoroutine = StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        enemyBase.isAttack = true;

        // �� �ٶ󺸱�
        enemyBase.LookAt(target);

        // �̵� ȣ��
        enemyBase.Turn_AttackMove();
        while (enemyBase.isExchangeMove)
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
                while (anim.GetBool("isTripleCombo"))
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
                anim.SetBool("isTripleCombo", true);

                // �ִϸ��̼� ȣ��
                while (anim.GetBool("isAttack"))
                {
                    yield return null;
                }

                // ���� Ƚ�� ���
                a++;
            }
        }

        // �ִϸ��̼� ���
        while (anim.GetBool("isTripleCombo"))
        {
            yield return null;
        }

        enemyBase.isAttack = false;
    }
}
