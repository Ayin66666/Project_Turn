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

        // 애니메이터 할당
        anim = Player_Manager.instnace.player_Turn.anim;
        Player_Manager.instnace.isAttack = true;


        // 공격 기능 동작 호출
        StartCoroutine(Attack());
    }

    // 사용 안함
    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, GameObject targetObj, bool isExchange, int attackCount)
    {
        throw new System.NotImplementedException();
    }


    private IEnumerator Attack()
    {
        // 디버깅용 코드
        // Player_Manager.instnace.player_Turn.ResetAll();

        Player_Manager.instnace.isAttack = true;

        // 영상
        undersideSet.SetActive(true);
        undersideVideo.Play();
        yield return new WaitForSeconds(0.2f);
        while (undersideVideo.isPlaying)
        {
            yield return null;
        }
        undersideSet.SetActive(false);

        // 스킬 비활성화
        Player_UI.instance.Turn_UndersideSkillOn(false);

        // 카메라 동작
        Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.UndersideA, 0.3f);

        // 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isUndersideAttack", true);
        anim.SetFloat("AnimProgress", 0);

        // 이동
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

        // 카메라 동작
        Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.UndersideB, 0.75f);

        // 플레이어 비활성화
        for (int i = 0; i < bodys.Length; i++)
        {
            bodys[i].enabled = false;
        }

        // 데미지 계산 -> 연타 10회 -> 변경 필요 -> 아니면 아이엔티티로 그대로?
        StartCoroutine(Attack_Damage());

        // 공격 이펙트
        undersideSlashVFX.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        // 플레이어 활성화
        for (int i = 0; i < bodys.Length; i++)
        {
            bodys[i].enabled = true;
        }

        // 카메라 동작
        Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.UndersideC, 0.3f);


        // 마지막 공격
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isUndersideAttack", true);
        anim.SetFloat("AnimProgress", 0);

        // 애니메이션 대기
        while (anim.GetBool("isUndersideAttack"))
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);


        // 카메라 동작
        Camera_Manager.instance.BrainCam_Setting(0.75f);
        Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.None, 0.75f);

        // 공격 종료
        Player_Manager.instnace.isAttack = false;
    }

    private IEnumerator Attack_Damage()
    {
        for (int i = 0;i < 8; i++)
        {
            Player_Manager.instnace.player_Turn.Attack_UndersideCall();

            // 딜레이
            yield return new WaitForSeconds(0.15f);
        }
    }
}
