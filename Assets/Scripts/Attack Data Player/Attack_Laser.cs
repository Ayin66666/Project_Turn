using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Laser : Attack_Base
{
    // 5회 타격?
    [Header("=== Attack Setting ===")]
    [SerializeField] private int refCount;


    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, GameObject targetObj, bool isExchange, int attackCount)
    {
        switch (attackOwner)
        {
            case AttackOwner.Player:
                owner = attackOwner;

                // 애니메이터 할당
                anim = Player_Manager.instnace.player_Turn.anim;
                Player_Manager.instnace.isAttack = true;

                // 타겟 셋팅
                target = targetObj;

                // 남은 공격 횟수 셋팅
                refCount = attackCount;

                // 여기선 이걸 남은 횟수로 넣어줌!
                Player_Manager.instnace.player_Turn.curAttackCount = refCount;

                // 공격 기능 동작 호출
                StartCoroutine(Attack());
                break;


            case AttackOwner.Enemy:
                if (owner == AttackOwner.Enemy)
                {
                    Debug.Log("에너미");
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
        // 디버깅용 코드
        Player_Manager.instnace.player_Turn.ResetAll();

        // 타겟 바라보기
        Player_Manager.instnace.player_Turn.LookAt(target);
        //Player_Manager.instnace.player_Turn.turnAnim.LaserOff();

        // 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isAttack", true);
        anim.SetBool("isLaser", true);

        // 카메라 연출
        Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.LaserA, 0.5f);

        // 애니메이션 대기
        while(anim.GetBool("isLaser"))
        {
            yield return null;
        }

        // 카메라 연출
        Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.None, 0.75f);

        yield return new WaitForSeconds(0.25f);
        
        Player_Manager.instnace.isAttack = false;
    }
}
