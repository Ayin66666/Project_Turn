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


    // �� ó�� ���� ������ �� ȣ�� -> ���� ��� ������
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

    // ���� ����
    public void Turn_Select()
    {
        isSelect = true;
        Player_UI.instance.TurnFight_Select(false);

    }

    // ���� ���� üũ
    public void Select_Check()
    {
        for (int i = 0; i < attackCount; i++)
        {

        }

        isSelect = false;
    }

    // �� �̵�
    public void Turn_EngageMove()
    {

    }

    // �� ������ ���� ���� �� �� ȣ�� -> �¸�/��� ��� ������
    public void Turn_End()
    {

    }
}
