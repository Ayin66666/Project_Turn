using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnFight_Manager : MonoBehaviour
{
    [Header("=== State ===")]
    [SerializeField] private Turn curTurn;
    [SerializeField] private int nextAttackDelay;

    private bool isExchangeMove;
    private bool isExchange;
    private bool isAttack;


    private enum Turn { Await, Start, Select, Fight, End }
    private enum ExchangeType { OneSide, Exchange }
    private enum Object { Player, Enemy }

    [Header("=== Pos Setting ===")]
    [SerializeField] private Transform playerExchanagePos;
    [SerializeField] private Transform enemyExchanagePos;
    [SerializeField] private Transform[] enemyReturnPos;
    [SerializeField] private Transform playerReturnPos;


    [Header("=== Enemy Setting ===")]
    [SerializeField] private List<Enemy_Base> enemys;
    [SerializeField] private int enemyCount;


    [Header("=== Attack Setting===")]
    [SerializeField] private List<Attack_Slot> playerAttacks;
    [SerializeField] private List<Attack_Slot> enemyAttacks;

    private Attack_Slot playerSlot;
    private Attack_Slot enemySlot;
    private Enemy_Base enemy;


    // 1. ���̵� ���� ȣ��

    // 2. ���� �ʵ�� �÷��̾� �̵�

    // 3. ���̵� ���� ���� �� �÷��̾� ���� ��� & ���� Ȱ��ȭ

    // 4. �÷��̾� ��ų ����

    // 5. ���� UI ����

    // 6. �÷��̾� - ���� �̵�

    // 7. �÷��̾� - ���� ��

    // 8. �÷��̾� - ���� ����ġ

    // ���� 4 ~ 8 �ݺ�



    // 1 2 3 ȣ��
    public void TurnFight_Start()
    {
        StartCoroutine(TurnFight_StartCall());
    }


    // 1 2 3 ����
    private IEnumerator TurnFight_StartCall() 
    {
        curTurn = Turn.Start;

        // 1. ���̵� ���� ȣ��
        Player_UI.instance.TurnFight_Fade();

        // ���̵� �� �ɶ����� ���
        yield return new WaitForSeconds(1f);


        // 2. ���� �ʵ�� �÷��̾� �̵�
        Player_Manager.instnace.Turn_Fight_Start(playerReturnPos, this);


        // ���̵� ���� ���� ���
        while (Player_UI.instance.isFade)
        {
            yield return null;
        }

        // 3. �÷��̾� ���� ��� & ���� Ȱ��ȭ
        Player_Manager.instnace.player_Turn.Turn_StartAnim();
        for (int i = 0; i < enemys.Count; i++)
        {
            enemys[i].gameObject.SetActive(true);
        }

        // ���� �ִϸ��̼� ���
        while (Player_Manager.instnace.player_Turn.isSpawnAnim)
        {
            yield return null;
        }

        // ���̵� �ƿ� �� �Ͻ� ���
        yield return new WaitForSeconds(1f);

        // 4. �÷��̾� ��ų ����
        StartCoroutine(Turn_Select());
    }


    // 4. �÷��̾� ��ų ����
    private IEnumerator Turn_Select()
    {
        curTurn = Turn.Select;

        // 4. �÷��̾� ��ų ����
        Player_Manager.instnace.player_Turn.Turn_AttackSelect();
        while(Player_Manager.instnace.player_Turn.isSelect)
        {
            yield return null;
        }
    }


    // �÷��̾ �ڽ��� ������ �����ϰ�, � ���Կ� ������ ������ ���ϱ� ���� ������ ����
    public List<Attack_Slot> GetEnemyAttackSlot()
    {
        List<Attack_Slot> enemyAttackList = new List<Attack_Slot>();

        // ��� ������ ���� ���� ��ȸ -> ó������ ���������� ���� ���� �� ���� �޾ƿ���
        for (int i = 0; i < enemys.Count; i++)
        {
            // Ȥ�� �� ����Ʈ null üũ
            if(enemys[i] != null)
            {
                // �ش� ������ ���� ������ ����Ʈ�� ���
                for (int i2 = 0; i2 < enemys[i].attack_Slots.Count; i2++)
                {
                    enemyAttackList.Add(enemys[i].attack_Slots[i2]);
                }
            }
        }

        // ���������� ����� ������ �ѱ��
        return enemyAttackList;
    }

    // ���� ��� ����
    private IEnumerator Turn_Fight()
    {
        curTurn = Turn.Fight;


        // 5. ���� UI ����
        Player_UI.instance.Turn_FightSelect(false);


        // ���� ���������� �����ϴ� ���


        // �÷��̾� & ���� ���� ����Ʈ�� �ϳ��� ��ġ�� ���
        List<Attack_Slot> combine = new List<Attack_Slot>();


        // ����Ʈ�� ���� ���� ���������� ����
        combine.Sort((a, b) => b.slotSpeed.CompareTo(a.slotSpeed));


        // ���� �ӵ���� ������ ����
        for (int i = 0; i < combine.Count; i++)
        {
            // ���� ����
            Attack_Setting(combine[i]);

            // 6. �÷��̾� - ���� �̵�
            switch (combine[i].attackType)
            {
                // ���� ���� ���
                case Attack_Slot.AttackType.None:
                    break;

                // �÷��̾� - ���� �̵� (�Ϲ� ����)
                case Attack_Slot.AttackType.Oneside_Attack:
                    StartCoroutine(Turn_ExchangeMove_OneSide(combine[i].slotType == Attack_Slot.SlotType.Player ? Object.Player : Object.Enemy));
                    break;

                // �÷��̾� - ���� �̵� (�� ����)
                case Attack_Slot.AttackType.Exchange_Attacks:
                    StartCoroutine(Turn_ExchangeMove_Exchange());
                    break;
            }

            // �̵� ���
            while(isExchangeMove)
            {
                yield return null;
            }


            // 7. �÷��̾� - ���� �� ���� �ִϸ��̼�
            if (combine[i].attackType == Attack_Slot.AttackType.Exchange_Attacks)
            {
                Player_Manager.instnace.player_Turn.Turn_ExchangeStartAnim();
                enemy.Turn_ExchangeStartAnim();
                yield return new WaitForSeconds(0.5f);
            }

            // 7. �÷��̾� - ���� �� ��� �� & ��� �ִϸ��̼�
            // ���� Ÿ�� �� ����
            switch (combine[i].attackType)
            {
                // �Ϲ� ������ ���
                case Attack_Slot.AttackType.Oneside_Attack:
                    StartCoroutine(Turn_OneSideAttack(combine[i].slotType == Attack_Slot.SlotType.Player ? Object.Player : Object.Enemy, combine[i]));
                    break;

                // �� ������ ���
                case Attack_Slot.AttackType.Exchange_Attacks:
                    StartCoroutine(Turn_ExchangeAttack());
                    break;
            }

            // ���� ���
            while(isAttack)
            {
                yield return null;
            }


            // ���� ���� �� ����Ʈ���� ����
            // -> ���� �� ������ ��� �� �� ����
            switch (combine[i].attackType)
            {
                case Attack_Slot.AttackType.None:
                case Attack_Slot.AttackType.Oneside_Attack:
                    combine.RemoveAt(i);
                    break;

                case Attack_Slot.AttackType.Exchange_Attacks:
                    combine.Remove(combine[i].targetSlot);
                    combine.RemoveAt(i);
                    break;
            }


            // ���� ���ݱ����� ��� ���
            yield return new WaitForSeconds(nextAttackDelay);
        }


        // 8. �÷��̾� - ���� ����ġ
        Turn_ReturnPos();
    }


    // �Ϲ� ���� �̵�
    private IEnumerator Turn_ExchangeMove_OneSide(Object moveObject)
    {
        isExchangeMove = true;

        switch (moveObject)
        {
            case Object.Player:
                while (Player_Manager.instnace.player_Turn.isExchangeMove)
                {
                    yield return null;
                }
                break;

            case Object.Enemy:
                while (enemy.isExchangeMove)
                {
                    yield return null;
                }
                break;
        }

        isExchangeMove = false;
    }


    // �÷��̾� - ���� ���� �Ҵ�
    private void Attack_Setting(Attack_Slot slot)
    {
        switch (slot.slotType)
        {
            case Attack_Slot.SlotType.Player:
                playerSlot = slot;
                enemySlot = slot.targetSlot;
                enemy = slot.targetSlot.slotOwner.GetComponent<Enemy_Base>();
                break;

            case Attack_Slot.SlotType.Enemy:
                enemySlot = slot;
                playerSlot = slot.targetSlot;
                enemy = slot.slotOwner.GetComponent<Enemy_Base>();
                break;
        }
    }


    // �� ���� �̵�
    private IEnumerator Turn_ExchangeMove_Exchange()
    {
        isExchangeMove = true;

        
        while (Player_Manager.instnace.player_Turn.isExchangeMove || enemy.isExchangeMove)
        {
            yield return null;
        }

        isExchangeMove = false;
    }


    // �Ϲ� ���� ���� -> ������� �۾�!
    private IEnumerator Turn_OneSideAttack(Object attackOwner, Attack_Slot slot)
    {
        isAttack = true;
        switch (attackOwner)
        {
            case Object.Player:
                slot.Attack();
                break;

            case Object.Enemy:
                slot.Attack();
                break;
        }
        yield return null;
        isAttack = false;
    }


    // �� ���� ����
    private IEnumerator Turn_ExchangeAttack()
    {
        isAttack = true;
        yield return null;
        isAttack = false;
    }


    // 8. �÷��̾� - ���� ����ġ
    private void Turn_ReturnPos()
    {
        Player_Manager.instnace.player_Turn.transform.position = playerReturnPos.position;
        for (int i = 0; i < enemys.Count; i++)
        {
            enemys[i].transform.position = enemyReturnPos[i].position;
        }
    }
}
