using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Explosion : Attack_Base
{
    [Header("=== Attack Setting ===")]
    [SerializeField] private int refCount;
    [SerializeField] private List<GameObject> targetList;
    [SerializeField] private GameObject explosionVFX;

    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, GameObject targetObj, bool isExchange, int attackCount)
    {

    }

    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, List<GameObject> targetObj, bool isExchange, int attackCount)
    {
        switch (attackOwner)
        {
            case AttackOwner.Player:
                owner = attackOwner;

                // �ִϸ����� �Ҵ�
                anim = Player_Manager.instnace.player_Turn.anim;
                Player_Manager.instnace.isAttack = true;

                // Ÿ�� ����
                targetList = targetObj;

                // ���� ���� Ƚ�� ����
                refCount = attackCount;

                // ���� ��� ���� ȣ��
                StartCoroutine(Attack());
                break;


            case AttackOwner.Enemy:
                break;
        }
    }

    private IEnumerator Attack()
    {
        // ������ �ڵ�
        Player_Manager.instnace.player_Turn.ResetAll();

        // Ÿ�� �ٶ󺸱�
        Player_Manager.instnace.player_Turn.LookAt(Player_Manager.instnace.player_Turn.forwardTarget);

        // ī�޶� ����
        Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.ExplosionA, 0.5f);

        // �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isExplosion", true);

        // �ִϸ��̼� ���
        while (anim.GetBool("isExplosion"))
        {
            yield return null;
        }

        // ī�޶� ����
        Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.None, 0.75f);

        Player_Manager.instnace.isAttack = false;
    }
}
