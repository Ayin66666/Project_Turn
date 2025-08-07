using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Combo : Attack_Base
{
    [Header("=== Attack Setting ===")]
    [SerializeField] private int refCount;
    private Enemy_Base enemy;


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
                enemy = target.GetComponent<Enemy_Base>();

                // 남은 공격 횟수 셋팅
                refCount = attackCount;

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

    // 사용 안함
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

        // 이동 호출
        Player_Manager.instnace.player_Turn.Turn_AttackMove();
        while (Player_Manager.instnace.player_Turn.isExchangeMove)
        {
            yield return null;
        }

        // 공격 호출
        int a = 0;
        while (a < refCount)
        {
            // 타겟의 사망 체크
            if (enemy.isDie)
            {
                anim.SetTrigger("Action");
                anim.SetBool("isAttack", false);
                while (anim.GetBool("isComboAttack"))
                {
                    yield return null;
                }

                Player_Manager.instnace.isAttack = false;
                yield break;
            }
            else
            {
                // 애니메이션 호출
                anim.SetTrigger("Action");
                anim.SetBool("isAttack", true);
                anim.SetBool("isComboAttack", true);

                // 애니메이션 대기
                while (anim.GetBool("isAttack"))
                {
                    yield return null;
                }

                // 공격 횟수 상승
                a++;
            }
        }

        // 공격 종료 -> while 문 밖
        Player_Manager.instnace.isAttack = false;
    }
}
