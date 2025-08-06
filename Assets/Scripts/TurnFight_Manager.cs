using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 일방 / 합 데이터를 받아서 리스트화
[System.Serializable]
public class Exchange
{
    public enum AttackType { Oneside, Exchange }
    public enum AttackOwner { Player, Enemy }

    public AttackOwner owner; // 공격의 주체
    public AttackType attackType; // 합 & 일방 공격 체크
    public Attack_Slot player_Slot; // 플레이어 슬롯
    public Attack_Slot enemy_Slot; // 에너미 슬롯
    public int attackSpeed; // 플레이어 공격 속도


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
    private bool isFrist = true; // 이거 맨 처음 전투 시작할 때 플레이어 UI 부분 1회용 체크 불값임!
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
    public List<GameObject> enemyObjects; // 에너미 오브젝트 전체
    public List<Enemy_Base> enemys;
    [SerializeField] private int enemyCount;


    [Header("=== Attack Setting===")]
    [SerializeField] private List<Exchange> enemyExchangeList; // 에너미 공격 리스트
    [SerializeField] private List<Exchange> exchangeList; // 전체 공격 리스트
    [SerializeField] private List<Attack_Slot> enemySlots; // 에너미 슬롯 데이터
    [SerializeField] private Attack_Slot playerSlot;
    [SerializeField] private Attack_Slot enemySlot;
    [SerializeField] private Enemy_Base enemy;


    // 1. 페이드 연출 호출

    // 2. 전투 필드로 플레이어 이동

    // 3. 페이드 연출 종료 후 플레이어 시작 모션 & 몬스터 활성화

    // 4. 플레이어 스킬 선택

    // 5. 선택 UI 종료

    // 6. 플레이어 - 몬스터 이동

    // 7. 플레이어 - 몬스터 합

    // 8. 플레이어 - 몬스터 원위치

    // 이하 4 ~ 8 반복

    public void Exchange_Setting_Add2(Attack_Slot player, Attack_Slot enemy)
    {
        // 리스트에 들어갈 새로운 공격 생성
        Exchange newExchange = new Exchange();
        newExchange.owner = Exchange.AttackOwner.Player;
        newExchange.player_Slot = player;
        newExchange.enemy_Slot = enemy;
        newExchange.attackType = newExchange.player_Slot.slotSpeed >= newExchange.enemy_Slot.slotSpeed ? Exchange.AttackType.Exchange : Exchange.AttackType.Oneside;
        newExchange.attackSpeed = player.slotSpeed;

        // 공격하려는 슬롯이 나를 노린다면
        if (enemy.targetSlot == player)
        {
            // 합 상태 전환
            newExchange.Slot_AttackType_Setting(Exchange.AttackType.Exchange);
            newExchange.Slot_AttackTarget_Setting(Exchange.AttackOwner.Player);
            exchangeList.Add(newExchange);
            player.myExchange = newExchange;
            return;
        }

        // 속도가 더 빨라서 합이 가능하다면
        if (player.slotSpeed >= enemy.slotSpeed)
        {
            // 해당 공격과 합 하고 있는 공격이 있는지 체크
            for (int i = 0; i < exchangeList.Count; i++)
            {
                // 있으면 일방 공격으로 전환
                if (exchangeList[i].owner == Exchange.AttackOwner.Player && exchangeList[i].attackType == Exchange.AttackType.Exchange && exchangeList[i].enemy_Slot == newExchange.enemy_Slot)
                {
                    exchangeList[i].Slot_AttackType_Setting(Exchange.AttackType.Oneside);
                }
            }

            // 합 상태 전환
            newExchange.Slot_AttackType_Setting(Exchange.AttackType.Exchange);
            newExchange.Slot_AttackTarget_Setting(Exchange.AttackOwner.Player);
            exchangeList.Add(newExchange);
            player.myExchange = newExchange;
            return;
        }
        // 속도가 느려서 합이 불가능하다면
        else
        {
            newExchange.Slot_AttackType_Setting(Exchange.AttackType.Oneside);
            newExchange.Slot_AttackTarget_Setting(Exchange.AttackOwner.Player);
            exchangeList.Add(newExchange);

            // 공격 설정
            player.myExchange = newExchange;
            return;
        }
    }

