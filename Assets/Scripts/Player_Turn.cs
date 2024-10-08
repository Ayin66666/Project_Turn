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
    public Transform exchangePos;


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
            anim.SetTrigger("Action");
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

        // Player_UI.instance();
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
        for(int i = 0;i < attackSlot.Count; i++)
        {
           if(!attackSlot[i].haveAttack)
            {
                attackSlot[i].Attack_Setting(attack);
                Slot_TargetSetting(attackSlot[i]);
                break;
            }
        }
    }


    // �ش� �������� �� ������ ����
    public void Slot_TargetSetting(Attack_Slot slot)
    {
        StartCoroutine(Slot_TargetSettingCall(slot));
    }


    private IEnumerator Slot_TargetSettingCall(Attack_Slot slot)
    {
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

        // Ÿ�� ���� -> ����ü ����
        Player_Manager.instnace.turnManger.Exchange_Setting_Add(slot, enemyAttackList[enemyIndex]);
        slot.targetSlot = enemyAttackList[enemyIndex];

        // Ÿ�� ���� UI Off
        Player_UI.instance.Turn_TargetSelect(false);
        Player_UI.instance.Turn_EngageUI(Player_UI.Object.None, slot, false);
        slot.Attack_LineSetting(false, null);
        enemyAttackList[enemyIndex].Highlights_Effect(false);
        isExchangeTargetSelect = false;

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

        // �ִϸ��̼�
        if(anim != null)
        {
            anim.SetTrigger("Action");
            anim.SetBool("isExchangeMove", true);
        }

        // �̵� �ڵ�
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


    // �� �и� ���� �ٽ� �� ��ġ�� �̵� ȣ��
    public void Turn_ExchangeMove_After(Vector3 pos)
    {
        StartCoroutine(Turn_ExchangeMoveAfterCall(pos));
    }


    // �� �и� ���� �ٽ� �� ��ġ�� �̵� ����
    private IEnumerator Turn_ExchangeMoveAfterCall(Vector3 pos)
    {
        isExchangeMove = true;

        Vector3 startPos = transform.position;
        Vector3 endPos = pos;

        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 4f;
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
            anim.SetBool("isExchangeAnim", true);
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

        // �� �� �и� �̵� ����
        StartCoroutine(Turn_ExchanageResuitMove(type));

        // �� �ִϸ��̼�
        if(anim != null)
        {
            anim.SetBool("isExchange", false);
            switch (type)
            {
                case ExchangeResuit.Win:
                    anim.SetBool("isExchangeWin", true);
                    while (anim.GetBool("isExchangeWin"))
                    {
                        yield return null;
                    }
                    break;

                case ExchangeResuit.Draw:
                    anim.SetBool("isExchangeDraw", true);
                    while (anim.GetBool("isExchangeDraw"))
                    {
                        yield return null;
                    }
                    break;

                case ExchangeResuit.Lose:
                    anim.SetBool("isExchangeLose", true);
                    while (anim.GetBool("isExchangeLose"))
                    {
                        yield return null;
                    }
                    break;
            }
        }

        // Delay
        yield return new WaitForSeconds(Random.Range(0.15f, 0.25f));

        isRecoilMove = false;
    }


    // �� �и� �̵� ����
    private IEnumerator Turn_ExchanageResuitMove(ExchangeResuit type)
    {
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
    }


    // ���� ��� ȣ�� & ����
    public void Turn_Attack(GameObject target, Attack_Base attack, int attackCount)
    {
        attack.UseAttack(Attack_Base.AttackOwner.Player, gameObject, target, attackCount);
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
