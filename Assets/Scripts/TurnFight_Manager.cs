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
    // [SerializeField] private bool alreadyHaveAttack; // 이거 스킬 삭제할때 사용하는 불값임!

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
    [SerializeField] private List<Exchange> enemyExchangeList; // 에너미 공격 리스트
    [SerializeField] private List<Exchange> exchangeList; // 전체 공격 리스트
    [SerializeField] private List<Attack_Slot> enemySlots; // 에너미 슬롯 데이터
   
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
        Debug.Log("구조체 입력 호출됨!");

        // 리스트 체크 -> 여기부터 작업
        for (int i = 0; i < exchangeList.Count; i++)
        {
            Debug.Log(exchangeList[i].owner + " 데이터 오너");
            // 플레이어의 공격 중 이미 해당 슬롯을 공격하는 공격이 있는지 체크
            if (exchangeList[i].owner == Exchange.AttackOwner.Player && exchangeList[i].attackType == Exchange.AttackType.Exchange && exchangeList[i].enemy_Slot == newExchange.enemy_Slot)
            {
                // 속도가 더 빠른쪽이 합 & 느린쪽은 일방으로 변경
                if (newExchange.attackSpeed >= exchangeList[i].attackSpeed)
                {
                    Debug.Log("플레이어 공격이 더 빠름");
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
                    Debug.Log("플레이어 공격이 더 느림");
                    newExchange.Slot_AttackType_Setting(Exchange.AttackType.Oneside);
                    newExchange.Slot_AttackTarget_Setting(Exchange.AttackOwner.Player);
                    exchangeList.Add(newExchange);

                    // 공격 설정
                    player.myExchange = newExchange;
                    return;
                }
            }

            // 에너미의 공격 중 이미 내 슬롯을 공격하는 공격이 있는지 체크
            if (exchangeList[i].owner == Exchange.AttackOwner.Enemy && exchangeList[i].enemy_Slot == newExchange.enemy_Slot)
            {
                Debug.Log("에너미 공격 체크 호출됨!");

                // 지정한 슬롯이 이미 내 슬롯을 공격한다면
                if (newExchange.attackSpeed >= exchangeList[i].attackSpeed)
                {
                    Debug.Log("에너미 공격 속도보다 플레이어가 더 빠름! / 합 뺏어오기");

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
                    Debug.Log("에너미 공격 속도보다 플레이어가 더 느림! / 그냥 추가");

                    // 더 느리다면 일방공격으로 추가
                    newExchange.Slot_AttackType_Setting(Exchange.AttackType.Oneside);
                    newExchange.Slot_AttackTarget_Setting(Exchange.AttackOwner.Player);
                    exchangeList.Add(newExchange);

                    // 공격 설정
                    player.myExchange = newExchange;
                    return;
                }
            }
        }

        // 선행 공격이 존재하지 않는 슬롯이라면
        Debug.Log("일반 입력 호출됨!");

        exchangeList.Add(newExchange);
    }


    // 플레이어 공격 데이터 삭제 -> 구조체
    public void Exchange_Setting_Remove(Attack_Slot player, Attack_Slot enemy, Exchange exchange)
    {
        // 이거 추가한 순서대로 제거하면 인덱스에 문제가 없는데, 그게 아니라면 인덱스가 변경되니까 문제가 발생함!
        Debug.Log("공격 삭제 호출");
        // alreadyHaveAttack = false;
        if(exchange == null)
        {
            Debug.Log("에러 발생 / 인덱스 없음 / indexof에서 -1 반환");
            return;
        }

        // 지우려던 공격이 합 공격일때만 동작
        if(exchange.attackType == Exchange.AttackType.Exchange)
        {
            // 에너미 슬롯이 원래 공격하던 슬롯이 있었는지 체크
            for (int i = 0; i < enemyExchangeList.Count; i++)
            {
                if (enemyExchangeList[i].enemy_Slot == exchange.enemy_Slot)
                {
                    // 원래 공격 추가
                    Exchange newAttack = enemyExchangeList[i];
                    newAttack.owner = Exchange.AttackOwner.Enemy;
                    newAttack.Slot_AttackType_Setting(Exchange.AttackType.Oneside);
                    exchangeList.Add(newAttack);

                    // 공격 삭제
                    exchangeList.Remove(exchange);
                    player.ResetSlot();
                    return;
                }
            }
        }
        else
        {
            // 합 공격이 아니였다면
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
                    // 에너미 슬롯에 원래 공격이 있었는지 체크
                    for (int i2 = 0; i2 < enemyExchangeList.Count; i2++)
                    {
                        // 에너미 슬롯에 원래 공격이 있었다면
                        if (enemyExchangeList[i2].enemy_Slot == enemy)
                        {
                            // 이미 해당 에너미 공격이 리스트에 들어가 있는지 체크
                            for (int i3 = 0; i3 < exchangeList.Count; i3++)
                            {
                                // 이미 공격이 들어가 있다면 bool 값 true 반환
                                if (exchangeList[i3].owner == Exchange.AttackOwner.Enemy && exchangeList[i3].enemy_Slot == enemyExchangeList[i2].enemy_Slot)
                                {
                                    alreadyHaveAttack = true;
                                }

                                // 들어가 있다면 무시 / 아니라면 추가
                                if (!alreadyHaveAttack)
                                {
                                    Debug.Log("원래 에너미 공격 존재 / 추가함 !");

                                    // 신규 공격 셋팅
                                    Exchange newExchange = enemyExchangeList[i2];
                                    newExchange.owner = Exchange.AttackOwner.Enemy;
                                    newExchange.Slot_AttackType_Setting(Exchange.AttackType.Oneside);
                                    newExchange.Slot_AttackTarget_Setting(Exchange.AttackOwner.Enemy);

                                    // 기존 공격 삭제
                                    exchangeList[i].player_Slot.ResetSlot();
                                    exchangeList.RemoveAt(i);

                                    // 신규 공격 추가
                                    exchangeList.Add(newExchange);
                                    return;
                                }
                            }
                        }
                    }
                }
                else if (exchangeList[i].attackType == Exchange.AttackType.Oneside)
                {
                    // 원래 공격이 없었다면
                    Debug.Log("공격 삭제 ! :" + exchangeList[i].player_Slot);
                    exchangeList[i].player_Slot.ResetSlot();
                    exchangeList[i].enemy_Slot.ResetSlot();
                    exchangeList.RemoveAt(i);
                    return;
                }
            }
        }
        */
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
        Debug.Log("데이터 입력 호출");
        exchangeList.Add(data);
        // exchangeList.AddRange(enemyExchangeList);
    }


    // 시작 연출 호출 (전투 진입 시 1회 호출)
    public void TurnFight_Start()
    {
        StartCoroutine(TurnFight_StartCall());
    }


    // 시작 연출 동작 (전투 진입 시 1회 호출)
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
            enemys[i].turnManager = this;
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


    // 플레이어 스킬 선택
    private IEnumerator Turn_Select()
    {
        curTurn = Turn.Select;

        // 4. 플레이어 스킬 선택
        Player_Manager.instnace.player_Turn.Turn_AttackSelect();

        // 플레이어 슬롯 스피드 셋팅
        Player_Manager.instnace.player_Turn.Slot_SpeedSetting();

        // 에너미 공격 셋팅
        for (int i = 0;i < enemys.Count;i++)
        {
            // 여기에 속도 뿌리는 기능 추가해야 함! 
            enemys[i].Turn_AttackSetting();
        }

        // 선택 완료까지 대기
        while(Player_Manager.instnace.player_Turn.isSelect)
        {
            yield return null;
        }

        // 선택 UI 종료
        Player_UI.instance.Turn_FightSelect(false);

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
            if(enemys[i] != null)
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


    // 전투 기능 동작 ( 5, 6, 7, 8 )
    private IEnumerator Turn_Fight()
    {
        Debug.Log("Call Turn Fight");
        curTurn = Turn.Fight;

        // 리스트 속도 순으로 정리
        exchangeList.Sort((a, b) => b.attackSpeed.CompareTo(a.attackSpeed));

        // 합 리스트 만큼 공격 진행
        for (int i = 0; i < exchangeList.Count; i++)
        {
            Debug.Log(i + "번 슬롯 공격 !");
            Debug.Log("공격 타입 : " + exchangeList[i].attackType);
            Debug.Log("플레이어 공격 : " + exchangeList[i].player_Slot.myAttack + " / 적 공격 : " + exchangeList[i].enemy_Slot.myAttack);
            
            // 슬롯 셋팅
            playerSlot = exchangeList[i].player_Slot;
            enemySlot = exchangeList[i].enemy_Slot;
            enemy = enemySlot.slotOwner.GetComponent<Enemy_Base>();

            // 합 이동 호출
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

            // 대기
            yield return new WaitForSeconds(0.25f);

            // 합 시작
            if (exchangeList[i].attackType == Exchange.AttackType.Exchange)
            {
                Debug.Log("합 동작!");
                StartCoroutine(Turn_ExchangeAttack(exchangeList[i]));
                while (isExchange)
                {
                    yield return null;
                }
            }

            /*
            // 공격 시작
            while(isAttack)
            {
                yield return null;
            }

            // 원위치
            while(isExchangeMove)
            {
                yield return null;
            }
            */

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


    // 합 공격 이동
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


    // 합 동작 -> 사용 안함 (구조체로 변경)
    private IEnumerator Turn_ExchangeAttack(Exchange exchange)
    {
        isAttack = true;
        
        // 합 시작
        // 플레이어 또는 몬스터의 공격 횟수가 전부 소진될때가지 반복
        int playerCount = 0;
        int enemyCount = 0;
        while (playerCount < playerSlot.myAttack.damageValue.Length && enemyCount < enemySlot.myAttack.damageValue.Length)
        {
            // 데미지 계산
            (int pdamage, bool isPC) = Player_Manager.instnace.DamageCal(playerSlot.myAttack, playerCount);
            (int edamage, bool isCC) = enemy.DamageCal(enemySlot.myAttack, enemyCount);
            Debug.Log("플레이어 카운트 : " + playerCount + " / 에너미 카운트" + enemyCount);
            Debug.Log("플레이어 데미지 : " + pdamage + " / " + "에너미 데미지 : " + edamage);

            // 치명타 뜨면 UI 부분에서 추가적인 이펙트 넣을 것!

            // 합 결과
            if(pdamage > edamage)
            {
                // 플레이어 승리
                Player_Manager.instnace.player_Turn.Turn_ExchangeResuitAnim(Player_Turn.ExchangeResuit.Win);
                enemy.Turn_ExchangeResuitAnim(Enemy_Base.ExchangeResuit.Lose);
                enemyCount++;
                Debug.Log("Player Win!");
            }
            else if( pdamage == edamage)
            {
                // 무승부
                Player_Manager.instnace.player_Turn.Turn_ExchangeResuitAnim(Player_Turn.ExchangeResuit.Draw);
                enemy.Turn_ExchangeResuitAnim(Enemy_Base.ExchangeResuit.Draw);
                Debug.Log("Draw!");
            }
            else
            {
                // 애너미 승리
                Player_Manager.instnace.player_Turn.Turn_ExchangeResuitAnim(Player_Turn.ExchangeResuit.Lose);
                enemy.Turn_ExchangeResuitAnim(Enemy_Base.ExchangeResuit.Win);
                playerCount++;
                Debug.Log("enemy Win!");
            }

            // 밀림 이동 & 애니메이션 대기
            while (Player_Manager.instnace.player_Turn.isRecoilMove || enemy.isRecoilMove)
            {
                yield return null;
            }

            // 합 위치 복귀 이동 & 애니메이션
            Player_Manager.instnace.player_Turn.Turn_ExchangeMove_After(playerExchanagePos.position);
            enemy.Turn_ExchangeMove_After(playerExchanagePos.position);

            // 합 위치 복귀 이동 & 애니메이션 대기
            while (Player_Manager.instnace.player_Turn.isExchangeMove || enemy.isExchangeMove)
            {
                yield return null;
            }

            // 다음 합 딜레이
            yield return new WaitForSeconds(Random.Range(0.3f, 0.5f));
        }

        // 남은 공격 횟수 체크
        int count = (playerCount == 0 ? enemySlot.myAttack.attackCount - enemyCount : playerSlot.myAttack.attackCount - playerCount);
        Debug.Log("남은 공격 횟수 : " + count);


        // 플레이어 - 몬스터 공격 호출
        StartCoroutine(Turn_Attack((playerCount == 0 ? Object.Enemy : Object.Player), (playerCount == 0 ? enemySlot : playerSlot), count));
    }


    // 합 승리 & 일방공격 호출
    private IEnumerator Turn_Attack(Object win, Attack_Slot slot, int attackCount)
    {
        switch(win)
        {
            case Object.Player:
                // 공격 호출
                Player_Manager.instnace.player_Turn.Turn_Attack(enemy.gameObject, slot.myAttack, attackCount);

                // 공격 종료까지 대기
                while (Player_Manager.instnace.isAttack)
                {
                    yield return null;
                }
                break;

            case Object.Enemy:
                // 공격 호출
                enemy.Turn_Attack(Player_Manager.instnace.player_Turn.gameObject, slot.myAttack, attackCount);

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


    #region 전투 승리 & 패배
    // 전투 종료 체크
    private void Turn_StageCheck()
    {

    }


    // 전투 승리 호출
    private void Turn_FightWin()
    {
        StartCoroutine(TurnFightEndCall());
    }


    // 전투 승리 동작
    private IEnumerator TurnFightEndCall()
    {
        curTurn = Turn.End;

        // 전투 종료 애니메이션 호출
        Player_Manager.instnace.player_Turn.Turn_End(Player_Turn.EndType.Win);
        while (Player_Manager.instnace.player_Turn.isEndAnim)
        {
            yield return null;
        }

        // 전투 종료 UI 호출
        Player_UI.instance.TurnFight_Lose();
        while(Player_UI.instance.isFade)
        {
            yield return null;
        }

        // 플레이어 원위치
        Player_Manager.instnace.Turn_Fight_End();

        // 매니저 파괴
        Destroy(gameObject);
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

        // 전투 종료 애니메이션 호출
        Player_Manager.instnace.player_Turn.Turn_End(Player_Turn.EndType.Lose);
        while(Player_Manager.instnace.player_Turn.isEndAnim)
        {
            yield return null;
        }

        // 전투 종료 UI 호출 -> 이거 일단 승리꺼 넣어둠!
        Player_UI.instance.TurnFight_Lose();

        // 
    } // -> 미완성
    #endregion
}