    // 플레이어 공격 설정 -> 구조체
    public void Exchange_Setting_Add(Attack_Slot player, Attack_Slot enemy)
    {
        // 리스트에 들어갈 새로운 공격 생성
        Exchange newExchange = new Exchange();
        newExchange.owner = Exchange.AttackOwner.Player;
        newExchange.player_Slot = player;
        newExchange.enemy_Slot = enemy;
        newExchange.attackType = newExchange.player_Slot.slotSpeed >= newExchange.enemy_Slot.slotSpeed ? Exchange.AttackType.Exchange : Exchange.AttackType.Oneside;
        newExchange.attackSpeed = player.slotSpeed;

        // 리스트 체크 -> 여기부터 작업
        for (int i = 0; i < exchangeList.Count; i++)
        {
            // 플레이어의 공격 중 이미 해당 슬롯을 공격하는 공격이 있는지 체크
            if (exchangeList[i].owner == Exchange.AttackOwner.Player && exchangeList[i].attackType == Exchange.AttackType.Exchange && exchangeList[i].enemy_Slot == newExchange.enemy_Slot)
            {
                // 속도가 더 빠른쪽이 합 & 느린쪽은 일방으로 변경
                /*
                if (newExchange.attackSpeed >= exchangeList[i].attackSpeed)
                {
                    exchangeList[i].Slot_AttackType_Setting(Exchange.AttackType.Oneside);

                    newExchange.Slot_AttackType_Setting(Exchange.AttackType.Exchange);
                    newExchange.Slot_AttackTarget_Setting(Exchange.AttackOwner.Player);
                    exchangeList.Add(newExchange);

                    // 공격 설정
                    player.myExchange = newExchange;
                    return;
                }
                else
                {
                    newExchange.Slot_AttackType_Setting(Exchange.AttackType.Oneside);
                    newExchange.Slot_AttackTarget_Setting(Exchange.AttackOwner.Player);
                    exchangeList.Add(newExchange);

                    // 공격 설정
                    player.myExchange = newExchange;
                    return;
                }
                */
                exchangeList[i].Slot_AttackType_Setting(Exchange.AttackType.Oneside);

                newExchange.Slot_AttackType_Setting(Exchange.AttackType.Exchange);
                newExchange.Slot_AttackTarget_Setting(Exchange.AttackOwner.Player);
                exchangeList.Add(newExchange);

                // 공격 설정
                player.myExchange = newExchange;
                return;
            }

            // 에너미의 공격 중 이미 내 슬롯을 공격하는 공격이 있는지 체크
            if (exchangeList[i].owner == Exchange.AttackOwner.Enemy && exchangeList[i].enemy_Slot == newExchange.enemy_Slot)
            {
                // 지정한 슬롯이 이미 내 슬롯을 공격한다면
                /*
                if (newExchange.attackSpeed >= exchangeList[i].attackSpeed)
                {
                    // 속도가 더 빠르다면 해당 공격을 제거 & 합 공격으로 공격 추가
                    exchangeList.Remove(exchangeList[i]);

                    newExchange.Slot_AttackType_Setting(Exchange.AttackType.Exchange);
                    newExchange.Slot_AttackTarget_Setting(Exchange.AttackOwner.Player);
                    exchangeList.Add(newExchange);

                    // 공격 설정
                    player.myExchange = newExchange;
                    return;
                }
                else
                {
                    // 더 느리다면 일방공격으로 추가
                    newExchange.Slot_AttackType_Setting(Exchange.AttackType.Oneside);
                    newExchange.Slot_AttackTarget_Setting(Exchange.AttackOwner.Player);
                    exchangeList.Add(newExchange);

                    // 공격 설정
                    player.myExchange = newExchange;
                    return;
                }
                */
                // 속도가 더 빠르다면 해당 공격을 제거 & 합 공격으로 공격 추가
                exchangeList.Remove(exchangeList[i]);

                newExchange.Slot_AttackType_Setting(Exchange.AttackType.Exchange);
                newExchange.Slot_AttackTarget_Setting(Exchange.AttackOwner.Player);
                exchangeList.Add(newExchange);

                // 공격 설정
                player.myExchange = newExchange;
                return;
            }
        }

        // 선행 공격이 존재하지 않는 슬롯이라면
        exchangeList.Add(newExchange);
    }


