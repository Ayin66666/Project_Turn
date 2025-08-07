using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAttack_NormalShot : Attack_Base
{
    [Header("=== Attack Setting ===")]
    [SerializeField] private Enemy_Range enemy;
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

        // ���� ���� Ƚ�� üũ �� ����
        int a = 0;
        while (a < refCount)
        {
            anim.SetTrigger("Action");
            anim.SetBool("isAttack", true);
            anim.SetBool("isNormalShot", true);
            while (anim.GetBool("isAttack"))
            {
                yield return null;
            }

            // Ÿ���� ��� üũ
            if (Player_Manager.instnace.isDie)
            {
                // �ִϸ��̼� ����
                anim.SetTrigger("Action");
                anim.SetBool("isAttack", false);
                anim.SetBool("isNormalShot", false);
                while (anim.GetBool("isNormalShot"))
                {
                    yield return null;
                }

                // ���� ����
                enemyBase.isAttack = false;
                yield break;
            }

            // �ִϸ��̼� ���
            while (anim.GetBool("isAttack"))
            {
                yield return null;
            }

            Debug.Log("�Ϲ� ���� " + a + " ȸ !");
            a++;

            // ���� ���� �Ͻ� ���
            yield return new WaitForSeconds(0.15f);
        }

        // �ִϸ��̼� ����
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", false);
        anim.SetBool("isNormalShot", false);
        while (anim.GetBool("isNormalShot"))
        {
            yield return null;
        }

        // ���� ���� -> while �� ��
        enemyBase.isAttack = false;
    }
}
