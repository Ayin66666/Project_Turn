using Easing.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAttack_Boss_SuperSlash : Attack_Base
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

    private IEnumerator Attack()
    {
        boss.isAttack = true;
        int a = 0;

        // �齺��
        anim.SetTrigger("Action");
        anim.SetBool("isBackstep", true);
        boss.LookAt(target);
        Vector3 startPos = ownerObj.transform.position;
        Vector3 endPos = boss.backstepPos.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 2.5f;
            ownerObj.transform.position = Vector3.Lerp(startPos, endPos, timer);
            yield return null;
        }
        anim.SetBool("isBackstep", false);

        // ����
        boss.SoundCall(10);

        // ��¡ �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isSuperSlashCharge", true);
        anim.SetBool("isSuperSlash", true);

        // ������
        boss.chargeVFX.SetActive(true);
        while(anim.GetBool("isSuperSlashCharge"))
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        boss.chargeVFX.SetActive(false);


        // 1�� ���� - �÷�����
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isSuperSlash", true);
        while (anim.GetBool("isAttack"))
        {
            yield return null;
        }
        a++;


        // 2�� ���� - �˱� ����
        if (a < refCount)
        {
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

            for (int i = 0; i < 4; i++)
            {
                // Ÿ���� ��� üũ
                if (Player_Manager.instnace.isDie)
                {
                    anim.SetTrigger("Action");
                    anim.SetBool("isAttack", false);
                    while (anim.GetBool("isSuperSlash"))
                    {
                        yield return null;
                    }

                    enemyBase.isAttack = false;
                    yield break;
                }
                else
                {
                    // �ڷ���Ʈ
                    ownerObj.transform.position = movePos[i];
                    Instantiate(boss.teleportVFX, ownerObj.transform.position, Quaternion.identity);
                    boss.LookAt(target);

                    // ����
                    anim.SetTrigger("Action");
                    anim.SetBool("isAttack", true);
                    anim.SetBool("isSuperSlash", true);
                    while (anim.GetBool("isAttack"))
                    {
                        yield return null;
                    }

                    a++;
                }
            }
            // ���� ����Ʈ
            boss.wingVFX.SetActive(false);
        }

        // 3�� ���� - ��Ʈ����ũ
        if(a < refCount)
        {
            // �� �ٶ󺸱�
            enemyBase.LookAt(target);

            // ���� �ִϸ��̼�
            anim.SetTrigger("Action");
            anim.SetBool("isAttack", true);
            anim.SetBool("isSuperSlashCharge", true);
            anim.SetBool("isSuperSlash", true);
            anim.SetFloat("AnimProgress", 0);

            // ����
            boss.SoundCall(10);

            // �غ� ����Ʈ
            boss.wingVFX.SetActive(true);
            boss.chargeVFX.SetActive(true);

            // ������
            while (anim.GetBool("isSuperSlashCharge"))
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
            boss.chargeVFX.SetActive(false);

            // ���� ���
            anim.SetTrigger("Action");
            anim.SetBool("isAttack", true);
            anim.SetBool("isSuperSlash", true);
            startPos = ownerObj.transform.position;
            endPos = Player_Manager.instnace.player_Turn.exchangePos.position;
            timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime * 1.5f;
                anim.SetFloat("AnimProgress", EasingFunctions.OutExpo(timer));
                ownerObj.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
                yield return null;
            }
            ownerObj.transform.position = endPos;
            anim.SetFloat("AnimProgress", 1);
            while (anim.GetBool("isSuperSlash"))
            {
                yield return null;
            }

            boss.wingVFX.SetActive(false);
            yield return new WaitForSeconds(0.25f);
        }

        // ���� ����
        anim.SetTrigger("Action");
        while (anim.GetBool("isSuperSlash"))
        {
            yield return null;
        }

        // ���� ���� -> while �� ��
        enemyBase.isAttack = false;
    }
}
