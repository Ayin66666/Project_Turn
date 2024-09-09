using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class Player_Turn : MonoBehaviour
{
    [Header("=== State ===")]
    public bool isSpawnAnim;
    public bool isSelect;
    public bool isEngageMove;
    public bool isRecoilMove;


    [Header("=== Attack Setting ===")]
    [SerializeField] private List<Attack_Base> attackSlot;
    [SerializeField] private int attackCount;


    [Header("=== Component ===")]
    [SerializeField] private Animator anim;
    private Coroutine curCoroutine;


    // 맨 처음 전투 시작할 때 호출 -> 등장 모션 같은거
    public void Turn_StartAnim()
    {
        curCoroutine = StartCoroutine(StartAnimCall());
    }

    private IEnumerator StartAnimCall()
    {
        isSpawnAnim = true;

        if(anim != null)
        {
            // Animation
            anim.SetTrigger("Spawn");
            anim.SetBool("isSpawn", true);

            // Camera Movement


            // Animation Wait
            while (anim.GetBool("isSpawn"))
            {
                yield return null;
            }
        }

        isSpawnAnim = false;
    }

    // 공격 선택
    public void Turn_Select()
    {
        isSelect = true;
        Player_UI.instance.TurnFight_Select(false);

    }

    // 공격 선택 체크
    public void Select_Check()
    {
        for (int i = 0; i < attackCount; i++)
        {

        }

        isSelect = false;
    }

    // 합 이동
    public void Turn_EngageMove()
    {

    }

    // 맨 마지막 전투 종료 할 때 호출 -> 승리/사망 모션 같은거
    public void Turn_End()
    {

    }
}
