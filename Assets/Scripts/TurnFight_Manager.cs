using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnFight_Manager : MonoBehaviour
{
    [Header("=== State ===")]
    [SerializeField] private Turn curTurn;
    [SerializeField] private int nextAttackDelay;

    private enum Turn { Await, Start, Select, Fight, End }
    private enum ExchangeType { OneSide, Exchange }

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
            switch (combine[i].attackType)
            {
                case Attack_Slot.AttackType.None: // ���� ���� ���
                    break;

                case Attack_Slot.AttackType.Oneside_Attack: // 6. �÷��̾� - ���� �̵� (�Ϲ� ����)
                    // ���� - �÷��̾� �� ������ �������� üũ
                    switch (combine[i].slotType) 
                    {
                        case Attack_Slot.SlotType.Player:
                            Player_Manager.instnace.player_Turn.Turn_ExchangeMove(playerExchanagePos);
                            break;

                        case Attack_Slot.SlotType.Enemy:
                            combine[i].slotOwner.GetComponent<Enemy_Base>().Turn_ExchangeMove(enemyExchanagePos);
                            break;
                    }
                    break;

                case Attack_Slot.AttackType.Exchange_Attacks: // 6. �÷��̾� - ���� �̵� (�� ����)
                    Player_Manager.instnace.player_Turn.Turn_ExchangeMove(playerExchanagePos);
                    combine[i].slotOwner.GetComponent<Enemy_Base>().Turn_ExchangeMove(enemyExchanagePos);
                    break;
            }


            // �̵� ���
            if (combine[i].attackType == Attack_Slot.AttackType.Exchange_Attacks)
            {
                // �� ������ ��� - �� �� �̵��� ���������� ���
                Enemy_Base enemy = combine[i].slotOwner.GetComponent<Enemy_Base>();
                while (Player_Manager.instnace.player_Turn.isExchangeMove || enemy.isExchangeMove)
                {
                    yield return null;
                }
            }
            else
            {
                // �Ϲ� ������ ���
                switch (combine[i].slotType)
                {
                    // �÷��̾� �Ϲ� ����
                    case Attack_Slot.SlotType.Player:
                        while(Player_Manager.instnace.player_Turn.isExchangeMove)
                        {
                            yield return null;
                        }
                        break;
                    // �ֳʹ� �Ϲ� ����
                    case Attack_Slot.SlotType.Enemy:
                        Enemy_Base enemy = combine[i].slotOwner.GetComponent<Enemy_Base>();
                        while (enemy.isExchangeMove)
                        {
                            yield return null;
                        }
                        break;
                }
            }

            // 7. �÷��̾� - ���� ��

            // ���� ��� ����?
            //  - ���� Ÿ�� üũ �� ������ ���ݰ� �޾ƿ���
            //  - �� �޾ƿ��� �迡 ���� �� ��� ����
            //  - ���� �� ���� ��
            //  - ���� �� �� �������� �¸�, �� ���� ���� Ƚ�� ����
            //  - ������ ���� Ƚ���� 0 �� �ɶ����� �ݺ�
            //  - ������ �̱� �ʿ��� ���� �ǽ�

            switch (combine[i].attackType)
            {
                case Attack_Slot.AttackType.Oneside_Attack:

                    switch (combine[i].slotType)
                    {
                        case Attack_Slot.SlotType.Player:
                            for (int i2 = 0; i2 < combine[i].remainingAttackCount; i2++)
                            {
                                int damage = combine[i].DamageCal(i2);
                                // �̰� ������ ���������� �Ϻ����ε�, ���ݿ� ������ Ÿ�� ������ ��!
                                combine[i].targetSlot.slotOwner.GetComponent<Enemy_Base>().TakeDamage(damage, Enemy_Base.DamageType.phsical);
                            }
                            break;

                        case Attack_Slot.SlotType.Enemy:
                            for (int i2 = 0; i2 < combine[i].remainingAttackCount; i2++)
                            {
                                int damage = combine[i].DamageCal(i2);
                                Player_Manager.instnace.Take_Damage(damage, Player_Manager.DamageType.phsical);
                            }
                            break;
                    }
                    break;

                case Attack_Slot.AttackType.Exchange_Attacks:
                    break;
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
        Player_Manager.instnace.player_Turn.transform.position = playerReturnPos.position;
        for (int i = 0; i < enemys.Count; i++)
        {
            enemys[i].transform.position = enemyReturnPos[i].position;
        }
    }
}
