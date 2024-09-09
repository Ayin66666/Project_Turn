using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player_Turn : MonoBehaviour
{
    [Header("=== State ===")]
    public bool isSpawnAnim;
    public bool isSelect;
    public bool isEngageMove;
    public bool isRecoilMove;

    [Header("=== Attack Setting ===")]
    [SerializeField] private List<Attack_Slot> attackSlot;


    [Header("=== Component ===")]
    [SerializeField] private Animator anim;
    private Coroutine curCoroutine;


    // �� ó�� ���� ������ �� ȣ�� -> ���� ��� ������
    public void Turn_Start()
    {
        curCoroutine = StartCoroutine(Turn_StartCall());
    }

    private IEnumerator Turn_StartCall()
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
    }

    // ���� ���� üũ
    public void Select_Check()
    {
        for (int i = 0; i < attackSlot.Count; i++)
        {
            if (!attackSlot[i].haveAttack)
            {
                return;
            }
        }

        isSelect = false;
    }

    // �� ������ ���� ���� �� �� ȣ�� -> �¸�/��� ��� ������
    public void Turn_End()
    {

    }
}