    // 플레이어 공격 데이터 삭제 -> 구조체 -> 이거 가끔 에너미의 구조체 공격 마지막 부분에서 exchange로 변경되는 이슈있음...
    public void Exchange_Setting_Remove(Attack_Slot player, Attack_Slot enemy, Exchange exchange)
    {
        // 이거 추가한 순서대로 제거하면 인덱스에 문제가 없는데, 그게 아니라면 인덱스가 변경되니까 문제가 발생함!
        if (exchange == null)
        {
            return;
        }

        // 공격 버튼 비활성화
        Player_UI.instance.Turn_AttackButton(false);


        // 코스트 반환
        Player_Manager.instnace.AttackPointAdd(exchange.player_Slot.myAttack.attackCost);


        // 공격 타입 체크 (일방 / 합)
        switch (exchange.attackType)
        {
            // 합 공격이 아니였다면
            case Exchange.AttackType.Oneside:

                exchangeList.Remove(exchange);
                player.ResetSlot();
                break;

            case Exchange.AttackType.Exchange:
                // 에너미 슬롯이 원래 공격하던 슬롯이 있었는지 체크
                for (int i = 0; i < enemyExchangeList.Count; i++)
                {
                    if (enemyExchangeList[i].enemy_Slot == exchange.enemy_Slot)
                    {
                        // 원래 공격 추가
                        Exchange newAttack = enemyExchangeList[i];
                        newAttack.owner = Exchange.AttackOwner.Enemy;
                        newAttack.Slot_AttackType_Setting(Exchange.AttackType.Oneside);

                        // 공격 삭제
                        exchangeList.Remove(exchange);
                        player.ResetSlot();

                        exchangeList.Add(newAttack);
                        return;
                    }
                }
                break;

        }
    }


    // 에너미의 공격 설정 -> 구조체
    public void Exchange_Setting_Enemy(Attack_Slot enemy, Attack_Slot player)
    {
        // 데이터 셋팅
        Exchange data = new Exchange();
        data.owner = Exchange.AttackOwner.Enemy;
        data.attackType = Exchange.AttackType.Oneside;
        data.player_Slot = player;
        data.enemy_Slot = enemy;
        data.attackSpeed = enemy.slotSpeed;

        // 리스트에 추가
        enemyExchangeList.Add(data);

        // 리스트 속도 순 정렬
        enemyExchangeList.Sort((a, b) => b.attackSpeed.CompareTo(a.attackSpeed));

        // 데이터를 전체 공격 리스트에 추가
        exchangeList.Add(data);
    }


    public void Phase2()
    {
        phase2 = true;
    }


    // 시작 연출 호출 (전투 진입 시 1회 호출)
    public void TurnFight_Start()
    {
        StartCoroutine(TurnFight_StartCall());
    }


    // 시작 연출 동작 - 노말 (전투 진입 시 1회 호출)
    private IEnumerator TurnFight_StartCall()
    {
        curTurn = Turn.Start;

        // 다이얼로그 종료
        Dialog_Manager.instance.DialogOff();

        // 튜토리얼 종료
        if(Tutorial_Manager.instance != null)
        {
            Tutorial_Manager.instance.Tutorial_Reset();
        }

        // 사운드 Off
        Stage_Manager.instance.BGM_OnOff(false);

        // 월드 UI Off
        Player_UI.instance.World_UISetting(false);

        // 커서 셋팅
        Player_Manager.instnace.Cursor_Setting(false);

        // 1. 페이드 연출 호출
        Player_UI.instance.TurnFight_Fade(roomType != RoomType.Normal);

       // 페이드 인 될때까지 대기
        yield return new WaitForSeconds(0.75f);

        // 웨이포인트 종료
        Waypoint_Manager.instance.Waypoint_Setting(false, 0);

        // 몬스터 활성화
        for (int i = 0; i < enemys.Count; i++)
        {
            enemys[i].turnManager = this;
            enemys[i].transform.position = enemyReturnPos[i].position;
            enemys[i].gameObject.SetActive(true);
        }

        // 사운드 On
        Stage_Manager.instance.BGM_Setting(1);

        // 2. 전투 필드로 플레이어 이동
        Player_Manager.instnace.Turn_Fight_Start(playerReturnPos, this);

        // 플레이어 맵 중앙 바라보게 하기
        Player_Manager.instnace.player_Turn.LookAt(exchangeHolderPos.gameObject);

        // 페이드 연출 종료 대기
        while (Player_UI.instance.isFade)
        {
            yield return null;
        }

        // 보스전 - 컷신 대기
        if (roomType == RoomType.Boss)
        {
            while (enemys[0].isCutscene)
            {
                yield return null;
            }
        }

        // 3. 플레이어 & 몬스터 시작 모션 + 맵 중앙 바라보기
        Player_Manager.instnace.player_Turn.Turn_StartAnim();
        for (int i = 0; i < enemys.Count; i++)
        {
            enemys[i].Spawn();
            enemys[i].LookAt(Player_Manager.instnace.player_Turn.gameObject);
        }

        // 에너미 슬롯 셋팅 - UI 활성화
        for (int i = 0; i < enemys.Count; i++)
        {
            enemys[i].enemyUI.UI_Setting(true);
            for (int i2 = 0; i2 < enemys[i].attack_Slots.Count; i2++)
            {
                enemySlots.Add(enemys[i].attack_Slots[i2]);
            }
        }

        // 시작 애니메이션 대기
        while (Player_Manager.instnace.player_Turn.isSpawnAnim)
        {
            yield return null;
        }

        // 전투 시작 UI
        Player_UI.instance.Turn_StartUI();

        // 다이얼로그 호출
        if (haveStartDialog)
        {
            Dialog_Manager.instance.Dialog(startDialogIndex);
        }

        // 선택 UI On
        Player_UI.instance.Turn_UndersideGaugeSetting();
        Player_Manager.instnace.UnderSideButtonOn();
        Player_UI.instance.Turn_FightSelect(true);

        // 4. 플레이어 셋팅
        Player_Manager.instnace.player_Turn.forwardTarget = exchangeHolderPos.gameObject;
        Player_Manager.instnace.player_Turn.Turn_AttackSelect();
        Player_Manager.instnace.player_Turn.Slot_SpeedSetting();

        // 튜토리얼
        if (haveStartTutorial)
        {
            Tutorial_Manager.instance.Tutorial_Fight();
            //Tutorial_Manager.instance.TutorialOn(tutorialIndex[0]);
        }

        // 페이드 아웃 후 일시 대기
        yield return new WaitForSeconds(0.25f);

        // 4. 플레이어 스킬 선택
        StartCoroutine(Turn_Select());
    }


