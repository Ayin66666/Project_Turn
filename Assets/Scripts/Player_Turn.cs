using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing.Tweening;
using UnityEngine.UIElements;


public class Player_Turn : MonoBehaviour
{
    /*
    üũ�ؾ��ϴ� ����

    0. ������ ��� �����ǰ�?
	    = �÷��̾� ���� üũ

    1. �÷��̾� ��ü ���� Ƚ�� vs �� ��ü ���� Ƚ��
	    = ������� ��� �Ϲ������ �ϴ°�?

    2. �÷��̾� ���� Ƚ�� vs �� ���� Ƚ��
	    = �ش� ������ ��� ���� �ؾ��ϴ°�?
	    = �� ���� �� ��� ���� �� �ִ°�?

    3. �Ϲ� ����
	    = �÷��̾�&���Ͱ� ������ ��� �Ϲ���� �ϴ°�?
    */
    [Header("=== State ===")]
    public bool isSpawnAnim;
    public bool isSelect;
    public bool isEngageMove;
    public bool isRecoilMove;

    public enum RecoilType { Win, Draw, Lose }

    [Header("=== Attack Setting ===")]
    public List<Attack_Slot> attackSlot;


    [Header("=== Pos Setting ===")]
    [SerializeField] private Transform[] recoilPos;


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
        
        // ���� ���� -> �����ִ� ���� ���� ��� ���
        for (int i = 0; i < attackSlot.Count; i++)
        {
            attackSlot[i].ResetSlot();
        }

        // ���� UI On
        Player_UI.instance.TurnFight_Select(true);
    }

    // �� �̵�
    public void Turn_EngageMove(Transform movePos)
    {
        curCoroutine = StartCoroutine(Turn_EngageMoveCall(movePos));
    }

    private IEnumerator Turn_EngageMoveCall(Transform movePos)
    {
        isEngageMove = true;

        Vector3 startPos = transform.position;
        Vector3 endPos = movePos.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.5f;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        isEngageMove = false;
    }

    // �� �ִϸ��̼�
    public void EngageResuit(RecoilType type)
    {
        curCoroutine = StartCoroutine(EngageAnimCall(type));
    }

    // �� ���� �� �¸�, ���º�, �й� �ִϸ��̼�
    private IEnumerator EngageAnimCall(RecoilType type)
    {
        isRecoilMove = true;

        anim.SetTrigger("Attack");
        anim.SetBool("EngageAnim", true);

        // Delay
        yield return new WaitForSeconds(Random.Range(0.5f, 1f));

        // Recoil Move
        StartCoroutine(RecoilMove(type));

        // Win & Lose Animation
        switch (type)
        {
            case RecoilType.Win:
                anim.SetBool("EngageWin", true);
                while (anim.GetBool("EngageWin"))
                {
                    yield return null;
                }
                break;

            case RecoilType.Draw:
                anim.SetBool("EngageDraw", true);
                while (anim.GetBool("EngageDraw"))
                {
                    yield return null;
                }
                break;

            case RecoilType.Lose:
                anim.SetBool("EngageLose", true);
                while (anim.GetBool("EngageLose"))
                {
                    yield return null;
                }
                break;
        }

        // Delay
        yield return new WaitForSeconds(Random.Range(0.25f, 0.55f));

        isRecoilMove = false;
    }

    // �� �̵� ���
    private IEnumerator RecoilMove(RecoilType type)
    {
        isRecoilMove = true;

        // Recoil Move
        Vector3 startPos = transform.position;
        Vector3 endPos = recoilPos[type == RecoilType.Win ? 0 : (type == RecoilType.Draw ? 1 : 2)].position;
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * 1.5f;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        // Delay
        yield return new WaitForSeconds(Random.Range(0.15f, 0.25f));

        isRecoilMove = false;
    }

    // �� ������ ���� ���� �� �� ȣ�� -> �¸�/��� ��� ������
    public void Turn_End()
    {
        StartCoroutine(Turn_EndCall());
    }

    // �ִϸ��̼� -> ���� �ִϸ��̼� ���� ��� �ȸ���!
    private IEnumerator Turn_EndCall()
    {
        anim.SetTrigger("TurnEnd");
        anim.SetBool("isTurnEndAnim", true);
        while(anim.GetBool("isTurnEndAnim"))
        {
            yield return null;
        }
    }
}
