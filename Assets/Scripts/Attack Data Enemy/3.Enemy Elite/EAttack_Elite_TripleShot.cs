using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAttack_Elite_TripleShot : Attack_Base
{
    [Header("=== Attack Setting ===")]
    [SerializeField] private Enemy_Elite elite;
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

    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, List<GameObject> targetObj, bool isExchange, int attackCount)
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator Attack()
    {
        // �� �ٶ󺸱�
        enemyBase.LookAt(target);
        anim.SetInteger("Count", 0);

        // ����
        int a = 0;
        while (a < refCount)
        {
            // Ÿ���� ��� üũ
            if (Player_Manager.instnace.isDie)
            {
                // �ִϸ��̼�
                anim.SetTrigger("Action");
                anim.SetBool("isAttack", false);
                while (anim.GetBool("isTripleShot"))
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
                anim.SetBool("isTripleShot", true);
                anim.SetInteger("Count", a);


                // �ִϸ��̼� ȣ��
                while (anim.GetBool("isAttack"))
                {
                    yield return null;
                }

                // ���� Ƚ�� ���
                a++;
            }

            // ���� ������
            yield return new WaitForSeconds(0.15f);
        }

        // ���� ������
        yield return new WaitForSeconds(0.1f);

        // ���� ����
        elite.Line(false);

        anim.SetTrigger("Action");
        while (anim.GetBool("isTripleShot"))
        {
            yield return null;
        }

        // ���� ���� -> while �� ��
        enemyBase.isAttack = false;
    }
}
