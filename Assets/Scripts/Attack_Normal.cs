using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Normal : Attack_Base
{
    [Header("=== Attack Setting ===")]
    [SerializeField] private Animator anim;
    private Enemy_Base enemy;
    private GameObject target;

    public override void UseAttack(AttackOwner attackOwner, GameObject ownerObj, GameObject targetObj)
    {
        // 애니메이터 할당
        switch (attackOwner)
        {
            case AttackOwner.Player:
                anim = Player_Manager.instnace.player_Turn.anim;
                Player_Manager.instnace.isAttack = true;
                break;

            case AttackOwner.Enemy:
                enemy = ownerObj.GetComponent<Enemy_Base>();
                anim = enemy.anim;
                break;
        }

        // 타겟 셋팅
        target = targetObj;

        // 공격 기능 동작 호출
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {


        yield return null;
    }
}
