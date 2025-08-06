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
        if (attackType == AttackType.Oneside)
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
                if (owner == AttackOwner.Player)
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
    [SerializeField] private RoomType roomType;
    [SerializeField] private Turn curTurn;
    [SerializeField] private float nextAttackDelay;
    [SerializeField] private int skillPoint;

    private bool isExchangeMove;
    private bool isExchange;
    private bool isAttack;
    private bool isEnd;
    private bool isFrist = true; // �̰� �� ó�� ���� ������ �� �÷��̾� UI �κ� 1ȸ�� üũ �Ұ���!
    private int turnCount = 1;


    [Header("=== Dialog ===")]
    [SerializeField] private bool haveStartDialog;
    [SerializeField] private bool haveEndDialog;
    [SerializeField] private int startDialogIndex;
    [SerializeField] private int endDialogIndex;


    [Header("=== Tutorial ===")]
    [SerializeField] private bool haveStartTutorial;
    [SerializeField] private bool haveEndTutorial;
    [SerializeField] private int[] tutorialIndex;
    [SerializeField] private int tutorialStartTurn;


    [Header("=== Phase Setting ===")]
    [SerializeField] private bool havePhase2;
    private bool phase2;


    private enum RoomType { Normal, Boss }
    private enum Turn { Await, Start, Select, Fight, End }
    public enum ExchangeType { OneSide, Exchange }
    private enum Object { Player, Enemy }

    [Header("=== Pos Setting ===")]
    [SerializeField] private Transform playerExchanagePos;
    [SerializeField] private Transform enemyExchanagePos;
    public Transform exchangeHolderPos;

    [SerializeField] private Transform[] enemyReturnPos;
    public Transform playerReturnPos;


    [Header("=== Enemy Setting ===")]
    public List<GameObject> enemyObjects; // ���ʹ� ������Ʈ ��ü
    public List<Enemy_Base> enemys;
    [SerializeField] private int enemyCount;


    [Header("=== Attack Setting===")]
    [SerializeField] private List<Exchange> enemyExchangeList; // ���ʹ� ���� ����Ʈ
    [SerializeField] private List<Exchange> exchangeList; // ��ü ���� ����Ʈ
    [SerializeField] private List<Attack_Slot> enemySlots; // ���ʹ� ���� ������
    [SerializeField] private Attack_Slot playerSlot;
    [SerializeField] private Attack_Slot enemySlot;
    [SerializeField] private Enemy_Base enemy;


    // 1. ���̵� ���� ȣ��

    // 2. ���� �ʵ�� �÷��̾� �̵�

    // 3. ���̵� ���� ���� �� �÷��̾� ���� ��� & ���� Ȱ��ȭ

    // 4. �÷��̾� ��ų ����

    // 5. ���� UI ����

    // 6. �÷��̾� - ���� �̵�

    // 7. �÷��̾� - ���� ��

    // 8. �÷��̾� - ���� ����ġ

    // ���� 4 ~ 8 �ݺ�

    public void Exchange_Setting_Add2(Attack_Slot player, Attack_Slot enemy)
    {
        // ����Ʈ�� �� ���ο� ���� ����
        Exchange newExchange = new Exchange();
        newExchange.owner = Exchange.AttackOwner.Player;
        newExchange.player_Slot = player;
        newExchange.enemy_Slot = enemy;
        newExchange.attackType = newExchange.player_Slot.slotSpeed >= newExchange.enemy_Slot.slotSpeed ? Exchange.AttackType.Exchange : Exchange.AttackType.Oneside;
        newExchange.attackSpeed = player.slotSpeed;

        // �����Ϸ��� ������ ���� �븰�ٸ�
        if (enemy.targetSlot == player)
        {
            // �� ���� ��ȯ
            newExchange.Slot_AttackType_Setting(Exchange.AttackType.Exchange);
            newExchange.Slot_AttackTarget_Setting(Exchange.AttackOwner.Player);
            exchangeList.Add(newExchange);
            player.myExchange = newExchange;
            return;
        }

        // �ӵ��� �� ���� ���� �����ϴٸ�
        if (player.slotSpeed >= enemy.slotSpeed)
        {
            // �ش� ���ݰ� �� �ϰ� �ִ� ������ �ִ��� üũ
            for (int i = 0; i < exchangeList.Count; i++)
            {
                // ������ �Ϲ� �������� ��ȯ
                if (exchangeList[i].owner == Exchange.AttackOwner.Player && exchangeList[i].attackType == Exchange.AttackType.Exchange && exchangeList[i].enemy_Slot == newExchange.enemy_Slot)
                {
                    exchangeList[i].Slot_AttackType_Setting(Exchange.AttackType.Oneside);
                }
            }

            // �� ���� ��ȯ
            newExchange.Slot_AttackType_Setting(Exchange.AttackType.Exchange);
            newExchange.Slot_AttackTarget_Setting(Exchange.AttackOwner.Player);
            exchangeList.Add(newExchange);
            player.myExchange = newExchange;
            return;
        }
        // �ӵ��� ������ ���� �Ұ����ϴٸ�
        else
        {
            newExchange.Slot_AttackType_Setting(Exchange.AttackType.Oneside);
            newExchange.Slot_AttackTarget_Setting(Exchange.AttackOwner.Player);
            exchangeList.Add(newExchange);

            // ���� ����
            player.myExchange = newExchange;
            return;
        }
    }

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

        // ����Ʈ üũ -> ������� �۾�
        for (int i = 0; i < exchangeList.Count; i++)
        {
            // �÷��̾��� ���� �� �̹� �ش� ������ �����ϴ� ������ �ִ��� üũ
            if (exchangeList[i].owner == Exchange.AttackOwner.Player && exchangeList[i].attackType == Exchange.AttackType.Exchange && exchangeList[i].enemy_Slot == newExchange.enemy_Slot)
            {
                // �ӵ��� �� �������� �� & �������� �Ϲ����� ����
                /*
                if (newExchange.attackSpeed >= exchangeList[i].attackSpeed)
                {
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
                    newExchange.Slot_AttackType_Setting(Exchange.AttackType.Oneside);
                    newExchange.Slot_AttackTarget_Setting(Exchange.AttackOwner.Player);
                    exchangeList.Add(newExchange);

                    // ���� ����
                    player.myExchange = newExchange;
                    return;
                }
                */
                exchangeList[i].Slot_AttackType_Setting(Exchange.AttackType.Oneside);

                newExchange.Slot_AttackType_Setting(Exchange.AttackType.Exchange);
                newExchange.Slot_AttackTarget_Setting(Exchange.AttackOwner.Player);
                exchangeList.Add(newExchange);

                // ���� ����
                player.myExchange = newExchange;
                return;
            }

            // ���ʹ��� ���� �� �̹� �� ������ �����ϴ� ������ �ִ��� üũ
            if (exchangeList[i].owner == Exchange.AttackOwner.Enemy && exchangeList[i].enemy_Slot == newExchange.enemy_Slot)
            {
                // ������ ������ �̹� �� ������ �����Ѵٸ�
                /*
                if (newExchange.attackSpeed >= exchangeList[i].attackSpeed)
                {
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
                    // �� �����ٸ� �Ϲ�������� �߰�
                    newExchange.Slot_AttackType_Setting(Exchange.AttackType.Oneside);
                    newExchange.Slot_AttackTarget_Setting(Exchange.AttackOwner.Player);
                    exchangeList.Add(newExchange);

                    // ���� ����
                    player.myExchange = newExchange;
                    return;
                }
                */
                // �ӵ��� �� �����ٸ� �ش� ������ ���� & �� �������� ���� �߰�
                exchangeList.Remove(exchangeList[i]);

                newExchange.Slot_AttackType_Setting(Exchange.AttackType.Exchange);
                newExchange.Slot_AttackTarget_Setting(Exchange.AttackOwner.Player);
                exchangeList.Add(newExchange);

                // ���� ����
                player.myExchange = newExchange;
                return;
            }
        }

        // ���� ������ �������� �ʴ� �����̶��
        exchangeList.Add(newExchange);
    }


    // �÷��̾� ���� ������ ���� -> ����ü -> �̰� ���� ���ʹ��� ����ü ���� ������ �κп��� exchange�� ����Ǵ� �̽�����...
    public void Exchange_Setting_Remove(Attack_Slot player, Attack_Slot enemy, Exchange exchange)
    {
        // �̰� �߰��� ������� �����ϸ� �ε����� ������ ���µ�, �װ� �ƴ϶�� �ε����� ����Ǵϱ� ������ �߻���!
        if (exchange == null)
        {
            return;
        }

        // ���� ��ư ��Ȱ��ȭ
        Player_UI.instance.Turn_AttackButton(false);


        // �ڽ�Ʈ ��ȯ
        Player_Manager.instnace.AttackPointAdd(exchange.player_Slot.myAttack.attackCost);


        // ���� Ÿ�� üũ (�Ϲ� / ��)
        switch (exchange.attackType)
        {
            // �� ������ �ƴϿ��ٸ�
            case Exchange.AttackType.Oneside:

                exchangeList.Remove(exchange);
                player.ResetSlot();
                break;

            case Exchange.AttackType.Exchange:
                // ���ʹ� ������ ���� �����ϴ� ������ �־����� üũ
                for (int i = 0; i < enemyExchangeList.Count; i++)
                {
                    if (enemyExchangeList[i].enemy_Slot == exchange.enemy_Slot)
                    {
                        // ���� ���� �߰�
                        Exchange newAttack = enemyExchangeList[i];
                        newAttack.owner = Exchange.AttackOwner.Enemy;
                        newAttack.Slot_AttackType_Setting(Exchange.AttackType.Oneside);

                        // ���� ����
                        exchangeList.Remove(exchange);
                        player.ResetSlot();

                        exchangeList.Add(newAttack);
                        return;
                    }
                }
                break;

        }
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
        exchangeList.Add(data);
    }


    public void Phase2()
    {
        phase2 = true;
    }


    // ���� ���� ȣ�� (���� ���� �� 1ȸ ȣ��)
    public void TurnFight_Start()
    {
        StartCoroutine(TurnFight_StartCall());
    }


    // ���� ���� ���� - �븻 (���� ���� �� 1ȸ ȣ��)
    private IEnumerator TurnFight_StartCall()
    {
        curTurn = Turn.Start;

        // ���̾�α� ����
        Dialog_Manager.instance.DialogOff();

        // Ʃ�丮�� ����
        if(Tutorial_Manager.instance != null)
        {
            Tutorial_Manager.instance.Tutorial_Reset();
        }

        // ���� Off
        Stage_Manager.instance.BGM_OnOff(false);

        // ���� UI Off
        Player_UI.instance.World_UISetting(false);

        // Ŀ�� ����
        Player_Manager.instnace.Cursor_Setting(false);

        // 1. ���̵� ���� ȣ��
        Player_UI.instance.TurnFight_Fade(roomType != RoomType.Normal);

       // ���̵� �� �ɶ����� ���
        yield return new WaitForSeconds(0.75f);

        // ��������Ʈ ����
        Waypoint_Manager.instance.Waypoint_Setting(false, 0);

        // ���� Ȱ��ȭ
        for (int i = 0; i < enemys.Count; i++)
        {
            enemys[i].turnManager = this;
            enemys[i].transform.position = enemyReturnPos[i].position;
            enemys[i].gameObject.SetActive(true);
        }

        // ���� On
        Stage_Manager.instance.BGM_Setting(1);

        // 2. ���� �ʵ�� �÷��̾� �̵�
        Player_Manager.instnace.Turn_Fight_Start(playerReturnPos, this);

        // �÷��̾� �� �߾� �ٶ󺸰� �ϱ�
        Player_Manager.instnace.player_Turn.LookAt(exchangeHolderPos.gameObject);

        // ���̵� ���� ���� ���
        while (Player_UI.instance.isFade)
        {
            yield return null;
        }

        // ������ - �ƽ� ���
        if (roomType == RoomType.Boss)
        {
            while (enemys[0].isCutscene)
            {
                yield return null;
            }
        }

        // 3. �÷��̾� & ���� ���� ��� + �� �߾� �ٶ󺸱�
        Player_Manager.instnace.player_Turn.Turn_StartAnim();
        for (int i = 0; i < enemys.Count; i++)
        {
            enemys[i].Spawn();
            enemys[i].LookAt(Player_Manager.instnace.player_Turn.gameObject);
        }

        // ���ʹ� ���� ���� - UI Ȱ��ȭ
        for (int i = 0; i < enemys.Count; i++)
        {
            enemys[i].enemyUI.UI_Setting(true);
            for (int i2 = 0; i2 < enemys[i].attack_Slots.Count; i2++)
            {
                enemySlots.Add(enemys[i].attack_Slots[i2]);
            }
        }

        // ���� �ִϸ��̼� ���
        while (Player_Manager.instnace.player_Turn.isSpawnAnim)
        {
            yield return null;
        }

        // ���� ���� UI
        Player_UI.instance.Turn_StartUI();

        // ���̾�α� ȣ��
        if (haveStartDialog)
        {
            Dialog_Manager.instance.Dialog(startDialogIndex);
        }

        // ���� UI On
        Player_UI.instance.Turn_UndersideGaugeSetting();
        Player_Manager.instnace.UnderSideButtonOn();
        Player_UI.instance.Turn_FightSelect(true);

        // 4. �÷��̾� ����
        Player_Manager.instnace.player_Turn.forwardTarget = exchangeHolderPos.gameObject;
        Player_Manager.instnace.player_Turn.Turn_AttackSelect();
        Player_Manager.instnace.player_Turn.Slot_SpeedSetting();

        // Ʃ�丮��
        if (haveStartTutorial)
        {
            Tutorial_Manager.instance.Tutorial_Fight();
            //Tutorial_Manager.instance.TutorialOn(tutorialIndex[0]);
        }

        // ���̵� �ƿ� �� �Ͻ� ���
        yield return new WaitForSeconds(0.25f);

        // 4. �÷��̾� ��ų ����
        StartCoroutine(Turn_Select());
    }


    // �÷��̾� ��ų ����
    private IEnumerator Turn_Select()
    {
        curTurn = Turn.Select;

        // 2������ ȣ��
        if (phase2)
        {
            phase2 = false;
            enemys[0].Phase2();
        }

        // �������� üũ
        Turn_StageCheck();
        if (isEnd)
        {
            yield break;
        }

        // �� UI
        if(!isFrist)
        {
            turnCount++;
            Player_UI.instance.Turn_TurnCountUI(turnCount);
        }

        // ����Ʈ �ʱ�ȭ
        Turn_ListReset();

        // ���ʹ� üũ
        Enemy_Check();

        // ���� ��ư ��Ȱ��ȭ
        Player_UI.instance.Turn_AttackButton(false);

        // �ڿ� ȸ��
        Player_Manager.instnace.Turn_AttackPointRecovery();

        // ����
        Player_Manager.instnace.UI_Sound_Call(4);

        // ���ʹ� ���� ���� & UI On
        for (int i = 0; i < enemys.Count; i++)
        {
            if (!enemys[i].isDie)
            {
                enemys[i].Turn_AttackSetting(gameObject);
                enemys[i].enemyUI.Turn_FightSelect(true);
                enemys[i].LookAt(Player_Manager.instnace.player_Turn.gameObject);
            }
        }

        // �� ó�� �� ���ۿ��� ���Ⱑ �ƴ϶� ������ UI Ȱ��ȭ��Ŵ!
        if (isFrist)
        {
            isFrist = false;
        }
        else
        {
            // ���� UI On
            Player_UI.instance.Turn_UndersideGaugeSetting();
            Player_Manager.instnace.UnderSideButtonOn();
            Player_UI.instance.Turn_FightSelect(true);
            Player_UI.instance.hpSet.SetActive(true);

            // 4. �÷��̾� ���ǵ� & ���� ����
            Player_Manager.instnace.player_Turn.Turn_AttackSelect();
            Player_Manager.instnace.player_Turn.Slot_SpeedSetting();
        }


        // ���� �Ϸ���� ���
        while (Player_Manager.instnace.player_Turn.isSelect)
        {
            yield return null;
        }

        // ���� UI ����
        Player_UI.instance.Turn_FightSelect(false);

        // ���ʹ� UI Off
        for (int i = 0; i < enemys.Count; i++)
        {
            enemys[i].enemyUI.Turn_FightSelect(false);
        }

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
            if (enemys[i] != null)
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


    // �÷��̾� �ʻ�� ��� �� �� �ʱ�ȭ ��� ȣ��-> �ʱ�ȭ ��ɸ� ������ ��!
    public void Turn_UnderSideAttack()
    {
        StartCoroutine(Turn_UnderSideAttackCall());
    }

    private IEnumerator Turn_UnderSideAttackCall()
    {
        isAttack = true;

        // ������ �ʱ�ȭ
        Player_Manager.instnace.UnderSideGaugeAdd(-999);

        // UI ��Ȱ��ȭ
        Player_UI.instance.Turn_FightSelect(false);

        // �����ڸ� UI Ȱ��ȭ
        int ran = Random.Range(0, 2);

        // ���� ����Ʈ ��ü �ʱ�ȭ
        exchangeList.Clear();
        enemyExchangeList.Clear();

        // �÷��̾� ���� ���� �ʱ�ȭ
        for (int i = 0; i < Player_Manager.instnace.player_Turn.attackSlot.Count; i++)
        {
            if (Player_Manager.instnace.player_Turn.attackSlot[i].haveAttack)
            {
                Player_Manager.instnace.AttackPointAdd(Player_Manager.instnace.player_Turn.attackSlot[i].myAttack.attackCost);
                Player_Manager.instnace.player_Turn.attackSlot[i].ResetSlot();
            }
        }

        // ���� ������� ���
        yield return new WaitForSeconds(0.5f);
        while (Player_Manager.instnace.isAttack)
        {
            yield return null;

        }

        // 8. �÷��̾� ����ġ
        Player_Manager.instnace.player_Turn.Turn_ExchangeEndMove(playerReturnPos, 1.5f);
        while (Player_Manager.instnace.player_Turn.isExchangeMove)
        {
            yield return null;
        }

        // �����ڸ� UI ��Ȱ��ȭ
        Player_UI.instance.TurnFight_Outline_Setting(ran == 0 ? Player_UI.Outline.TypeA : Player_UI.Outline.TypeB, false);

        // ������ UI ����
        Player_UI.instance.TurnFight_ExchanageSummary(false, null);

        // �÷��̾��� ���� Ÿ�� ������ �ʱ�ȭ
        Player_Manager.instnace.player_Turn.Turn_AttackData_Reset();

        yield return new WaitForSeconds(0.1f); // ���� �ذ� ��� -> �ٷ� �ִϸ��̼��� ȣ���ؼ� ���� �����ε� / 0.1�� ���ð� ��


        // �������� ���� üũ
        Turn_StageCheck();
        if(!isEnd)
        {
            // 9. �÷��̾� ���� �� ����
            isAttack = false;
            StartCoroutine(Turn_Select());
        }
    }


    // ���� ��� ���� ( 5, 6, 7, 8 )
    private IEnumerator Turn_Fight()
    {
        curTurn = Turn.Fight;

        // ����
        Player_Manager.instnace.UI_Sound_Call(3);

        // ����Ʈ �ӵ� ������ ����
        exchangeList.Sort((a, b) => b.attackSpeed.CompareTo(a.attackSpeed));

        // UI Off
        Player_UI.instance.Turn_FightSelect(false);
        Player_UI.instance.hpSet.SetActive(false);

        // �����ڸ� UI Ȱ��ȭ
        int ran = Random.Range(0, 2);
        Player_UI.instance.TurnFight_Outline_Setting(ran == 0 ? Player_UI.Outline.TypeA : Player_UI.Outline.TypeB, true);

        // �� ����Ʈ ��ŭ ���� ����
        for (int i = 0; i < exchangeList.Count; i++)
        {
            // �������� ���� üũ
            Turn_StageCheck();
            if (isEnd)
            {
                yield break;
            }

            // Ÿ���� ��� üũ
            if (!exchangeList[i].enemy_Slot.slotOwner.GetComponent<Enemy_Base>().isDie)
            {
                // �÷��̾��� ���� Ÿ�� ������ �ʱ�ȭ
                Player_Manager.instnace.player_Turn.Turn_AttackData_Reset();

                // �÷��̾� & ���� ���� ����
                Turn_Slot_Setting(i);

                // ī�޶� ����Ʈ
                Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.ExchangeA, 0.35f);

                // �� �̵� ȣ�� && �̵� ���
                Turn_ExchangeMove(exchangeList[i]);
                while (isExchangeMove)
                {
                    yield return null;
                }

                // �Ϲ� ���� or �� ���� ȣ��
                Turn_Exchange(exchangeList[i]);
                while (isAttack || isExchange)
                {
                    yield return null;
                }

                // UI ����
                Player_UI.instance.TurnFight_ExchanageSummary(false, null);
                enemy.enemyUI.TurnFight_ExchanageSummary(false, null);

                // ī�޶� ����
                Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.None, 0.75f);

                // ���� ��ġ�� ����
                Turn_Return(exchangeList[i]);
                while (Player_Manager.instnace.player_Turn.isExchangeMove || enemy.isExchangeMove)
                {
                    yield return null;
                }


                // ���� ���ݱ����� ��� ���
                // yield return new WaitForSeconds(nextAttackDelay);
            }
        }

        // �����ڸ� UI ��Ȱ��ȭ
        Player_UI.instance.TurnFight_Outline_Setting(ran == 0 ? Player_UI.Outline.TypeA : Player_UI.Outline.TypeB, false);

        // �������� ���� üũ
        Turn_StageCheck();

        // 9. �÷��̾� ���� �� ����
        StartCoroutine(Turn_Select());
    }


    // �÷��̾� & ���� ���� ����
    private void Turn_Slot_Setting(int index)
    {
        // �÷��̾� & ���� ���� ����
        playerSlot = exchangeList[index].player_Slot;
        enemySlot = exchangeList[index].enemy_Slot;
        enemy = enemySlot.slotOwner.GetComponent<Enemy_Base>();
    }

    // �� �̵� ȣ�� �κ�
    private void Turn_ExchangeMove(Exchange exchange)
    {
        switch (exchange.attackType)
        {
            case Exchange.AttackType.Oneside:
                break;

            case Exchange.AttackType.Exchange:
                if (exchange.owner == Exchange.AttackOwner.Enemy)
                {
                    StartCoroutine(Turn_AttackCall(Object.Enemy, enemySlot, false, enemySlot.myAttack.damageValue.Length)); // -> ���� ��� �����...
                }
                else
                {
                    StartCoroutine(Turn_ExchangeMove_Exchange());
                }
                break;
        }
    }


    // �� ���� �̵�
    private IEnumerator Turn_ExchangeMove_Exchange()
    {
        isExchangeMove = true;

        float ran = Random.Range(1.2f, 1.7f);
        Player_Manager.instnace.player_Turn.Turn_ExchangeMove(playerExchanagePos, ran);
        enemy.Turn_ExchangeMove(playerExchanagePos, ran);

        while (Player_Manager.instnace.player_Turn.isExchangeMove || enemy.isExchangeMove)
        {
            yield return null;
        }

        isExchangeMove = false;
    }


    // �� ���� or �Ϲ���� ȣ��
    private void Turn_Exchange(Exchange exchange)
    {
        switch (exchange.attackType)
        {
            // �Ϲ� ����
            case Exchange.AttackType.Oneside:
                switch (exchange.owner)
                {
                    case Exchange.AttackOwner.Player:
                        StartCoroutine(Turn_AttackCall(Object.Player, playerSlot, false, playerSlot.myAttack.damageValue.Length));
                        break;

                    case Exchange.AttackOwner.Enemy:
                        StartCoroutine(Turn_AttackCall(Object.Enemy, enemySlot, false, enemySlot.myAttack.damageValue.Length));
                        break;
                }
                break;

            // �� ����
            case Exchange.AttackType.Exchange:
                StartCoroutine(Turn_ExchangeCall(exchange));
                break;
        }
    }


    // �� ����
    private IEnumerator Turn_ExchangeCall(Exchange exchange)
    {
        isExchange = true;
        Player_UI.instance.TurnFight_ExchanageSummary(true, playerSlot.myAttack);

        // �� ����
        // �÷��̾� �Ǵ� ������ ���� Ƚ���� ���� �����ɶ����� �ݺ�
        int playerCount = 0;
        int enemyCount = 0;
        bool loop = false;
        while (playerCount < playerSlot.myAttack.damageValue.Length && enemyCount < enemySlot.myAttack.damageValue.Length)
        {
            // ù��° ���� ����
            if (loop)
            {
                // �� ��ġ ���� �̵� & �ִϸ��̼�
                float moveSpeed = Random.Range(1.5f, 1.7f);
                Player_Manager.instnace.player_Turn.Turn_ExchangeMove(playerExchanagePos, moveSpeed);
                enemy.Turn_ExchangeMove(enemyExchanagePos, moveSpeed);

                // �� ��ġ ���� �̵� & �ִϸ��̼� ���
                while (Player_Manager.instnace.player_Turn.isExchangeMove || enemy.isExchangeMove)
                {
                    yield return null;
                }
            }
            else
            {
                loop = true;
            }

            // �÷��̾� ���� �̵� UI
            Player_UI.instance.TurnFight_ExchanageSummary(true, playerSlot.myAttack);
            Player_UI.instance.TurnFight_ExchangeSummary_AttackCount(playerCount);

            // ���ʹ� ���� �̵� UI
            enemy.enemyUI.TurnFight_ExchanageSummary(true, enemySlot.myAttack);
            enemy.enemyUI.TurnFight_ExchangeSummary_AttackCount(enemyCount);


            // ������ ���
            (int pdamage, bool isPC) = Player_Manager.instnace.DamageCal(playerSlot.myAttack, playerCount);
            (int edamage, bool isCC) = enemy.DamageCal(enemySlot.myAttack, enemyCount);


            // ������ UI
            //float ran = Random.Range(0.1f, 0.15f);
            //Player_UI.instance.TurnFight_ExchangeSummary_ExchangeValue(pdamage, ran);
            //enemy.enemyUI.TurnFight_ExchangeSummary_AttackDamage(edamage, ran);
            Player_UI.instance.TurnFight_ExchangeSummary_ExchangeValue(pdamage, 0);
            enemy.enemyUI.TurnFight_ExchangeSummary_AttackDamage(edamage, 0);

            // ����
            Player_Manager.instnace.Turn_Sound_Call(2);

            // ��� ��� (UI ����Ʈ)
            // yield return new WaitForSeconds(ran);

            // �� ī�޶� ��鸲 -> �� ���̿� ���� �ٸ��� �ൿ
            Turn_ExchangeCamreaEffect(Mathf.Abs(pdamage - edamage));

            Player_UI.instance.TurnFight_Ouutline_SpeedUp();  // �ƿ����� ����Ʈ

            // �� ���
            if (pdamage > edamage)
            {
                // �÷��̾� �¸�
                Player_Manager.instnace.player_Turn.Turn_ExchangeResuitAnim(Player_Turn.ExchangeResuit.Win);
                enemy.Turn_ExchangeResuitAnim(Enemy_Base.ExchangeResuit.Lose);
                enemyCount++;
            }
            else if (pdamage == edamage)
            {
                // ���º�
                Player_Manager.instnace.player_Turn.Turn_ExchangeResuitAnim(Player_Turn.ExchangeResuit.Draw);
                enemy.Turn_ExchangeResuitAnim(Enemy_Base.ExchangeResuit.Draw);
            }
            else if (pdamage <= edamage)
            {
                // �ֳʹ� �¸�
                Player_Manager.instnace.player_Turn.Turn_ExchangeResuitAnim(Player_Turn.ExchangeResuit.Lose);
                enemy.Turn_ExchangeResuitAnim(Enemy_Base.ExchangeResuit.Win);
                playerCount++;
            }

            // �и� �̵� & �ִϸ��̼� ���
            while (Player_Manager.instnace.player_Turn.isRecoilMove || enemy.isRecoilMove)
            {
                yield return null;
            }

            // ���� �� ������
            // yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
        }

        // UI Off -> �̰� ������ ���⼭ ���µ�, ������ ���� �� ������ ���°� ����!
        Player_UI.instance.TurnFight_ExchanageSummary(false, null);
        enemy.enemyUI.TurnFight_ExchanageSummary(false, null);


        // ���� ���ݱ��� ������
        // yield return new WaitForSeconds(Random.Range(0.1f, 0.15f));

        isExchange = false;

        // ���� ���� Ƚ�� üũ
        int count;
        // ���ʹ� ���� ȣ��
        if (playerCount == playerSlot.myAttack.damageValue.Length)
        {
            count = enemySlot.myAttack.attackCount - enemyCount;
            StartCoroutine(Turn_AttackCall(Object.Enemy, enemySlot, true, count));
        }

        // �÷��̾� ���� ȣ��
        if (enemyCount == enemySlot.myAttack.damageValue.Length)
        {
            count = playerSlot.myAttack.attackCount - playerCount;
            StartCoroutine(Turn_AttackCall(Object.Player, playerSlot, true, count));
        }
    }


    // ��ü �� & ���� ���� �� ����
    private void Turn_Return(Exchange exchange)
    {
        // ���� ��ġ�� ����
        switch (exchange.attackType)
        {
            case Exchange.AttackType.Oneside:
                switch (exchange.owner)
                {
                    case Exchange.AttackOwner.Player:
                        Player_Manager.instnace.player_Turn.Turn_ExchangeEndMove(playerReturnPos, 1.5f);
                        break;

                    case Exchange.AttackOwner.Enemy:
                        int index = enemys.IndexOf(enemy);
                        enemy.Turn_ExchangeEndMove(enemyReturnPos[index], 1.5f);
                        break;
                }
                break;

            case Exchange.AttackType.Exchange:
                if (!Player_Manager.instnace.isDie)
                {
                    Player_Manager.instnace.player_Turn.Turn_ExchangeEndMove(playerReturnPos, 1.5f);
                }

                if (!enemy.isDie)
                {
                    int index = enemys.IndexOf(enemy);
                    enemy.Turn_ExchangeEndMove(enemyReturnPos[index], 1.5f);
                }
                break;
        }
    }


    /// <summary>
    /// �� ���� �� ���� ���̿� ���� ȭ�� ��鸲�� �ٸ��� ȣ���ϴ� ���
    /// </summary>
    /// <param name="value">�÷��̾� & ���� ���ݷ� ���� ���� ����</param>
    private void Turn_ExchangeCamreaEffect(int value)
    {
        // ���� ���� ���� ȭ�� ��鸲 ���� �ٸ��� ȣ��
        if (value <= 20)
        {
            Camera_Manager.instance.Turn_CameraShack(7f, 0.025f);
        }
        else if(value <= 40)
        {
            Camera_Manager.instance.Turn_CameraShack(12f, 0.025f);
        }
        else
        {
            Camera_Manager.instance.Turn_CameraShack(15f, 0.025f);
        }
    }


    // �� �¸� & �Ϲ���� ȣ��
    /// <summary>
    /// ���� �� ȣ���ϴ� �ڷ�ƾ
    /// </summary>
    /// <param name="win">�÷��̾� & ���ʹ� �� ���� �¸��ߴ°�</param>
    /// <param name="slot">���� �����͸� ���� ����</param>
    /// <param name="isExchange">�� �������� �ƴ��� Ȯ�� (�հ����̸� �ʻ�� �������� ȸ����)</param>
    /// <param name="attackCount">���� ���� Ƚ��</param>
    private IEnumerator Turn_AttackCall(Object win, Attack_Slot slot, bool isExchange, int attackCount)
    {
        isAttack = true;

        // �÷��̾� & ���ʹ� ���� ȣ��
        switch (win)
        {
            case Object.Player:
                if (slot.myAttack.isAOE)
                {
                    Player_Manager.instnace.player_Turn.Turn_AttackData_Setting(enemyObjects, slot.myAttack, isExchange, attackCount);
                }
                else
                {
                    Player_Manager.instnace.player_Turn.Turn_AttackData_Setting(enemy.gameObject, slot.myAttack, isExchange, attackCount);
                }
                break;

            case Object.Enemy:
                enemy.Turn_AttackData_Setting(Player_Manager.instnace.player_Turn.gameObject, slot.myAttack, isExchange, attackCount);
                break;
        }

        // ���� ���
        while (Player_Manager.instnace.isAttack || enemy.isAttack)
        {
            yield return null;
        }

        isAttack = false;
    }

    // ���ʹ� ����Ʈ üũ
    private void Enemy_Check()
    {
        for (int i = 0; i < enemys.Count; i++)
        {
            if (enemys[i].isDie)
            {
                // ���� ���� ����
                for (int j = 0; j < enemys[i].attack_Slots.Count; j++)
                {
                    enemySlots.Remove(enemys[i].attack_Slots[j]);
                }

                // ���ʹ� ����
                enemyObjects.Remove(enemys[i].gameObject);
                enemys.Remove(enemys[i]);

                // ī��Ʈ
                enemyCount = enemys.Count;
            }
        }
    }

    // ����Ʈ �ʱ�ȭ
    private void Turn_ListReset()
    {
        enemyExchangeList.Clear();
        exchangeList.Clear();
    }


    #region ���� �¸� & �й�

    // ���� ���� üũ
    private void Turn_StageCheck()
    {
        Debug.Log("���� üũ ȣ��");
        // �÷��̾� ��� üũ
        if (Player_Manager.instnace.isDie)
        {
            // ���� �й� ȣ��
            Turn_FightLose();
            return;
        }

        // ���ʹ� ��� üũ
        for (int i = 0; i < enemys.Count; i++)
        {
            // ���� ������ ���Ͱ� �ִٸ� ���� ����
            if (!enemys[i].isDie)
            {
                Debug.Log("�� �������");
                return;
            }
        }

        // ���� �¸� ȣ��
        switch (roomType)
        {
            case RoomType.Normal:
                Debug.Log("���� �¸� �븻 ȣ��");
                Turn_ight_NormalWin();
                break;

            case RoomType.Boss:
                Debug.Log("���� �¸� ���� ȣ��");
                TurnFthgt_BossWin();
                break;
        }
    }


    // �Ϲ����� �¸� ȣ��
    private void Turn_ight_NormalWin()
    {
        StopAllCoroutines();
        StartCoroutine(TurnFightNormalEndCall());
    }

    // �Ϲ����� �¸� ����
    private IEnumerator TurnFightNormalEndCall() // �̰� �ʻ��� ȣ��Ǹ� ���� �ִϸ��̼� �ȳ���... Ʈ���� üũ �ȵǴµ�
    {
        curTurn = Turn.End;
        isEnd = true;

        // UI �ʱ�ȭ
        Player_UI.instance.TurnFight_ExchanageSummary(false, null);
        Player_UI.instance.TurnFight_Outline_Setting(Player_UI.Outline.TypeA, false);
        Player_UI.instance.TurnFight_Outline_Setting(Player_UI.Outline.TypeB, false);

        // ü�¹� Ȱ��ȭ
        Player_UI.instance.hpSet.SetActive(true);

        // Ŀ�� ���
        Player_Manager.instnace.Cursor_Setting(true);

        // ����
        Player_Manager.instnace.UI_Sound_Call(5);

        // ��ų ����Ʈ UI
        Player_Skill_Manager.instance.Add_SkillPoint(skillPoint);

        // ���� ���� �ִϸ��̼� ȣ��
        Player_Manager.instnace.player_Turn.Turn_End(Player_Turn.EndType.Win);
        while (Player_Manager.instnace.player_Turn.isEndAnim)
        {
            yield return null;
        }

        // ���� ���� UI ȣ��
        Player_UI.instance.TurnFight_Win();
        while (Player_UI.instance.isFade)
        {
            yield return null;
        }

        // ����
        Stage_Manager.instance.BGM_Setting(0);

        // ���̾�α� ȣ��
        if (haveEndDialog)
        {
            Dialog_Manager.instance.Dialog(endDialogIndex);
        }

        // Ʃ�丮��
        if (haveEndTutorial)
        {
            Tutorial_Manager.instance.TutorialOn(tutorialIndex[1]);
        }

        // �Ŵ��� �ı�
        Destroy(gameObject);
    }


    // �������� �¸� ȣ��
    private void TurnFthgt_BossWin()
    {
        StopAllCoroutines();
        StartCoroutine(TurnFightBossEndCall());
    }

    // �������� �¸� ����
    private IEnumerator TurnFightBossEndCall()
    {
        curTurn = Turn.End;
        isEnd = true;

        // UI �ʱ�ȭ
        Player_UI.instance.Turn_FightSelect(false);
        Player_UI.instance.TurnFight_Outline_Setting(Player_UI.Outline.TypeA, false);
        Player_UI.instance.TurnFight_Outline_Setting(Player_UI.Outline.TypeB, false);

        // ���� ���� üũ
        while (enemys[0].isCutscene)
        {
            yield return null;
        }

        // ���̾�α� ȣ��
        if (haveEndDialog)
        {
            Dialog_Manager.instance.Dialog(endDialogIndex);
        }

        // �÷��̾� �ı�
        Destroy(Player_Manager.instnace.gameObject);

        // ���� ���� �� ȣ��
        Scene_Loading.LoadScene("Ending_Scene");
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
        isEnd = true;

        // ��� ���� Off
        Stage_Manager.instance.BGM_OnOff(false);

        // UI ����
        Player_Manager.instnace.UI_Sound_Call(6);

        // UI �ʱ�ȭ
        Player_UI.instance.TurnFight_Outline_Setting(Player_UI.Outline.TypeA, false);
        Player_UI.instance.TurnFight_Outline_Setting(Player_UI.Outline.TypeB, false);

        // ���� ���� �ִϸ��̼� ȣ��
        Player_Manager.instnace.player_Turn.Turn_End(Player_Turn.EndType.Lose);
        while (Player_Manager.instnace.player_Turn.isEndAnim)
        {
            yield return null;
        }

        // ���� ���� UI ȣ��
        Player_UI.instance.TurnFight_Lose();

        // ������ üũ����Ʈ���� �ٽ� ����
    } // -> �̿ϼ�
    #endregion
}
