using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing.Tweening;

public class EAttack_Flamethrower : Attack_Base
{
    [Header("=== Attack Setting ===")]
    private Enemy_Base enemyBase;
    private GameObject ownerObj;
    private int count; // ȭ�� ���� ���� Ƚ�� -> �̰� �̺�Ʈ���� Ÿ�̸ӷ� ȣ��!
    private int refCount;

    [SerializeField] private GameObject attackVFX;

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

    // ��� ����
    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, List<GameObject> targetObj, bool isExchange, int attackCount)
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator Attack()
    {
        // �� �ٶ󺸱�
        enemyBase.LookAt(target);

        // �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isFlamethrower", true);
        anim.SetFloat("AnimProgress", 0);

        // ������
        StartCoroutine(Damage());

        // �ִϸ��̼� ����
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime;
            anim.SetFloat("AnimProgress", timer);
            yield return null;
        }
        anim.SetFloat("AnimProgress", 1);

        // �ִϸ��̼� ���
        while(anim.GetBool("isFlamethrower"))
        {
            yield return null;
        }

        // ���� ���� -> while �� ��
        enemyBase.isAttack = false;
    }

    IEnumerator Damage()
    {
        // ������ �ο�
        for (int i = 0; i < refCount; i++)
        {
            // ������ ���
            (int pdamage, bool isCritical) = enemyBase.DamageCal(this, i);

            // ������ ����
            Player_Manager.instnace.Take_Damage(pdamage, damageType[i] == DamageType.physical ? Player_Manager.DamageType.Physical : Player_Manager.DamageType.Magical, isCritical);

            yield return new WaitForSeconds(1 / refCount);
        }
    }
}
