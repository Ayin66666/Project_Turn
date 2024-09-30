using Easing.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player_Turn : MonoBehaviour
{
    /*
    체크해야하는 값이

    0. 누구를 몇번 때릴건가?
	    = 플레이어 공격 체크

    1. 플레이어 전체 공격 횟수 vs 적 전체 공격 횟수
	    = 어느쪽이 몇번 일방공격을 하는가?

    2. 플레이어 공격 횟수 vs 적 공격 횟수
	    = 해당 공격이 몇번 합을 해야하는가?
	    = 합 종료 후 몇번 때릴 수 있는가?

    3. 일방 공격
	    = 플레이어&몬스터가 누구를 몇번 일방공격 하는가?
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


    // 턴 전투 시작 애니메이션 호출
    public void Turn_StartAnim()
    {
        curCoroutine = StartCoroutine(Turn_StartAnimCall());
    }


    // 턴 전투 시작 애니메이션 동작
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


    // 공격 슬롯 속도 셋팅
    public void Slot_SpeedSetting()
    {
        for (int i = 0; i < attackSlot.Count; i++)
        {
            int ran = Random.Range(Player_Manager.instnace.SlotSpeed.x, Player_Manager.instnace.SlotSpeed.y);
            attackSlot[i].Speed_Setting(ran);
        }
    }


    // 공격 선택 턴 시작
    public void Turn_AttackSelect()
    {
        if(isExchangeTargetSelect)
        {
            return;
        }
        isSelect = true;
       

        // 선택 UI On
        Player_UI.instance.Turn_FightSelect(true);

        // 슬롯 정리 -> 남아있는 공격 있을 경우 대비
        for (int i = 0; i < attackSlot.Count; i++)
        {
            attackSlot[i].ResetSlot();
        }
    }


    // 슬롯에 공격 삽입
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


    // 해당 슬롯으로 뭘 때릴지 선택
    public void Slot_TargetSetting(Attack_Slot slot)
    {
        Debug.Log("Call Slot TargetSetting");
        StartCoroutine(Slot_TargetSettingCall(slot));
    }


    private IEnumerator Slot_TargetSettingCall(Attack_Slot slot)
    {
        Debug.Log("Call Slot Target Setting");
        isExchangeTargetSelect = true;

        // 리스트 정리 -> 몹이 죽은 경우 null 뜰 수도 있으니
        enemyAttackList?.Clear(); // -> 이건 null 전파 기능이라는데 아마 삼항처럼 줄이는 용도인듯?

        // 몬스터 공격 슬롯 가져오기
        enemyAttackList = Player_Manager.instnace.turnManger.GetEnemyAttackSlot();
        
        // 타겟 선택 UI On
        enemyIndex = 0;
        Player_UI.instance.Turn_TargetSelect_DataSetting(true, enemyAttackList[enemyIndex].slotOwner.GetComponent<Enemy_Base>());
        Player_UI.instance.Turn_TargetSelect(true);
        slot.Attack_LineSetting(true, enemyAttackList[enemyIndex].gameObject);

        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Player, slot, true);
        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Enemy, enemyAttackList[enemyIndex], true);

        // 슬롯 발광 효과
        enemyAttackList[enemyIndex].Highlights_Effect(true);

        // 타겟 선택까지 대기
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            if(enemyAttackList.Count > 1)
            {
                // 좌로 이동
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

                // 우로 이동
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

        // 타겟 셋팅
        slot.Attack_TargetSetting(enemyAttackList[enemyIndex]);

        // 타겟 선택 UI Off
        Player_UI.instance.Turn_TargetSelect(false);
        Player_UI.instance.Turn_EngageUI(Player_UI.Object.None, slot, false);
        slot.Attack_LineSetting(false, null);
        enemyAttackList[enemyIndex].Highlights_Effect(false);

        // 전투 시작 체크
        Turn_FightButtonCheck();
    }


    // 전투 시작 버튼 활성화 체크
    private void Turn_FightButtonCheck()
    {
        // 상대 선택 후 아직 지정하지 않은 슬롯이 있는지 체크
        for (int i = 0; i < attackSlot.Count; i++)
        {
            // 아직 공격을 전부 지정하지 않았을 경우
            if (!attackSlot[i].haveAttack)
            {
                break;
            }
        }

        // 모든 슬롯에 공격이 지정된 경우
        Player_UI.instance.Turn_AttackButton(true);
    }


    // 합 이동 호출
    public void Turn_ExchangeMove(Transform movePos)
    {
        curCoroutine = StartCoroutine(Turn_EngageMoveCall(movePos));
    }


    // 합 이동 동작
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


    // 합 시작 애니매이션
    public void Turn_ExchangeStartAnim()
    {
        if(anim != null)
        {
            anim.SetTrigger("Exchange");
            anim.SetBool("isExchange", true);
        }
    }


    // 합 결과 애니메이션 호출
    public void Turn_ExchangeResuitAnim(ExchangeResuit type)
    {
        curCoroutine = StartCoroutine(Turn_ExchangeResuitAnimCall(type));
    }


    // 합 결과 애니메이션 동작
    private IEnumerator Turn_ExchangeResuitAnimCall(ExchangeResuit type)
    {
        isRecoilMove = true;

        // 애니메이션 대기 종료
        if(anim != null)
        {
            anim.SetTrigger("Exchange");
            anim.SetBool("isExchange", false);
        }

        // 합 후 딜레이
        yield return new WaitForSeconds(Random.Range(0.25f, 0.5f));

        // 합 후 밀림 이동 동작
        StartCoroutine(Turn_ExchanageResuitMove(type));

        // 합 애니메이션
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

        // 밀림 이후 대기
        yield return new WaitForSeconds(Random.Range(0.25f, 0.55f));

        isRecoilMove = false;
    }


    // 합 밀림 이동 동작
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


    // 공격 기능 호출 & 동작
    public void Turn_Attack(GameObject target, Attack_Base attack)
    {
        attack.UseAttack(Attack_Base.AttackOwner.Player, gameObject, target);
    }


    // 맨 마지막 전투 종료 할 때 호출 -> 승리/사망 모션 같은거
    public void Turn_End(EndType type)
    {
        StartCoroutine(Turn_EndCall(type));
    }


    // 애니메이션 -> 아직 애니메이션 이후 기능 안만듬!
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
