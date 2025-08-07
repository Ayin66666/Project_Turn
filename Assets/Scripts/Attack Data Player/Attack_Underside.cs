using Easing.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Attack_Underside : Attack_Base
{

    [Header("=== Underside Attack ===")]
    [SerializeField] private GameObject undersideSet;
    [SerializeField] private VideoPlayer undersideVideo;
    [SerializeField] private Transform undersidePos;
    [SerializeField] private SkinnedMeshRenderer[] bodys;
    [SerializeField] private GameObject undersideSlashVFX;
    [SerializeField] private GameObject undersideLastVFX;


    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, List<GameObject> targetObj, bool isExchange, int attackCount)
    {
        owner = attackOwner;

        // �ִϸ����� �Ҵ�
        anim = Player_Manager.instnace.player_Turn.anim;
        Player_Manager.instnace.isAttack = true;


        // ���� ��� ���� ȣ��
        StartCoroutine(Attack());
    }

    // ��� ����
    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, GameObject targetObj, bool isExchange, int attackCount)
    {
        throw new System.NotImplementedException();
    }


    private IEnumerator Attack()
    {
        // ������ �ڵ�
        // Player_Manager.instnace.player_Turn.ResetAll();

        Player_Manager.instnace.isAttack = true;

        // ����
        undersideSet.SetActive(true);
        undersideVideo.Play();
        yield return new WaitForSeconds(0.2f);
        while (undersideVideo.isPlaying)
        {
            yield return null;
        }
        undersideSet.SetActive(false);

        // ��ų ��Ȱ��ȭ
        Player_UI.instance.Turn_UndersideSkillOn(false);

        // ī�޶� ����
        Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.UndersideA, 0.3f);

        // �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isUndersideAttack", true);
        anim.SetFloat("AnimProgress", 0);

        // �̵�
        Vector3 startPos = Player_Manager.instnace.player_Turn.gameObject.transform.position;
        Vector3 endPos = undersidePos.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 2f;
            anim.SetFloat("AnimProgress", timer);
            Player_Manager.instnace.player_Turn.gameObject.transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        transform.position = endPos;
        anim.SetFloat("AnimProgress", 1);

        // ī�޶� ����
        Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.UndersideB, 0.75f);

        // �÷��̾� ��Ȱ��ȭ
        for (int i = 0; i < bodys.Length; i++)
        {
            bodys[i].enabled = false;
        }

        // ������ ��� -> ��Ÿ 10ȸ -> ���� �ʿ� -> �ƴϸ� ���̿�ƼƼ�� �״��?
        StartCoroutine(Attack_Damage());

        // ���� ����Ʈ
        undersideSlashVFX.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        // �÷��̾� Ȱ��ȭ
        for (int i = 0; i < bodys.Length; i++)
        {
            bodys[i].enabled = true;
        }

        // ī�޶� ����
        Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.UndersideC, 0.3f);


        // ������ ����
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isUndersideAttack", true);
        anim.SetFloat("AnimProgress", 0);

        // �ִϸ��̼� ���
        while (anim.GetBool("isUndersideAttack"))
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);


        // ī�޶� ����
        Camera_Manager.instance.BrainCam_Setting(0.75f);
        Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.None, 0.75f);

        // ���� ����
        Player_Manager.instnace.isAttack = false;
    }

    private IEnumerator Attack_Damage()
    {
        for (int i = 0;i < 8; i++)
        {
            Player_Manager.instnace.player_Turn.Attack_UndersideCall();

            // ������
            yield return new WaitForSeconds(0.15f);
        }
    }
}
