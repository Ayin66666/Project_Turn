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


    // 1. 페이드 연출 호출

    // 2. 전투 필드로 플레이어 이동

    // 3. 페이드 연출 종료 후 플레이어 시작 모션 & 몬스터 활성화

    // 4. 플레이어 스킬 선택

    // 5. 선택 UI 종료

    // 6. 플레이어 - 몬스터 이동

    // 7. 플레이어 - 몬스터 합

    // 8. 플레이어 - 몬스터 원위치

    // 이하 4 ~ 8 반복



    // 1 2 3 호출
    public void TurnFight_Start()
    {
        StartCoroutine(TurnFight_StartCall());
    }


    // 1 2 3 동작
    private IEnumerator TurnFight_StartCall() 
    {
        curTurn = Turn.Start;

        // 1. 페이드 연출 호출
        Player_UI.instance.TurnFight_Fade();

        // 페이드 인 될때까지 대기
        yield return new WaitForSeconds(1f);


        // 2. 전투 필드로 플레이어 이동
        Player_Manager.instnace.Turn_Fight_Start(playerReturnPos, this);


        // 페이드 연출 종료 대기
        while (Player_UI.instance.isFade)
        {
            yield return null;
        }

        // 3. 플레이어 시작 모션 & 몬스터 활성화
        Player_Manager.instnace.player_Turn.Turn_StartAnim();
        for (int i = 0; i < enemys.Count; i++)
        {
            enemys[i].gameObject.SetActive(true);
        }

        // 시작 애니메이션 대기
        while (Player_Manager.instnace.player_Turn.isSpawnAnim)
        {
            yield return null;
        }

        // 페이드 아웃 후 일시 대기
        yield return new WaitForSeconds(1f);

        // 4. 플레이어 스킬 선택
        StartCoroutine(Turn_Select());
    }


    // 4. 플레이어 스킬 선택
    private IEnumerator Turn_Select()
    {
        curTurn = Turn.Select;

        // 4. 플레이어 스킬 선택
        Player_Manager.instnace.player_Turn.Turn_AttackSelect();
        while(Player_Manager.instnace.player_Turn.isSelect)
        {
            yield return null;
        }
    }


    // 플레이어가 자신의 공격을 선택하고, 어떤 슬롯에 공격을 날릴지 정하기 위해 데이터 전달
    public List<Attack_Slot> GetEnemyAttackSlot()
    {
        List<Attack_Slot> enemyAttackList = new List<Attack_Slot>();

        // 모든 몬스터의 공격 슬롯 순회 -> 처음부터 마지막까지 공격 순서 및 공격 받아오기
        for (int i = 0; i < enemys.Count; i++)
        {
            // 혹시 모를 리스트 null 체크
            if(enemys[i] != null)
            {
                // 해당 몬스터으 공격 슬롯을 리스트에 담기
                for (int i2 = 0; i2 < enemys[i].attack_Slots.Count; i2++)
                {
                    enemyAttackList.Add(enemys[i].attack_Slots[i2]);
                }
            }
        }

        for (int i3 = 0; i3 < enemyAttackList.Count; i3++)
        {
            Debug.Log(enemyAttackList[i3]);

        }
        // 최종적으로 담아진 데이터 넘기기
        return enemyAttackList;
    }

    // 전투 기능 동작
    private IEnumerator Turn_Fight()
    {
        curTurn = Turn.Fight;


        // 5. 선택 UI 종료
        Player_UI.instance.Turn_FightSelect(false);


        // 이하 순차적으로 공격하는 기능


        // 플레이어 & 몬스터 슬롯 리스트를 하나로 합치는 기능
        List<Attack_Slot> combine = new List<Attack_Slot>();


        // 리스트의 값을 가장 빠른순부터 정렬
        combine.Sort((a, b) => b.slotSpeed.CompareTo(a.slotSpeed));


        // 공격 속도대로 순차적 공격
        for (int i = 0; i < combine.Count; i++)
        {
            // 슬롯 셋팅
            Attack_Setting(combine[i]);

            // 6. 플레이어 - 몬스터 이동
            switch (combine[i].attackType)
            {
                // 공격 없을 경우
                case Attack_Slot.AttackType.None:
                    break;

                // 플레이어 - 몬스터 이동 (일방 공격)
                case Attack_Slot.AttackType.Oneside_Attack:
                    StartCoroutine(Turn_ExchangeMove_OneSide(combine[i].slotType == Attack_Slot.SlotType.Player ? Object.Player : Object.Enemy));
                    break;

                // 플레이어 - 몬스터 이동 (합 공격)
                case Attack_Slot.AttackType.Exchange_Attacks:
                    StartCoroutine(Turn_ExchangeMove_Exchange());
                    break;
            }

            // 이동 대기
            while(isExchangeMove)
            {
                yield return null;
            }


            // 7. 플레이어 - 몬스터 합 시작 애니메이션
            if (combine[i].attackType == Attack_Slot.AttackType.Exchange_Attacks)
            {
                Player_Manager.instnace.player_Turn.Turn_ExchangeStartAnim();
                enemy.Turn_ExchangeStartAnim();
                yield return new WaitForSeconds(0.5f);
            }

            // 7. 플레이어 - 몬스터 합 결과 값 & 결과 애니메이션
            // 공격 타입 별 동작
            switch (combine[i].attackType)
            {
                // 일방 공격의 경우
                case Attack_Slot.AttackType.Oneside_Attack:
                    StartCoroutine(Turn_OneSideAttack(combine[i].slotType == Attack_Slot.SlotType.Player ? Object.Player : Object.Enemy, combine[i]));
                    break;

                // 합 공격의 경우
                case Attack_Slot.AttackType.Exchange_Attacks:
                    StartCoroutine(Turn_ExchangeAttack(combine[i], enemySlot));
                    break;
            }

            // 공격 대기
            while(isAttack)
            {
                yield return null;
            }


            // 공격 종료 후 리스트에서 제거
            // -> 만약 합 공격의 경우 둘 다 제거
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


            // 다음 공격까지의 잠시 대기
            yield return new WaitForSeconds(nextAttackDelay);
        }


        // 8. 플레이어 - 몬스터 원위치
        Turn_ReturnPos();
    }


    // 일방 공격 이동
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


    // 플레이어 - 몬스터 슬롯 할당
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


    // 합 공격 이동
    private IEnumerator Turn_ExchangeMove_Exchange()
    {
        isExchangeMove = true;

        
        while (Player_Manager.instnace.player_Turn.isExchangeMove || enemy.isExchangeMove)
        {
            yield return null;
        }

        isExchangeMove = false;
    }


    // 일방 공격 동작 -> 여기부터 작업!
    private IEnumerator Turn_OneSideAttack(Object attackOwner, Attack_Slot slot)
    {
        isAttack = true;


        yield return null;
        isAttack = false;
    }


    // 합 공격 동작
    private IEnumerator Turn_ExchangeAttack(Attack_Slot playerSlot, Attack_Slot enemySlot)
    {
        isAttack = true;
        
        // 합 시작
        // 플레이어 또는 몬스터의 공격 횟수가 전부 소진될때가지 반복
        int playerCount = 0;
        int enemyCount = 0;
        while (playerCount < playerSlot.myAttack.attackCount || enemyCount < enemySlot.myAttack.attackCount)
        {
            // 데미지 계산
            (int pdamage, bool isPC) = Player_Manager.instnace.DamageCal(playerSlot.myAttack, playerCount);
            (int edamage, bool isCC) = enemy.DamageCal(enemySlot.myAttack, enemyCount);

            // 치명타 뜨면 UI 부분에서 추가적인 이펙트 넣을 것!

            // 합 결과
            if(pdamage > edamage)
            {
                // 플레이어 승리
                Player_Manager.instnace.player_Turn.Turn_ExchangeResuitAnim(Player_Turn.ExchangeResuit.Win);
                enemy.Turn_ExchangeResuitAnim(Enemy_Base.ExchangeResuit.Lose);
                enemyCount++;
            }
            else if( pdamage == edamage)
            {
                // 무승부
                Player_Manager.instnace.player_Turn.Turn_ExchangeResuitAnim(Player_Turn.ExchangeResuit.Draw);
                enemy.Turn_ExchangeResuitAnim(Enemy_Base.ExchangeResuit.Draw);
            }
            else
            {
                // 애너미 승리
                Player_Manager.instnace.player_Turn.Turn_ExchangeResuitAnim(Player_Turn.ExchangeResuit.Lose);
                enemy.Turn_ExchangeResuitAnim(Enemy_Base.ExchangeResuit.Win);
                playerCount++;
            }

            // 다음 합 딜레이
            yield return new WaitForSeconds(Random.Range(0.15f, 0.25f));
        }

        // 플레이어 - 몬스터 공격 호출
        StartCoroutine(Turn_Attack((playerCount == 0 ? Object.Enemy : Object.Player), (playerCount == 0 ? enemySlot : playerSlot)));
    }

    // 합 승리 & 일방공격 호출
    private IEnumerator Turn_Attack(Object win, Attack_Slot slot)
    {
        switch(win)
        {
            case Object.Player:
                // 공격 호출
                Player_Manager.instnace.player_Turn.Turn_Attack(enemy.gameObject, slot.myAttack);

                // 공격 종료까지 대기
                while (Player_Manager.instnace.isAttack)
                {
                    yield return null;
                }
                break;

            case Object.Enemy:
                // 공격 호출
                // enemy.Turn_Attack(Player_Manager.instnace.player_Turn.gameObject, slot.myAttack);

                // 공격 종료까지 대기
                while (enemy.isAttack)
                {
                    yield return null;
                }
                break;
        }

        isAttack = false;
    }

    // 8. 플레이어 - 몬스터 원위치
    private void Turn_ReturnPos()
    {
        Player_Manager.instnace.player_Turn.transform.position = playerReturnPos.position;
        for (int i = 0; i < enemys.Count; i++)
        {
            enemys[i].transform.position = enemyReturnPos[i].position;
        }
    }
}
