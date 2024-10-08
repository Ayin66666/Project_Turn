using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// �Ϲ� / �� �����͸� �޾Ƽ� ����Ʈȭ
[System.Serializable]
public class Exchange
{
    public enum AttackType { Oneside, Exchange }
    public enum AttackOwner { Player, Enemy }

    public AttackOwner owner; // ������ ��ü
    public AttackType attackType; // �� & �Ϲ� ���� üũ
    public Attack_Slot player_Slot; // �÷��̾� ����
    public Attack_Slot enemy_Slot; // ���ʹ� ����
    public int attackSpeed; // �÷��̾� ���� �ӵ�


    public void Slot_AttackTarget_Setting(AttackOwner owner)
    {
        if(attackType == AttackType.Oneside)
        {
            switch (owner)
            {
                case AttackOwner.Player:
                    player_Slot.targetSlot = enemy_Slot;
                    break;
                case AttackOwner.Enemy:
                    enemy_Slot.targetSlot = player_Slot;
                    break;
            }
        }
        else
        {
            player_Slot.targetSlot = enemy_Slot;
            enemy_Slot.targetSlot = player_Slot;
        }
    }

    public void Slot_AttackType_Setting(AttackType type)
    {
        switch (type)
        {
            case AttackType.Oneside:
                attackType = AttackType.Oneside;
                if(owner == AttackOwner.Player)
                {
                    player_Slot.attackType = Attack_Slot.AttackType.Oneside_Attack;
                }
                else
                {
                    enemy_Slot.attackType = Attack_Slot.AttackType.Oneside_Attack;
                }
                break;

            case AttackType.Exchange:
                attackType = AttackType.Exchange;
                player_Slot.attackType = Attack_Slot.AttackType.Exchange_Attacks;
                enemy_Slot.attackType = Attack_Slot.AttackType.Exchange_Attacks;
                break;
        }
    }
}


public class TurnFight_Manager : MonoBehaviour
{
    [Header("=== State ===")]
    [SerializeField] private Turn curTurn;
    [SerializeField] private int nextAttackDelay;

    private bool isExchangeMove;
    private bool isExchange;
    private bool isAttack;
    // [SerializeField] private bool alreadyHaveAttack; // �̰� ��ų �����Ҷ� ����ϴ� �Ұ���!

    private enum Turn { Await, Start, Select, Fight, End }
    public enum ExchangeType { OneSide, Exchange }
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
    [SerializeField] private List<Exchange> enemyExchangeList; // ���ʹ� ���� ����Ʈ
    [SerializeField] private List<Exchange> exchangeList; // ��ü ���� ����Ʈ
    [SerializeField] private List<Attack_Slot> enemySlots; // ���ʹ� ���� ������
   
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



