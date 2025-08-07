using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Guard : Attack_Base
{
    [Header("=== Attack Setting ===")]
    [SerializeField] private int refCount;

    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, List<GameObject> targetObj, bool isExchange, int attackCount)
    {
        throw new System.NotImplementedException();
    }

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
                    ownerObj.GetComponent<Enemy_Base>().isAttack = false;
                }
                break;
        }
    }

    private IEnumerator Attack()
    {
        Player_Manager.instnace.isAttack = true;

        // 디버깅용 코드
        Player_Manager.instnace.player_Turn.ResetAll();

        // 회복 이펙트
        Player_Manager.instnace.player_Turn.turnAnim.guardEffect.SetActive(true);

        // 사운드
        Player_Manager.instnace.Turn_Sound_Call(5);

        // 회복 기능
        int ran = Random.Range(15, 30);
        int b = (Player_Manager.instnace.curHp + ran) - Player_Manager.instnace.maxHp;
        if (b > 0)
        {
            ran -= b;
        }
        Player_Manager.instnace.curHp += ran;

        // 이펙트 대기
        while (Player_Manager.instnace.player_Turn.turnAnim.guardEffect.activeSelf)
        {
            yield return null;
        }

        Player_Manager.instnace.isAttack = false;
    }
}
