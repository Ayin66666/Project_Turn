using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Strike : Attack_Base
{
    [Header("=== Attack Setting ===")]
    [SerializeField] private int refCount;

    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, GameObject targetObj, bool isExchange, int attackCount)
    {
        switch (attackOwner)
        {
            case AttackOwner.Player:
                owner = attackOwner;

                // �ִϸ����� �Ҵ�
                anim = Player_Manager.instnace.player_Turn.anim;
                Player_Manager.instnace.isAttack = true;

                // Ÿ�� ����
                target = targetObj;

                // ���� ���� Ƚ�� ����
                refCount = attackCount;

                // ���� ��� ���� ȣ��
                StartCoroutine(Attack());
                break;


            case AttackOwner.Enemy:
                break;
        }
    }

    // ��� ����
    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, List<GameObject> targetObj, bool isExchange, int attackCount)
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator Attack()
    {
        // ������ �ڵ�
        Player_Manager.instnace.player_Turn.ResetAll();

        // Ÿ�� �ٶ󺸱�
        Player_Manager.instnace.player_Turn.LookAt(target);

        // �̵� ȣ��
        Player_Manager.instnace.player_Turn.Turn_AttackMove();
        while (Player_Manager.instnace.player_Turn.isExchangeMove)
        {
            yield return null;
        }

        // Ÿ���� ��� üũ
        if (target.GetComponent<Enemy_Base>().isDie)
        {
            anim.SetTrigger("Action");
            anim.SetBool("isAttack", false);
            anim.SetBool("isStrikeAttack", false);
            while (anim.GetBool("isStrikeAttack"))
            {
                yield return null;
            }

            Player_Manager.instnace.isAttack = false;
            yield break;
        }
        else
        {
            anim.SetTrigger("Action");
            anim.SetBool("isAttack", true);
            anim.SetBool("isStrikeAttack", true);

            // �ִϸ��̼� ���
            while (anim.GetBool("isAttack"))
            {
                yield return null;
            }
        }

        // ���� ���� -> while �� ��
        Player_Manager.instnace.isAttack = false;
        Debug.Log("�÷��̾� ��Ʈ����ũ ���� ����");
    }
}