    // �÷��̾� ���� ���� -> ����ü
    public void Exchange_Setting_Add(Attack_Slot player, Attack_Slot enemy)
    {
        // ����Ʈ�� �� ���ο� ���� ����
        Exchange newExchange = new Exchange();
        newExchange.owner = Exchange.AttackOwner.Player;
        newExchange.player_Slot = player;
        newExchange.enemy_Slot = enemy;
        newExchange.attackType = newExchange.player_Slot.slotSpeed >= newExchange.enemy_Slot.slotSpeed ? Exchange.AttackType.Exchange : Exchange.AttackType.Oneside;
        newExchange.attackSpeed = player.slotSpeed;
        Debug.Log("����ü �Է� ȣ���!");

        // ����Ʈ üũ -> ������� �۾�
        for (int i = 0; i < exchangeList.Count; i++)
        {
            Debug.Log(exchangeList[i].owner + " ������ ����");
            // �÷��̾��� ���� �� �̹� �ش� ������ �����ϴ� ������ �ִ��� üũ
            if (exchangeList[i].owner == Exchange.AttackOwner.Player && exchangeList[i].attackType == Exchange.AttackType.Exchange && exchangeList[i].enemy_Slot == newExchange.enemy_Slot)
            {
                // �ӵ��� �� �������� �� & �������� �Ϲ����� ����
                if (newExchange.attackSpeed >= exchangeList[i].attackSpeed)
                {
                    Debug.Log("�÷��̾� ������ �� ����");
                    exchangeList[i].Slot_AttackType_Setting(Exchange.AttackType.Oneside);

                    newExchange.Slot_AttackType_Setting(Exchange.AttackType.Exchange);
                    newExchange.Slot_AttackTarget_Setting(Exchange.AttackOwner.Player);
                    exchangeList.Add(newExchange);

                    // ���� ����
                    player.myExchange = newExchange;
                    return;
                }
                else
                {
                    Debug.Log("�÷��̾� ������ �� ����");
                    newExchange.Slot_AttackType_Setting(Exchange.AttackType.Oneside);
                    newExchange.Slot_AttackTarget_Setting(Exchange.AttackOwner.Player);
                    exchangeList.Add(newExchange);

                    // ���� ����
                    player.myExchange = newExchange;
                    return;
                }
            }

            // ���ʹ��� ���� �� �̹� �� ������ �����ϴ� ������ �ִ��� üũ
            if (exchangeList[i].owner == Exchange.AttackOwner.Enemy && exchangeList[i].enemy_Slot == newExchange.enemy_Slot)
            {
                Debug.Log("���ʹ� ���� üũ ȣ���!");

                // ������ ������ �̹� �� ������ �����Ѵٸ�
                if (newExchange.attackSpeed >= exchangeList[i].attackSpeed)
                {
                    Debug.Log("���ʹ� ���� �ӵ����� �÷��̾ �� ����! / �� �������");

                    // �ӵ��� �� �����ٸ� �ش� ������ ���� & �� �������� ���� �߰�
                    exchangeList.Remove(exchangeList[i]);

                    newExchange.Slot_AttackType_Setting(Exchange.AttackType.Exchange);
                    newExchange.Slot_AttackTarget_Setting(Exchange.AttackOwner.Player);
                    exchangeList.Add(newExchange);

                    // ���� ����
                    player.myExchange = newExchange;
                    return;
                }
                else
                {
                    Debug.Log("���ʹ� ���� �ӵ����� �÷��̾ �� ����! / �׳� �߰�");

                    // �� �����ٸ� �Ϲ�������� �߰�
                    newExchange.Slot_AttackType_Setting(Exchange.AttackType.Oneside);
                    newExchange.Slot_AttackTarget_Setting(Exchange.AttackOwner.Player);
                    exchangeList.Add(newExchange);

                    // ���� ����
                    player.myExchange = newExchange;
                    return;
                }
            }
        }

        // ���� ������ �������� �ʴ� �����̶��
        Debug.Log("�Ϲ� �Է� ȣ���!");

        exchangeList.Add(newExchange);
    }


    // �÷��̾� ���� ������ ���� -> ����ü
    public void Exchange_Setting_Remove(Attack_Slot player, Attack_Slot enemy, Exchange exchange)
    {
        // �̰� �߰��� ������� �����ϸ� �ε����� ������ ���µ�, �װ� �ƴ϶�� �ε����� ����Ǵϱ� ������ �߻���!
        Debug.Log("���� ���� ȣ��");
        // alreadyHaveAttack = false;
        if(exchange == null)
        {
            Debug.Log("���� �߻� / �ε��� ���� / indexof���� -1 ��ȯ");
            return;
        }

        // ������� ������ �� �����϶��� ����
        if(exchange.attackType == Exchange.AttackType.Exchange)
        {
            // ���ʹ� ������ ���� �����ϴ� ������ �־����� üũ
            for (int i = 0; i < enemyExchangeList.Count; i++)
            {
                if (enemyExchangeList[i].enemy_Slot == exchange.enemy_Slot)
                {
                    // ���� ���� �߰�
                    Exchange newAttack = enemyExchangeList[i];
                    newAttack.owner = Exchange.AttackOwner.Enemy;
                    newAttack.Slot_AttackType_Setting(Exchange.AttackType.Oneside);
                    exchangeList.Add(newAttack);

                    // ���� ����
                    exchangeList.Remove(exchange);
                    player.ResetSlot();
                    return;
                }
            }
        }
        else
        {
            // �� ������ �ƴϿ��ٸ�
            exchangeList.Remove(exchange);
            player.ResetSlot();
        }

        /*
        for (int i = 0; i < exchangeList.Count; i++)
        {
            if (exchangeList[i].player_Slot == player)
            {
                if(exchangeList[i].attackType == Exchange.AttackType.Exchange)
                {
                    // ���ʹ� ���Կ� ���� ������ �־����� üũ
                    for (int i2 = 0; i2 < enemyExchangeList.Count; i2++)
                    {
                        // ���ʹ� ���Կ� ���� ������ �־��ٸ�
                        if (enemyExchangeList[i2].enemy_Slot == enemy)
                        {
                            // �̹� �ش� ���ʹ� ������ ����Ʈ�� �� �ִ��� üũ
                            for (int i3 = 0; i3 < exchangeList.Count; i3++)
                            {
                                // �̹� ������ �� �ִٸ� bool �� true ��ȯ
                                if (exchangeList[i3].owner == Exchange.AttackOwner.Enemy && exchangeList[i3].enemy_Slot == enemyExchangeList[i2].enemy_Slot)
                                {
                                    alreadyHaveAttack = true;
                                }

                                // �� �ִٸ� ���� / �ƴ϶�� �߰�
                                if (!alreadyHaveAttack)
                                {
                                    Debug.Log("���� ���ʹ� ���� ���� / �߰��� !");

                                    // �ű� ���� ����
                                    Exchange newExchange = enemyExchangeList[i2];
                                    newExchange.owner = Exchange.AttackOwner.Enemy;
                                    newExchange.Slot_AttackType_Setting(Exchange.AttackType.Oneside);
                                    newExchange.Slot_AttackTarget_Setting(Exchange.AttackOwner.Enemy);

                                    // ���� ���� ����
                                    exchangeList[i].player_Slot.ResetSlot();
                                    exchangeList.RemoveAt(i);

                                    // �ű� ���� �߰�
                                    exchangeList.Add(newExchange);
                                    return;
                                }
                            }
                        }
                    }
                }
                else if (exchangeList[i].attackType == Exchange.AttackType.Oneside)
                {
                    // ���� ������ �����ٸ�
                    Debug.Log("���� ���� ! :" + exchangeList[i].player_Slot);
                    exchangeList[i].player_Slot.ResetSlot();
                    exchangeList[i].enemy_Slot.ResetSlot();
                    exchangeList.RemoveAt(i);
                    return;
                }
            }
        }
        */
    }


