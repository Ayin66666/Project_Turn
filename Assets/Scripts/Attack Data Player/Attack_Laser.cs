using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Laser : Attack_Base
{
    // 5ȸ Ÿ��?
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

                // ���⼱ �̰� ���� Ƚ���� �־���!
                Player_Manager.instnace.player_Turn.curAttackCount = refCount;

                // ���� ��� ���� ȣ��
                StartCoroutine(Attack());
                break;


            case AttackOwner.Enemy:
                if (owner == AttackOwner.Enemy)
                {
                    Debug.Log("���ʹ�");
                    ownerObj.GetComponent<Enemy_Base>().isAttack = false;
                }
                break;
        }
    }

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
        //Player_Manager.instnace.player_Turn.turnAnim.LaserOff();

        // �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isLaser", true);

        // ī�޶� ����
        Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.LaserA, 0.5f);

        // �ִϸ��̼� ���
        while(anim.GetBool("isLaser"))
        {
            yield return null;
        }

        // ī�޶� ����
        Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.None, 0.75f);

        yield return new WaitForSeconds(0.25f);
        
        Player_Manager.instnace.isAttack = false;
    }
}
