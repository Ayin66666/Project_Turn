using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Normal : Attack_Base
{
    [Header("=== Attack Setting ===")]
    [SerializeField] private Animator anim = null;
    [SerializeField] private Enemy_Base enemy = null;
    private GameObject target;
    private int refCount;

    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, GameObject targetObj, int attackCount)
    {
        // �ִϸ����� �Ҵ�
        switch (attackOwner)
        {
            case AttackOwner.Player:
                anim = Player_Manager.instnace.player_Turn.anim;
                Player_Manager.instnace.isAttack = true;
                break;

            case AttackOwner.Enemy:
                enemy = ownerObj.GetComponent<Enemy_Base>();
                anim = enemy.anim;
                break;
        }

        // Ÿ�� ����
        target = targetObj;


        // ���� ���� Ƚ�� ����
        refCount = attackCount;

        // ���� ��� ���� ȣ��
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        Debug.Log("���� ȣ��!!!!");

        Player_Manager.instnace.isAttack = true;

        // ���� ���� Ƚ�� üũ �� ����
        for (int i = 0; i < attackCount - refCount; i++)
        {
            if(anim != null)
            {
                anim.SetTrigger("Attack");
                anim.SetBool("isNormalAttack", true);
                while (anim.GetBool("isNormalAttack"))
                {
                    yield return null;
                }
            }
            Debug.Log("�Ϲ� ���� " + i + " ȸ !");
            yield return new WaitForSeconds(0.1f);
        }

        Player_Manager.instnace.isAttack = false;
    }
}
