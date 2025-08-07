using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAttack_Boss_QudraSlash : Attack_Base
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
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        enemyBase.isAttack = true;

        // �� �ٶ󺸱�
        enemyBase.LookAt(target);

        // �̵� ��ġ ����
        Vector3[] movePos = new Vector3[boss.qudraSlashPos.Length];
        for (int i = 0; i < boss.qudraSlashPos.Length; i++)
        {
            movePos[i] = boss.qudraSlashPos[i].position;
        }

        // ���� ����Ʈ
        boss.wingVFX.SetActive(true);

        // ������
        yield return new WaitForSeconds(0.15f);

        // ����
        int a = 0;
        Debug.Log(refCount);
        while(a < refCount)
        {
            // Ÿ���� ��� üũ
            if (Player_Manager.instnace.isDie)
            {
                anim.SetTrigger("Action");
                anim.SetBool("isAttack", false);
                while (anim.GetBool("isQuadraSlash"))
                {
                    yield return null;
                }

                enemyBase.isAttack = false;
                yield break;
            }
            else
            {
                // �ڷ���Ʈ
                ownerObj.transform.position = movePos[a];
                Instantiate(boss.teleportVFX, ownerObj.transform.position, Quaternion.identity);
                boss.LookAt(target);

                // ����
                anim.SetTrigger("Action");
                anim.SetBool("isAttack", true);
                anim.SetBool("isQuadraSlash", true);
                while (anim.GetBool("isAttack"))
                {
                    yield return null;
                }
            }
            a++;
        }

        // ���� ����Ʈ
        boss.wingVFX.SetActive(false);

        // �ִϸ��̼� ���
        anim.SetTrigger("Action");
        while (anim.GetBool("isQuadraSlash"))
        {
            yield return null;
        }

        enemyBase.isAttack = false;
    }
}