    // 플레이어 스킬 선택
    private IEnumerator Turn_Select()
    {
        curTurn = Turn.Select;

        // 2페이즈 호출
        if (phase2)
        {
            phase2 = false;
            enemys[0].Phase2();
        }

        // 스테이지 체크
        Turn_StageCheck();
        if (isEnd)
        {
            yield break;
        }

        // 턴 UI
        if(!isFrist)
        {
            turnCount++;
            Player_UI.instance.Turn_TurnCountUI(turnCount);
        }

        // 리스트 초기화
        Turn_ListReset();

        // 에너미 체크
        Enemy_Check();

        // 공격 버튼 비활성화
        Player_UI.instance.Turn_AttackButton(false);

        // 자원 회복
        Player_Manager.instnace.Turn_AttackPointRecovery();

        // 사운드
        Player_Manager.instnace.UI_Sound_Call(4);

        // 에너미 공격 셋팅 & UI On
        for (int i = 0; i < enemys.Count; i++)
        {
            if (!enemys[i].isDie)
            {
                enemys[i].Turn_AttackSetting(gameObject);
                enemys[i].enemyUI.Turn_FightSelect(true);
                enemys[i].LookAt(Player_Manager.instnace.player_Turn.gameObject);
            }
        }

        // 맨 처음 턴 시작에는 여기가 아니라 위에서 UI 활성화시킴!
        if (isFrist)
        {
            isFrist = false;
        }
        else
        {
            // 선택 UI On
            Player_UI.instance.Turn_UndersideGaugeSetting();
            Player_Manager.instnace.UnderSideButtonOn();
            Player_UI.instance.Turn_FightSelect(true);
            Player_UI.instance.hpSet.SetActive(true);

            // 4. 플레이어 스피드 & 슬롯 셋팅
            Player_Manager.instnace.player_Turn.Turn_AttackSelect();
            Player_Manager.instnace.player_Turn.Slot_SpeedSetting();
        }


        // 선택 완료까지 대기
        while (Player_Manager.instnace.player_Turn.isSelect)
        {
            yield return null;
        }

        // 선택 UI 종료
        Player_UI.instance.Turn_FightSelect(false);

        // 에너미 UI Off
        for (int i = 0; i < enemys.Count; i++)
        {
            enemys[i].enemyUI.Turn_FightSelect(false);
        }

        // 전투 시작
        StartCoroutine(Turn_Fight());
    }


    // 플레이어가 자신의 공격을 선택하고, 어떤 슬롯에 공격을 날릴지 정하기 위해 데이터 전달
    public List<Attack_Slot> GetEnemyAttackSlot()
    {
        List<Attack_Slot> enemyAttackList = new List<Attack_Slot>();

        // 모든 몬스터 공격 슬롯 순회 -> 처음부터 마지막까지 공격 순서 및 공격 받아오기
        for (int i = 0; i < enemys.Count; i++)
        {
            // 혹시 모를 리스트 null 체크
            if (enemys[i] != null)
            {
                // 해당 몬스터의 공격 슬롯을 리스트에 담기
                for (int i2 = 0; i2 < enemys[i].attack_Slots.Count; i2++)
                {
                    enemyAttackList.Add(enemys[i].attack_Slots[i2]);
                }
            }
        }

        // 데이터 전달
        return enemyAttackList;
    }


    // 플레이어 필살기 사용 시 턴 초기화 기능 호출-> 초기화 기능만 있으면 됨!
    public void Turn_UnderSideAttack()
    {
        StartCoroutine(Turn_UnderSideAttackCall());
    }

