using Easing.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    public bool isExchangeTargetSelect;
    public bool isEndAnim;

    public enum ExchangeResuit { Win, Draw, Lose }
    public enum EndType { Win, Lose }

    [Header("=== Attack Setting ===")]
    public List<Attack_Slot> attackSlot;


    [Header("=== Pos Setting ===")]
    [SerializeField] private Transform[] exchange_RecoilPos;


    [Header("=== Component ===")]
    public Animator anim = null;
    private Coroutine curCoroutine;


    [Header("=== Target List ===")]
    [SerializeField] private List<Attack_Slot> enemyAttackList;
    [SerializeField] private int enemyIndex;


    // �� ���� ���� �ִϸ��̼� ȣ��
    public void Turn_StartAnim()
    {
        curCoroutine = StartCoroutine(Turn_StartAnimCall());
    }


    // �� ���� ���� �ִϸ��̼� ����
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


    // ���� ���� �ӵ� ����
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
       

        // ���� UI On
        Player_UI.instance.Turn_FightSelect(true);

        // ���� ���� -> �����ִ� ���� ���� ��� ���
        for (int i = 0; i < attackSlot.Count; i++)
        {
            attackSlot[i].ResetSlot();
        }
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
        Debug.Log("Call Slot TargetSetting");
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
        
        // Ÿ�� ���� UI On
        enemyIndex = 0;
        Player_UI.instance.Turn_TargetSelect_DataSetting(true, enemyAttackList[enemyIndex].slotOwner.GetComponent<Enemy_Base>());
        Player_UI.instance.Turn_TargetSelect(true);
        slot.Attack_LineSetting(true, enemyAttackList[enemyIndex].gameObject);

        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Player, slot, true);
        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Enemy, enemyAttackList[enemyIndex], true);

        // ���� �߱� ȿ��
        enemyAttackList[enemyIndex].Highlights_Effect(true);

        // Ÿ�� ���ñ��� ���
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            if(enemyAttackList.Count > 1)
            {
                // �·� �̵�
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    enemyAttackList[enemyIndex].Highlights_Effect(false);

                    enemyIndex = (enemyIndex - 1 + enemyAttackList.Count) % enemyAttackList.Count;
                    Player_UI.instance.Turn_TargetSelect_DataSetting(true, enemyAttackList[enemyIndex].slotOwner.GetComponent<Enemy_Base>());
                    Player_UI.instance.Turn_EngageUI(Player_UI.Object.Player, slot, true);
                    Player_UI.instance.Turn_EngageUI(Player_UI.Object.Enemy, enemyAttackList[enemyIndex], true);

                    slot.Attack_LineSetting(true, enemyAttackList[enemyIndex].gameObject);
                    enemyAttackList[enemyIndex].Highlights_Effect(true);
                }

                // ��� �̵�
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    enemyAttackList[enemyIndex].Highlights_Effect(false);

                    enemyIndex = (enemyIndex + 1) % enemyAttackList.Count;
                    Player_UI.instance.Turn_TargetSelect_DataSetting(true, enemyAttackList[enemyIndex].slotOwner.GetComponent<Enemy_Base>());
                    Player_UI.instance.Turn_EngageUI(Player_UI.Object.Player, slot, true);
                    Player_UI.instance.Turn_EngageUI(Player_UI.Object.Enemy, enemyAttackList[enemyIndex], true);

                    slot.Attack_LineSetting(true, enemyAttackList[enemyIndex].gameObject);
                    enemyAttackList[enemyIndex].Highlights_Effect(true);
                }
            }

            yield return null;
        }

        // Ÿ�� ����
        slot.Attack_TargetSetting(enemyAttackList[enemyIndex]);

        // Ÿ�� ���� UI Off
        Player_UI.instance.Turn_TargetSelect(false);
        Player_UI.instance.Turn_EngageUI(Player_UI.Object.None, slot, false);
        slot.Attack_LineSetting(false, null);
        enemyAttackList[enemyIndex].Highlights_Effect(false);

        // ���� ���� üũ
        Turn_FightButtonCheck();
    }


    // ���� ���� ��ư Ȱ��ȭ üũ
    private void Turn_FightButtonCheck()
    {
        // ��� ���� �� ���� �������� ���� ������ �ִ��� üũ
        for (int i = 0; i < attackSlot.Count; i++)
        {
            // ���� ������ ���� �������� �ʾ��� ���
            if (!attackSlot[i].haveAttack)
            {
                break;
            }
        }

        // ��� ���Կ� ������ ������ ���
        Player_UI.instance.Turn_AttackButton(true);
    }


    // �� �̵� ȣ��
    public void Turn_ExchangeMove(Transform movePos)
    {
        curCoroutine = StartCoroutine(Turn_EngageMoveCall(movePos));
    }


    // �� �̵� ����
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
        if(anim != null)
        {
            anim.SetTrigger("Exchange");
            anim.SetBool("isExchange", true);
        }
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
        if(anim != null)
        {
            anim.SetTrigger("Exchange");
            anim.SetBool("isExchange", false);
        }

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
        Vector3 endPos = exchange_RecoilPos[type == ExchangeResuit.Win ? 0 : (type == ExchangeResuit.Draw ? 1 : 2)].position;
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


    // ���� ��� ȣ�� & ����
    public void Turn_Attack(GameObject target, Attack_Base attack)
    {
        attack.UseAttack(Attack_Base.AttackOwner.Player, gameObject, target);
    }


    // �� ������ ���� ���� �� �� ȣ�� -> �¸�/��� ��� ������
    public void Turn_End(EndType type)
    {
        StartCoroutine(Turn_EndCall(type));
    }


    // �ִϸ��̼� -> ���� �ִϸ��̼� ���� ��� �ȸ���!
    private IEnumerator Turn_EndCall(EndType type)
    {
        isEndAnim = true;

        switch (type)
        {
            case EndType.Win:
                anim.SetTrigger("TurnEnd");
                anim.SetBool("isTurnEndWinAnim", true);
                while (anim.GetBool("isTurnEndWinAnim"))
                {
                    yield return null;
                }
                break;

            case EndType.Lose:
                anim.SetTrigger("TurnEnd");
                anim.SetBool("isTurnEndLoseAnim", true);
                while (anim.GetBool("isTurnEndLoseAnim"))
                {
                    yield return null;
                }
                break;
        }

        isEndAnim = false;
    }
}