    // ���ʹ��� ���� ���� -> ����ü
    public void Exchange_Setting_Enemy(Attack_Slot enemy, Attack_Slot player)
    {
        // ������ ����
        Exchange data = new Exchange();
        data.owner = Exchange.AttackOwner.Enemy;
        data.attackType = Exchange.AttackType.Oneside;
        data.player_Slot = player;
        data.enemy_Slot = enemy;
        data.attackSpeed = enemy.slotSpeed;

        // ����Ʈ�� �߰�
        enemyExchangeList.Add(data);

        // ����Ʈ �ӵ� �� ����
        enemyExchangeList.Sort((a, b) => b.attackSpeed.CompareTo(a.attackSpeed));

        // �����͸� ��ü ���� ����Ʈ�� �߰�
        Debug.Log("������ �Է� ȣ��");
        exchangeList.Add(data);
        // exchangeList.AddRange(enemyExchangeList);
    }


    // ���� ���� ȣ�� (���� ���� �� 1ȸ ȣ��)
    public void TurnFight_Start()
    {
        StartCoroutine(TurnFight_StartCall());
    }


    // ���� ���� ���� (���� ���� �� 1ȸ ȣ��)
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
            enemys[i].turnManager = this;
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


    // �÷��̾� ��ų ����
    private IEnumerator Turn_Select()
    {
        curTurn = Turn.Select;

        // 4. �÷��̾� ��ų ����
        Player_Manager.instnace.player_Turn.Turn_AttackSelect();

        // �÷��̾� ���� ���ǵ� ����
        Player_Manager.instnace.player_Turn.Slot_SpeedSetting();

        // ���ʹ� ���� ����
        for (int i = 0;i < enemys.Count;i++)
        {
            // ���⿡ �ӵ� �Ѹ��� ��� �߰��ؾ� ��! 
            enemys[i].Turn_AttackSetting();
        }

        // ���� �Ϸ���� ���
        while(Player_Manager.instnace.player_Turn.isSelect)
        {
            yield return null;
        }

        // ���� UI ����
        Player_UI.instance.Turn_FightSelect(false);

        // ���� ����
        StartCoroutine(Turn_Fight());
    }


    // �÷��̾ �ڽ��� ������ �����ϰ�, � ���Կ� ������ ������ ���ϱ� ���� ������ ����
    public List<Attack_Slot> GetEnemyAttackSlot()
    {
        List<Attack_Slot> enemyAttackList = new List<Attack_Slot>();

        // ��� ���� ���� ���� ��ȸ -> ó������ ���������� ���� ���� �� ���� �޾ƿ���
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

        // ������ ����
        return enemyAttackList;
    }