    private IEnumerator Turn_UnderSideAttackCall()
    {
        isAttack = true;

        // 게이지 초기화
        Player_Manager.instnace.UnderSideGaugeAdd(-999);

        // UI 비활성화
        Player_UI.instance.Turn_FightSelect(false);

        // 가장자리 UI 활성화
        int ran = Random.Range(0, 2);

        // 공격 리스트 전체 초기화
        exchangeList.Clear();
        enemyExchangeList.Clear();

        // 플레이어 공격 슬롯 초기화
        for (int i = 0; i < Player_Manager.instnace.player_Turn.attackSlot.Count; i++)
        {
            if (Player_Manager.instnace.player_Turn.attackSlot[i].haveAttack)
            {
                Player_Manager.instnace.AttackPointAdd(Player_Manager.instnace.player_Turn.attackSlot[i].myAttack.attackCost);
                Player_Manager.instnace.player_Turn.attackSlot[i].ResetSlot();
            }
        }

        // 공격 종료까지 대기
        yield return new WaitForSeconds(0.5f);
        while (Player_Manager.instnace.isAttack)
        {
            yield return null;

        }

        // 8. 플레이어 원위치
        Player_Manager.instnace.player_Turn.Turn_ExchangeEndMove(playerReturnPos, 1.5f);
        while (Player_Manager.instnace.player_Turn.isExchangeMove)
        {
            yield return null;
        }

        // 가장자리 UI 비활성화
        Player_UI.instance.TurnFight_Outline_Setting(ran == 0 ? Player_UI.Outline.TypeA : Player_UI.Outline.TypeB, false);

        // 데미지 UI 종료
        Player_UI.instance.TurnFight_ExchanageSummary(false, null);

        // 플레이어의 공격 타겟 데이터 초기화
        Player_Manager.instnace.player_Turn.Turn_AttackData_Reset();

        yield return new WaitForSeconds(0.1f); // 버그 해결 방법 -> 바로 애니메이션을 호출해서 생긴 문제인듯 / 0.1초 대기시간 줌


        // 스테이지 종료 체크
        Turn_StageCheck();
        if(!isEnd)
        {
            // 9. 플레이어 선택 턴 시작
            isAttack = false;
            StartCoroutine(Turn_Select());
        }
    }


