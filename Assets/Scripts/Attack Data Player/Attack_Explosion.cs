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

                // 애니메이터 할당
                anim = Player_Manager.instnace.player_Turn.anim;
                Player_Manager.instnace.isAttack = true;

                // 타겟 셋팅
                targetList = targetObj;

                // 남은 공격 횟수 셋팅
                refCount = attackCount;

                // 공격 기능 동작 호출
                StartCoroutine(Attack());
                break;


            case AttackOwner.Enemy:
                break;
        }
    }

    private IEnumerator Attack()
    {
        // 디버깅용 코드
        Player_Manager.instnace.player_Turn.ResetAll();

        // 타겟 바라보기
        Player_Manager.instnace.player_Turn.LookAt(Player_Manager.instnace.player_Turn.forwardTarget);

        // 카메라 연출
        Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.ExplosionA, 0.5f);

        // 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isExplosion", true);

        // 애니메이션 대기
        while (anim.GetBool("isExplosion"))
        {
            yield return null;
        }

        // 카메라 연출
        Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.None, 0.75f);

        Player_Manager.instnace.isAttack = false;
    }
}
