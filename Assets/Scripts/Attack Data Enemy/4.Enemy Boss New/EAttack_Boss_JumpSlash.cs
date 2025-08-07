using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing.Tweening;


public class EAttack_Boss_JumpSlash : Attack_Base
{
    [Header("=== Attack Setting ===")]
    [SerializeField] private Enemy_Boss boss;
    private Enemy_Base enemyBase;
    private GameObject ownerObj;
    private int refCount;


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
        StartCoroutine(Attack());
    }
    /// <summary>
    /// ���� �뽬 �� �˱� �߻�
    /// </summary>
    /// <returns></returns>
    private IEnumerator Attack()
    {
        enemyBase.isAttack = true;

        // 1Ÿ ����

        // ���̵彺��
        anim.SetTrigger("Action");
        anim.SetBool("isBackstep", true);
        Vector3 startPos = ownerObj.transform.position;
        Vector3 endPos = boss.jumpSlashPos[Random.Range(0, 2)].position;
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * 2f;
            ownerObj.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        anim.SetBool("isBackstep", false);

        // �˱� ����
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isJumpSlash", true);
        while (anim.GetBool(refCount == 2 ? "isAttack" : "isJumpSlash"))
        {
            yield return null;
        }

        // 2Ÿ ����
        if (refCount == 2)
        {
            // �ڷ���Ʈ
            Instantiate(boss.teleportVFX, ownerObj.transform.position, Quaternion.identity);
            ownerObj.transform.position = Player_Manager.instnace.player_Turn.exchangePos.position;
            boss.LookAt(target);

            // ���� ����
            anim.SetTrigger("Action");
            anim.SetBool("isAttack", true);
            anim.SetBool("isJumpSlash", true);
            while (anim.GetBool("isJumpSlash"))
            {
                yield return null;
            }
        }

        enemyBase.isAttack = false;
    }
}