    // ���� ��� ���� ( 5, 6, 7, 8 )
    private IEnumerator Turn_Fight()
    {
        Debug.Log("Call Turn Fight");
        curTurn = Turn.Fight;

        // ����Ʈ �ӵ� ������ ����
        exchangeList.Sort((a, b) => b.attackSpeed.CompareTo(a.attackSpeed));

        // �� ����Ʈ ��ŭ ���� ����
        for (int i = 0; i < exchangeList.Count; i++)
        {
            Debug.Log(i + "�� ���� ���� !");
            Debug.Log("���� Ÿ�� : " + exchangeList[i].attackType);
            Debug.Log("�÷��̾� ���� : " + exchangeList[i].player_Slot.myAttack + " / �� ���� : " + exchangeList[i].enemy_Slot.myAttack);
            
            // ���� ����
            playerSlot = exchangeList[i].player_Slot;
            enemySlot = exchangeList[i].enemy_Slot;
            enemy = enemySlot.slotOwner.GetComponent<Enemy_Base>();

            // �� �̵� ȣ��
            switch (exchangeList[i].attackType)
            {
                case Exchange.AttackType.Oneside:
                    StartCoroutine(Turn_ExchangeMove_OneSide(exchangeList[i].owner == Exchange.AttackOwner.Player ? Object.Player : Object.Enemy));
                    break;

                case Exchange.AttackType.Exchange:
                    StartCoroutine(Turn_ExchangeMove_Exchange());
                    break;
            }
            while(isExchangeMove)
            {
                yield return null;
            }

            // ���
            yield return new WaitForSeconds(0.25f);

            // �� ����
            if (exchangeList[i].attackType == Exchange.AttackType.Exchange)
            {
                Debug.Log("�� ����!");
                StartCoroutine(Turn_ExchangeAttack(exchangeList[i]));
                while (isExchange)
                {
                    yield return null;
                }
            }

            /*
            // ���� ����
            while(isAttack)
            {
                yield return null;
            }

            // ����ġ
            while(isExchangeMove)
            {
                yield return null;
            }
            */

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
                Player_Manager.instnace.player_Turn.Turn_ExchangeMove(enemy.exchangePos);
                while (Player_Manager.instnace.player_Turn.isExchangeMove)
                {
                    yield return null;
                }
                break;

            case Object.Enemy:
                enemy.Turn_ExchangeMove(Player_Manager.instnace.player_Turn.exchangePos);
                while (enemy.isExchangeMove)
                {
                    yield return null;
                }
                break;
        }

        isExchangeMove = false;
    }


    // �� ���� �̵�
    private IEnumerator Turn_ExchangeMove_Exchange()
    {
        isExchangeMove = true;

        Player_Manager.instnace.player_Turn.Turn_ExchangeMove(playerExchanagePos);
        enemy.Turn_ExchangeMove(enemyExchanagePos);

        while (Player_Manager.instnace.player_Turn.isExchangeMove || enemy.isExchangeMove)
        {
            yield return null;
        }

        isExchangeMove = false;
    }