    // 전투 기능 동작 ( 5, 6, 7, 8 )
    private IEnumerator Turn_Fight()
    {
        curTurn = Turn.Fight;

        // 사운드
        Player_Manager.instnace.UI_Sound_Call(3);

        // 리스트 속도 순으로 정리
        exchangeList.Sort((a, b) => b.attackSpeed.CompareTo(a.attackSpeed));

        // UI Off
        Player_UI.instance.Turn_FightSelect(false);
        Player_UI.instance.hpSet.SetActive(false);

        // 가장자리 UI 활성화
        int ran = Random.Range(0, 2);
        Player_UI.instance.TurnFight_Outline_Setting(ran == 0 ? Player_UI.Outline.TypeA : Player_UI.Outline.TypeB, true);

        // 합 리스트 만큼 공격 진행
        for (int i = 0; i < exchangeList.Count; i++)
        {
            // 스테이지 종료 체크
            Turn_StageCheck();
            if (isEnd)
            {
                yield break;
            }

            // 타겟의 사망 체크
            if (!exchangeList[i].enemy_Slot.slotOwner.GetComponent<Enemy_Base>().isDie)
            {
                // 플레이어의 공격 타겟 데이터 초기화
                Player_Manager.instnace.player_Turn.Turn_AttackData_Reset();

                // 플레이어 & 몬스터 슬롯 셋팅
                Turn_Slot_Setting(i);

                // 카메라 이펙트
                Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.ExchangeA, 0.35f);

                // 합 이동 호출 && 이동 대기
                Turn_ExchangeMove(exchangeList[i]);
                while (isExchangeMove)
                {
                    yield return null;
                }

                // 일방 공격 or 합 동작 호출
                Turn_Exchange(exchangeList[i]);
                while (isAttack || isExchange)
                {
                    yield return null;
                }

                // UI 종료
                Player_UI.instance.TurnFight_ExchanageSummary(false, null);
                enemy.enemyUI.TurnFight_ExchanageSummary(false, null);

                // 카메라 변경
                Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.None, 0.75f);

                // 원래 위치로 복귀
                Turn_Return(exchangeList[i]);
                while (Player_Manager.instnace.player_Turn.isExchangeMove || enemy.isExchangeMove)
                {
                    yield return null;
                }


                // 다음 공격까지의 잠시 대기
                // yield return new WaitForSeconds(nextAttackDelay);
            }
        }

        // 가장자리 UI 비활성화
        Player_UI.instance.TurnFight_Outline_Setting(ran == 0 ? Player_UI.Outline.TypeA : Player_UI.Outline.TypeB, false);

        // 스테이지 종료 체크
        Turn_StageCheck();

        // 9. 플레이어 선택 턴 시작
        StartCoroutine(Turn_Select());
    }


    // 플레이어 & 몬스터 슬롯 셋팅
    private void Turn_Slot_Setting(int index)
    {
        // 플레이어 & 몬스터 슬롯 셋팅
        playerSlot = exchangeList[index].player_Slot;
        enemySlot = exchangeList[index].enemy_Slot;
        enemy = enemySlot.slotOwner.GetComponent<Enemy_Base>();
    }

    // 합 이동 호출 부분
    private void Turn_ExchangeMove(Exchange exchange)
    {
        switch (exchange.attackType)
        {
            case Exchange.AttackType.Oneside:
                break;

            case Exchange.AttackType.Exchange:
                if (exchange.owner == Exchange.AttackOwner.Enemy)
                {
                    StartCoroutine(Turn_AttackCall(Object.Enemy, enemySlot, false, enemySlot.myAttack.damageValue.Length)); // -> 버그 대비 기능임...
                }
                else
                {
                    StartCoroutine(Turn_ExchangeMove_Exchange());
                }
                break;
        }
    }


    // 합 공격 이동
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


    // 합 동작 or 일방공격 호출
    private void Turn_Exchange(Exchange exchange)
    {
        switch (exchange.attackType)
        {
            // 일방 공격
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

            // 합 동작
            case Exchange.AttackType.Exchange:
                StartCoroutine(Turn_ExchangeCall(exchange));
                break;
        }
    }


    // 합 동작
    private IEnumerator Turn_ExchangeCall(Exchange exchange)
    {
        isExchange = true;
        Player_UI.instance.TurnFight_ExchanageSummary(true, playerSlot.myAttack);

        // 합 시작
        // 플레이어 또는 몬스터의 공격 횟수가 전부 소진될때가지 반복
        int playerCount = 0;
        int enemyCount = 0;
        bool loop = false;
        while (playerCount < playerSlot.myAttack.damageValue.Length && enemyCount < enemySlot.myAttack.damageValue.Length)
        {
            // 첫번째 루프 이후
            if (loop)
            {
                // 합 위치 복귀 이동 & 애니메이션
                float moveSpeed = Random.Range(1.5f, 1.7f);
                Player_Manager.instnace.player_Turn.Turn_ExchangeMove(playerExchanagePos, moveSpeed);
                enemy.Turn_ExchangeMove(enemyExchanagePos, moveSpeed);

                // 합 위치 복귀 이동 & 애니메이션 대기
                while (Player_Manager.instnace.player_Turn.isExchangeMove || enemy.isExchangeMove)
                {
                    yield return null;
                }
            }
            else
            {
                loop = true;
            }

            // 플레이어 공격 이동 UI
            Player_UI.instance.TurnFight_ExchanageSummary(true, playerSlot.myAttack);
            Player_UI.instance.TurnFight_ExchangeSummary_AttackCount(playerCount);

            // 에너미 공격 이동 UI
            enemy.enemyUI.TurnFight_ExchanageSummary(true, enemySlot.myAttack);
            enemy.enemyUI.TurnFight_ExchangeSummary_AttackCount(enemyCount);


            // 데미지 계산
            (int pdamage, bool isPC) = Player_Manager.instnace.DamageCal(playerSlot.myAttack, playerCount);
            (int edamage, bool isCC) = enemy.DamageCal(enemySlot.myAttack, enemyCount);


            // 데미지 UI
            //float ran = Random.Range(0.1f, 0.15f);
            //Player_UI.instance.TurnFight_ExchangeSummary_ExchangeValue(pdamage, ran);
            //enemy.enemyUI.TurnFight_ExchangeSummary_AttackDamage(edamage, ran);
            Player_UI.instance.TurnFight_ExchangeSummary_ExchangeValue(pdamage, 0);
            enemy.enemyUI.TurnFight_ExchangeSummary_AttackDamage(edamage, 0);

            // 사운드
            Player_Manager.instnace.Turn_Sound_Call(2);

            // 잠시 대기 (UI 이펙트)
            // yield return new WaitForSeconds(ran);

            // 합 카메라 흔들림 -> 값 차이에 따라 다르게 행동
            Turn_ExchangeCamreaEffect(Mathf.Abs(pdamage - edamage));

            Player_UI.instance.TurnFight_Ouutline_SpeedUp();  // 아웃라인 이펙트

            // 합 결과
            if (pdamage > edamage)
            {
                // 플레이어 승리
                Player_Manager.instnace.player_Turn.Turn_ExchangeResuitAnim(Player_Turn.ExchangeResuit.Win);
                enemy.Turn_ExchangeResuitAnim(Enemy_Base.ExchangeResuit.Lose);
                enemyCount++;
            }
            else if (pdamage == edamage)
            {
                // 무승부
                Player_Manager.instnace.player_Turn.Turn_ExchangeResuitAnim(Player_Turn.ExchangeResuit.Draw);
                enemy.Turn_ExchangeResuitAnim(Enemy_Base.ExchangeResuit.Draw);
            }
            else if (pdamage <= edamage)
            {
                // 애너미 승리
                Player_Manager.instnace.player_Turn.Turn_ExchangeResuitAnim(Player_Turn.ExchangeResuit.Lose);
                enemy.Turn_ExchangeResuitAnim(Enemy_Base.ExchangeResuit.Win);
                playerCount++;
            }

            // 밀림 이동 & 애니메이션 대기
            while (Player_Manager.instnace.player_Turn.isRecoilMove || enemy.isRecoilMove)
            {
                yield return null;
            }

            // 다음 합 딜레이
            // yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
        }

        // UI Off -> 이거 지금은 여기서 끄는데, 원래는 공격 다 끝나고 끄는게 맞음!
        Player_UI.instance.TurnFight_ExchanageSummary(false, null);
        enemy.enemyUI.TurnFight_ExchanageSummary(false, null);


        // 다음 공격까지 딜레이
        // yield return new WaitForSeconds(Random.Range(0.1f, 0.15f));

        isExchange = false;

        // 남은 공격 횟수 체크
        int count;
        // 에너미 공격 호출
        if (playerCount == playerSlot.myAttack.damageValue.Length)
        {
            count = enemySlot.myAttack.attackCount - enemyCount;
            StartCoroutine(Turn_AttackCall(Object.Enemy, enemySlot, true, count));
        }

        // 플레이어 공격 호출
        if (enemyCount == enemySlot.myAttack.damageValue.Length)
        {
            count = playerSlot.myAttack.attackCount - playerCount;
            StartCoroutine(Turn_AttackCall(Object.Player, playerSlot, true, count));
        }
    }


    // 전체 합 & 공격 종료 후 복귀
    private void Turn_Return(Exchange exchange)
    {
        // 원래 위치로 복귀
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
    /// 합 종료 후 값의 차이에 따라 화면 흔들림을 다르게 호출하는 기능
    /// </summary>
    /// <param name="value">플레이어 & 몬스터 공격력 값의 차이 절댓값</param>
    private void Turn_ExchangeCamreaEffect(int value)
    {
        // 공격 값에 따라 화면 흔들림 정도 다르게 호출
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


    // 합 승리 & 일방공격 호출
    /// <summary>
    /// 공격 시 호출하는 코루틴
    /// </summary>
    /// <param name="win">플레이어 & 에너미 중 누가 승리했는가</param>
    /// <param name="slot">공격 데이터를 담은 슬롯</param>
    /// <param name="isExchange">합 공격인지 아닌지 확인 (합공격이면 필살기 게이지가 회복됨)</param>
    /// <param name="attackCount">남은 공격 횟수</param>
    private IEnumerator Turn_AttackCall(Object win, Attack_Slot slot, bool isExchange, int attackCount)
    {
        isAttack = true;

        // 플레이어 & 에너미 공격 호출
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

        // 공격 대기
        while (Player_Manager.instnace.isAttack || enemy.isAttack)
        {
            yield return null;
        }

        isAttack = false;
    }

    // 에너미 리스트 체크
    private void Enemy_Check()
    {
        for (int i = 0; i < enemys.Count; i++)
        {
            if (enemys[i].isDie)
            {
                // 공격 슬롯 삭제
                for (int j = 0; j < enemys[i].attack_Slots.Count; j++)
                {
                    enemySlots.Remove(enemys[i].attack_Slots[j]);
                }

                // 에너미 삭제
                enemyObjects.Remove(enemys[i].gameObject);
                enemys.Remove(enemys[i]);

                // 카운트
                enemyCount = enemys.Count;
            }
        }
    }

    // 리스트 초기화
    private void Turn_ListReset()
    {
        enemyExchangeList.Clear();
        exchangeList.Clear();
    }


    #region 전투 승리 & 패배

    // 전투 종료 체크
    private void Turn_StageCheck()
    {
        Debug.Log("종료 체크 호출");
        // 플레이어 사망 체크
        if (Player_Manager.instnace.isDie)
        {
            // 전투 패배 호출
            Turn_FightLose();
            return;
        }

        // 에너미 사망 체크
        for (int i = 0; i < enemys.Count; i++)
        {
            // 아직 안죽은 몬스터가 있다면 동작 종료
            if (!enemys[i].isDie)
            {
                Debug.Log("몹 살아있음");
                return;
            }
        }

        // 전투 승리 호출
        switch (roomType)
        {
            case RoomType.Normal:
                Debug.Log("전투 승리 노말 호출");
                Turn_ight_NormalWin();
                break;

            case RoomType.Boss:
                Debug.Log("전투 승리 보스 호출");
                TurnFthgt_BossWin();
                break;
        }
    }


    // 일반전투 승리 호출
    private void Turn_ight_NormalWin()
    {
        StopAllCoroutines();
        StartCoroutine(TurnFightNormalEndCall());
    }

    // 일반전투 승리 동작
    private IEnumerator TurnFightNormalEndCall() // 이거 필살기로 호출되면 종료 애니메이션 안나옴... 트리거 체크 안되는듯
    {
        curTurn = Turn.End;
        isEnd = true;

        // UI 초기화
        Player_UI.instance.TurnFight_ExchanageSummary(false, null);
        Player_UI.instance.TurnFight_Outline_Setting(Player_UI.Outline.TypeA, false);
        Player_UI.instance.TurnFight_Outline_Setting(Player_UI.Outline.TypeB, false);

        // 체력바 활성화
        Player_UI.instance.hpSet.SetActive(true);

        // 커서 잠금
        Player_Manager.instnace.Cursor_Setting(true);

        // 사운드
        Player_Manager.instnace.UI_Sound_Call(5);

        // 스킬 포인트 UI
        Player_Skill_Manager.instance.Add_SkillPoint(skillPoint);

        // 전투 종료 애니메이션 호출
        Player_Manager.instnace.player_Turn.Turn_End(Player_Turn.EndType.Win);
        while (Player_Manager.instnace.player_Turn.isEndAnim)
        {
            yield return null;
        }

        // 전투 종료 UI 호출
        Player_UI.instance.TurnFight_Win();
        while (Player_UI.instance.isFade)
        {
            yield return null;
        }

        // 사운드
        Stage_Manager.instance.BGM_Setting(0);

        // 다이얼로그 호출
        if (haveEndDialog)
        {
            Dialog_Manager.instance.Dialog(endDialogIndex);
        }

        // 튜토리얼
        if (haveEndTutorial)
        {
            Tutorial_Manager.instance.TutorialOn(tutorialIndex[1]);
        }

        // 매니저 파괴
        Destroy(gameObject);
    }


    // 보스전투 승리 호출
    private void TurnFthgt_BossWin()
    {
        StopAllCoroutines();
        StartCoroutine(TurnFightBossEndCall());
    }

    // 보스전투 승리 동작
    private IEnumerator TurnFightBossEndCall()
    {
        curTurn = Turn.End;
        isEnd = true;

        // UI 초기화
        Player_UI.instance.Turn_FightSelect(false);
        Player_UI.instance.TurnFight_Outline_Setting(Player_UI.Outline.TypeA, false);
        Player_UI.instance.TurnFight_Outline_Setting(Player_UI.Outline.TypeB, false);

        // 종료 영상 체크
        while (enemys[0].isCutscene)
        {
            yield return null;
        }

        // 다이얼로그 호출
        if (haveEndDialog)
        {
            Dialog_Manager.instance.Dialog(endDialogIndex);
        }

        // 플레이어 파괴
        Destroy(Player_Manager.instnace.gameObject);

        // 게임 종료 씬 호출
        Scene_Loading.LoadScene("Ending_Scene");
    }


    // 전투 패배 호출
    private void Turn_FightLose()
    {
        StartCoroutine(TurnFightLoseCall());
    }

    // 전투 패배 동작
    private IEnumerator TurnFightLoseCall()
    {
        curTurn = Turn.End;
        isEnd = true;

        // 배경 사운드 Off
        Stage_Manager.instance.BGM_OnOff(false);

        // UI 사운드
        Player_Manager.instnace.UI_Sound_Call(6);

        // UI 초기화
        Player_UI.instance.TurnFight_Outline_Setting(Player_UI.Outline.TypeA, false);
        Player_UI.instance.TurnFight_Outline_Setting(Player_UI.Outline.TypeB, false);

        // 전투 종료 애니메이션 호출
        Player_Manager.instnace.player_Turn.Turn_End(Player_Turn.EndType.Lose);
        while (Player_Manager.instnace.player_Turn.isEndAnim)
        {
            yield return null;
        }

        // 전투 종료 UI 호출
        Player_UI.instance.TurnFight_Lose();

        // 뒤지면 체크포인트에서 다시 시작
    } // -> 미완성
    #endregion
}
