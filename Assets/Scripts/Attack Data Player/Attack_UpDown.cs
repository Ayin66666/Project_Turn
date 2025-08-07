using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_UpDown : Attack_Base
{
    [Header("=== Attack Setting ===")]
    [SerializeField] private int refCount;
    private Enemy_Base enemy;


    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, List<GameObject> targetObj, bool isExchange, int attackCount)
    {
        throw new System.NotImplementedException();
    }

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
                enemy = target.GetComponent<Enemy_Base>();

                // ���� ���� Ƚ�� ����
                refCount = attackCount;

                // ���� ��� ���� ȣ��
                StartCoroutine(Attack());
                break;


            case AttackOwner.Enemy:
                if (owner == AttackOwner.Enemy)
                {
                    ownerObj.GetComponent<Enemy_Base>().isAttack = false;
                }
                break;
        }
    }

    private IEnumerator Attack()
    {
        // Ÿ�� �ٶ󺸱�
        Player_Manager.instnace.player_Turn.LookAt(target);

        // ī�޶� ����
        Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.UpDownA, 0.5f);

        // �̵� ȣ��
        Player_Manager.instnace.player_Turn.Turn_AttackMove();
        while (Player_Manager.instnace.player_Turn.isExchangeMove)
        {
            yield return null;
        }

        // ���� ȣ��
        int a = 0;
        while (a < refCount)
        {
            // Ÿ���� ��� üũ
            if (enemy.isDie)
            {
                anim.SetTrigger("Action");
                anim.SetBool("isAttack", false);
                while (anim.GetBool("isUpDownAttack"))
                {
                    yield return null;
                }

                Player_Manager.instnace.isAttack = false;
                yield break;
            }
            else
            {
                // �ִϸ��̼� ȣ��
                anim.SetTrigger("Action");
                anim.SetBool("isAttack", true);
                anim.SetBool("isUpDownAttack", true);

                // �ִϸ��̼� ���
                while (anim.GetBool("isAttack"))
                {
                    yield return null;
                }

                // ���� Ƚ�� ���
                a++;
            }
        }

        // ī�޶� ����
        Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.None, 0.75f);

        // ���� ���� -> while �� ��
        Player_Manager.instnace.isAttack = false;
    }
}