    // �� ���� -> ��� ���� (����ü�� ����)
    private IEnumerator Turn_ExchangeAttack(Exchange exchange)
    {
        isAttack = true;
        
        // �� ����
        // �÷��̾� �Ǵ� ������ ���� Ƚ���� ���� �����ɶ����� �ݺ�
        int playerCount = 0;
        int enemyCount = 0;
        while (playerCount < playerSlot.myAttack.damageValue.Length && enemyCount < enemySlot.myAttack.damageValue.Length)
        {
            // ������ ���
            (int pdamage, bool isPC) = Player_Manager.instnace.DamageCal(playerSlot.myAttack, playerCount);
            (int edamage, bool isCC) = enemy.DamageCal(enemySlot.myAttack, enemyCount);
            Debug.Log("�÷��̾� ī��Ʈ : " + playerCount + " / ���ʹ� ī��Ʈ" + enemyCount);
            Debug.Log("�÷��̾� ������ : " + pdamage + " / " + "���ʹ� ������ : " + edamage);

            // ġ��Ÿ �߸� UI �κп��� �߰����� ����Ʈ ���� ��!

            // �� ���
            if(pdamage > edamage)
            {
                // �÷��̾� �¸�
                Player_Manager.instnace.player_Turn.Turn_ExchangeResuitAnim(Player_Turn.ExchangeResuit.Win);
                enemy.Turn_ExchangeResuitAnim(Enemy_Base.ExchangeResuit.Lose);
                enemyCount++;
                Debug.Log("Player Win!");
            }
            else if( pdamage == edamage)
            {
                // ���º�
                Player_Manager.instnace.player_Turn.Turn_ExchangeResuitAnim(Player_Turn.ExchangeResuit.Draw);
                enemy.Turn_ExchangeResuitAnim(Enemy_Base.ExchangeResuit.Draw);
                Debug.Log("Draw!");
            }
            else
            {
                // �ֳʹ� �¸�
                Player_Manager.instnace.player_Turn.Turn_ExchangeResuitAnim(Player_Turn.ExchangeResuit.Lose);
                enemy.Turn_ExchangeResuitAnim(Enemy_Base.ExchangeResuit.Win);
                playerCount++;
                Debug.Log("enemy Win!");
            }

            // �и� �̵� & �ִϸ��̼� ���
            while (Player_Manager.instnace.player_Turn.isRecoilMove || enemy.isRecoilMove)
            {
                yield return null;
            }

            // �� ��ġ ���� �̵� & �ִϸ��̼�
            Player_Manager.instnace.player_Turn.Turn_ExchangeMove_After(playerExchanagePos.position);
            enemy.Turn_ExchangeMove_After(playerExchanagePos.position);

            // �� ��ġ ���� �̵� & �ִϸ��̼� ���
            while (Player_Manager.instnace.player_Turn.isExchangeMove || enemy.isExchangeMove)
            {
                yield return null;
            }

            // ���� �� ������
            yield return new WaitForSeconds(Random.Range(0.3f, 0.5f));
        }

        // ���� ���� Ƚ�� üũ
        int count = (playerCount == 0 ? enemySlot.myAttack.attackCount - enemyCount : playerSlot.myAttack.attackCount - playerCount);
        Debug.Log("���� ���� Ƚ�� : " + count);


        // �÷��̾� - ���� ���� ȣ��
        StartCoroutine(Turn_Attack((playerCount == 0 ? Object.Enemy : Object.Player), (playerCount == 0 ? enemySlot : playerSlot), count));
    }


    // �� �¸� & �Ϲ���� ȣ��
    private IEnumerator Turn_Attack(Object win, Attack_Slot slot, int attackCount)
    {
        switch(win)
        {
            case Object.Player:
                // ���� ȣ��
                Player_Manager.instnace.player_Turn.Turn_Attack(enemy.gameObject, slot.myAttack, attackCount);

                // ���� ������� ���
                while (Player_Manager.instnace.isAttack)
                {
                    yield return null;
                }
                break;

            case Object.Enemy:
                // ���� ȣ��
                enemy.Turn_Attack(Player_Manager.instnace.player_Turn.gameObject, slot.myAttack, attackCount);

                // ���� ������� ���
                while (enemy.isAttack)
                {
                    yield return null;
                }
                break;
        }

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


    #region ���� �¸� & �й�
    // ���� ���� üũ
    private void Turn_StageCheck()
    {

    }


    // ���� �¸� ȣ��
    private void Turn_FightWin()
    {
        StartCoroutine(TurnFightEndCall());
    }


    // ���� �¸� ����
    private IEnumerator TurnFightEndCall()
    {
        curTurn = Turn.End;

        // ���� ���� �ִϸ��̼� ȣ��
        Player_Manager.instnace.player_Turn.Turn_End(Player_Turn.EndType.Win);
        while (Player_Manager.instnace.player_Turn.isEndAnim)
        {
            yield return null;
        }

        // ���� ���� UI ȣ��
        Player_UI.instance.TurnFight_Lose();
        while(Player_UI.instance.isFade)
        {
            yield return null;
        }

        // �÷��̾� ����ġ
        Player_Manager.instnace.Turn_Fight_End();

        // �Ŵ��� �ı�
        Destroy(gameObject);
    }


    // ���� �й� ȣ��
    private void Turn_FightLose()
    {
        StartCoroutine(TurnFightLoseCall());
    }


    // ���� �й� ����
    private IEnumerator TurnFightLoseCall()
    {
        curTurn = Turn.End;

        // ���� ���� �ִϸ��̼� ȣ��
        Player_Manager.instnace.player_Turn.Turn_End(Player_Turn.EndType.Lose);
        while(Player_Manager.instnace.player_Turn.isEndAnim)
        {
            yield return null;
        }

        // ���� ���� UI ȣ�� -> �̰� �ϴ� �¸��� �־��!
        Player_UI.instance.TurnFight_Lose();

        // 
    } // -> �̿ϼ�
    #endregion
}
