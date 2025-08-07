using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Guard : Attack_Base
{
    [Header("=== Attack Setting ===")]
    [SerializeField] private int refCount;

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

                // ���� ���� Ƚ�� ����
                refCount = attackCount;

                // ���⼱ �̰� ���� Ƚ���� �־���!
                Player_Manager.instnace.player_Turn.curAttackCount = refCount;

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
        Player_Manager.instnace.isAttack = true;

        // ������ �ڵ�
        Player_Manager.instnace.player_Turn.ResetAll();

        // ȸ�� ����Ʈ
        Player_Manager.instnace.player_Turn.turnAnim.guardEffect.SetActive(true);

        // ����
        Player_Manager.instnace.Turn_Sound_Call(5);

        // ȸ�� ���
        int ran = Random.Range(15, 30);
        int b = (Player_Manager.instnace.curHp + ran) - Player_Manager.instnace.maxHp;
        if (b > 0)
        {
            ran -= b;
        }
        Player_Manager.instnace.curHp += ran;

        // ����Ʈ ���
        while (Player_Manager.instnace.player_Turn.turnAnim.guardEffect.activeSelf)
        {
            yield return null;
        }

        Player_Manager.instnace.isAttack = false;
    }
}
