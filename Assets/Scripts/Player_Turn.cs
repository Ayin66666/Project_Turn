using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing.Tweening;


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
    public bool isExchangeMove;
    public bool isRecoilMove;
    private bool isExchangeTargetSelect;

    public enum ExchangeResuit { Win, Draw, Lose }


    [Header("=== Attack Setting ===")]
    public List<Attack_Slot> attackSlot;
    [SerializeField] private List<Attack_Slot> enemyAttackList; // ��� ���� �޾ƿ���

    [Header("=== Pos Setting ===")]
    [SerializeField] private Transform[] recoilPos;


    [Header("=== Component ===")]
    [SerializeField] private Animator anim;
    private Coroutine curCoroutine;


    // �� ó�� ���� ������ �� ȣ�� -> ���� ��� ������
    public void Turn_StartAnim()
    {
        curCoroutine = StartCoroutine(Turn_StartAnimCall());
    }


    private IEnumerator Turn_StartAnimCall()
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


    // ������ �⺻���� ��������, ���� �߶��� ���� ������ ��ɵ� �ʿ���!
    public void Slot_SpeedSetting()
    {
        for (int i = 0; i < attackSlot.Count; i++)
        {
            int ran = Random.Range(Player_Manager.instnace.SlotSpeed.x, Player_Manager.instnace.SlotSpeed.y);
            attackSlot[i].Speed_Setting(ran);
        }
    }


    // ���� ���� �� ����
    public void Turn_AttackSelect()
    {
        if(isExchangeTargetSelect)
        {
            return;
        }

        isSelect = true;
        
        // ���� ���� -> �����ִ� ���� ���� ��� ���
        for (int i = 0; i < attackSlot.Count; i++)
        {
            attackSlot[i].ResetSlot();
        }

        // ���� UI On
        Player_UI.instance.Turn_FightSelect(true);
    }


    // ���Կ� ���� ����
    public void Slot_AttackSetting(Attack_Base attack)
    {
        Debug.Log("Call slotSetting");
        for(int i = 0;i < attackSlot.Count; i++)
        {
           if(!attackSlot[i].haveAttack)
            {
                attackSlot[i].Attack_Setting_Player(attack);
                Slot_TargetSetting(attackSlot[i]);
                break;
            }
        }

        Debug.Log("full skill");
    }


    // �ش� �������� �� ������ ����
    public void Slot_TargetSetting(Attack_Slot slot)
    {
        StartCoroutine(Slot_TargetSettingCall(slot));
    }


    private IEnumerator Slot_TargetSettingCall(Attack_Slot slot)
    {
        Debug.Log("Call Slot Target Setting");
        isExchangeTargetSelect = true;

        // ����Ʈ ���� -> ���� ���� ��� null �� ���� ������
        enemyAttackList?.Clear(); // -> �̰� null ���� ����̶�µ� �Ƹ� ����ó�� ���̴� �뵵�ε�?

        // ���� ���� ���� ��������
        enemyAttackList = Player_Manager.instnace.turnManger.GetEnemyAttackSlot();

        // �÷��̾ ������ �� ���� ���
        // -> �� �κ� bool ������ ���� �ٲٰ� �÷��̾ UI�� �̸����� �����ϴ� ��� �־�� �ҵ�...
        while(isExchangeTargetSelect)
        {
            // -> ���� �۾����ε� ��� ���۽��Ѿ��ұ�?
            // ��� �������°� ���Ը��� ���� ��¡ ui ���� + A�� �� �Ϲ���� �����ִ°ǵ�...
            // ���� ǥ�� ������ �� ������ ����.
            yield return null;
        }

        // ��� ���� �� ���� �������� ���� ������ �ִ��� üũ
        for (int i = 0; i < attackSlot.Count; i++)
        {
            // ���� ������ ���� �������� �ʾ��� ���
            if(!attackSlot[i].haveAttack)
            {
                break;
            }
        }

        // ��� ���Կ� ������ ������ ���
        Player_UI.instance.Turn_AttackButton(true);
    }


    // �� �̵�
    public void Turn_ExchangeMove(Transform movePos)
    {
        curCoroutine = StartCoroutine(Turn_EngageMoveCall(movePos));
    }


    private IEnumerator Turn_EngageMoveCall(Transform movePos)
    {
        isExchangeMove = true;

        Vector3 startPos = transform.position;
        Vector3 endPos = movePos.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.5f;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        isExchangeMove = false;
    }

    // �� ���� �ִϸ��̼�
    public void Turn_ExchangeStartAnim()
    {
        anim.SetTrigger("Exchange");
        anim.SetBool("isExchange", true);
    }

    // �� ��� �ִϸ��̼� ȣ��
    public void Turn_ExchangeResuitAnim(ExchangeResuit type)
    {
        curCoroutine = StartCoroutine(Turn_ExchangeResuitAnimCall(type));
    }


    // �� ��� �ִϸ��̼� ����
    private IEnumerator Turn_ExchangeResuitAnimCall(ExchangeResuit type)
    {
        isRecoilMove = true;


        // �ִϸ��̼� ��� ����
        anim.SetTrigger("Exchange");
        anim.SetBool("isExchange", false);


        // �� �� ������
        yield return new WaitForSeconds(Random.Range(0.25f, 0.5f));


        // �� �� �и� �̵� ����
        StartCoroutine(Turn_ExchanageResuitMove(type));


        // �� �ִϸ��̼�
        switch (type)
        {
            case ExchangeResuit.Win:
                anim.SetBool("EngageWin", true);
                while (anim.GetBool("EngageWin"))
                {
                    yield return null;
                }
                break;

            case ExchangeResuit.Draw:
                anim.SetBool("EngageDraw", true);
                while (anim.GetBool("EngageDraw"))
                {
                    yield return null;
                }
                break;

            case ExchangeResuit.Lose:
                anim.SetBool("EngageLose", true);
                while (anim.GetBool("EngageLose"))
                {
                    yield return null;
                }
                break;
        }


        // �и� ���� ���
        yield return new WaitForSeconds(Random.Range(0.25f, 0.55f));


        isRecoilMove = false;
    }


    // �� �и� �̵� ����
    private IEnumerator Turn_ExchanageResuitMove(ExchangeResuit type)
    {
        isRecoilMove = true;

        // Recoil Move
        Vector3 startPos = transform.position;
        Vector3 endPos = recoilPos[type == ExchangeResuit.Win ? 0 : (type == ExchangeResuit.Draw ? 1 : 2)].position;
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
