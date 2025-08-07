using Easing.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;


public class Player_Turn : MonoBehaviour
{
    [Header("=== State ===")]
    public bool isSpawnAnim;
    public bool isSelect;
    public bool isExchangeMove;
    public bool isRecoilMove;
    public bool isExchangeTargetSelect;
    public bool isEndAnim;
    public bool isLook;
    [SerializeField]private float movement; // 애니메이션

    public enum ExchangeResuit { Win, Draw, Lose }
    public enum EndType { Win, Lose }

    [Header("=== Attack Setting ===")]
    public List<Attack_Slot> attackSlot;
    [SerializeField] private Attack_Base undersideData;


    [Header("=== Pos Setting ===")]
    [SerializeField] private Transform[] exchange_RecoilPos;
    public Transform exchangePos;


    [Header("=== LookAt Setting ===")]
    public GameObject forwardTarget; // 몬스터를 주시하지 않을 때 바라볼 대상
    [SerializeField] private GameObject looktarget; // 주시할 대상


    [Header("=== Component ===")]
    public Animator anim = null;
    public Player_Turn_Animation turnAnim;
    private Coroutine curCoroutine;
    private Coroutine selectAnimCoroutine;
    private Coroutine lookCoroutine;


    [Header("=== Target List ===")]
    [SerializeField] private List<Attack_Slot> enemyAttackList;
    [SerializeField] private int enemyIndex;


    [Header("=== Attack ===")]
    [Range(0,1)]
    [SerializeField] private float posF;
    [SerializeField] private GameObject target_Lockon;
    [SerializeField] public List<GameObject> attackTargets; // 다수 공격시 사용
    [SerializeField] public GameObject attackTarget; // 한명 공격 시 사용
    [SerializeField] private Attack_Base myAttack;
    public int curAttackCount;
    [SerializeField] private bool isExchange;

    [Header("---Animation---")]
    [SerializeField] private string[] animTrigger;
    [SerializeField] private string[] animBool;

    // 디버깅용 코드 - 이슈 위치 모르겠음;;
    public void ResetAll()
    {
        for (int i = 0; i < animTrigger.Length; i++)
        {
            anim.ResetTrigger(animTrigger[i]);
        }

        for (int i = 0; i < animBool.Length; i++)
        {
            anim.SetBool(animBool[i], false);
        }
    }


    // 턴 전투 시작 애니메이션 호출
    public void Turn_StartAnim()
    {
        curCoroutine = StartCoroutine(Turn_StartAnimCall());
    }

    // 턴 전투 시작 애니메이션 동작 -> 카메라 연출 필요함!
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

        // 슬롯 정리 -> 남아있는 공격 있을 경우 대비
        for (int i = 0; i < attackSlot.Count; i++)
        {
            attackSlot[i].ResetSlot();
        }
    }


    // 바라보기 기능 동작
    public void LookAt(GameObject target)
    {

        looktarget = target;
        if(lookCoroutine != null)
        {
            StopCoroutine(lookCoroutine);
        }
        lookCoroutine = StartCoroutine(LookAtCall());
    }
    private IEnumerator LookAtCall()
    {
        Vector3 lookDir = (looktarget.transform.position - transform.position).normalized;
        lookDir.y = 0;

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(lookDir);
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * 2f;
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, EasingFunctions.OutExpo(timer));
            yield return null;
        }

    }


    // 공격 간 타겟 위치로 이동
    public void Turn_AttackMove()
    {
        StartCoroutine(Turn_AttackMoveCall());
    }

    private IEnumerator Turn_AttackMoveCall()
    {
        isExchangeMove = true;

        // 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isAttackMove", true);
        anim.SetBool("isExchangeMoveReady", true);

        // 이동 준비 애니메이션 대기
        while(anim.GetBool("isExchangeMoveReady"))
        {
            yield return null;
        }

        // 사운드
        Player_Manager.instnace.Turn_Sound_Call(0);

        // 이동
        Vector3 startPos = transform.position;
        Vector3 endPos = attackTarget.GetComponent<Enemy_Base>().exchangePos.position;
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * 2.5f;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        anim.SetBool("isAttackMove", false);

        yield return new WaitForSeconds(0.05f);
        isExchangeMove = false;
    }


    // 슬롯에 공격 삽입
    public void Slot_AttackSetting(Attack_Base attack)
    {
        // 공격 포인트 체크
        if(Player_Manager.instnace.curAttackPoint < attack.attackCost)
        {
            Debug.Log("포인트 부족");
            return;
        }
        else
        {
            // 빈 슬롯 체크
            for (int i = 0; i < attackSlot.Count; i++)
            {
                if (!attackSlot[i].haveAttack)
                {
                    // 공격 포인트 UI 소모
                    Player_Manager.instnace.AttackPointAdd(-attack.attackCost);

                    attackSlot[i].Attack_Setting(attack);
                    Slot_TargetSetting(attackSlot[i]);
                    return;
                }
            }

            Debug.Log("슬롯 꽉참!");
        }
    }

    // 해당 슬롯으로 뭘 때릴지 선택 호출
    public void Slot_TargetSetting(Attack_Slot slot)
    {
        StartCoroutine(Slot_TargetSettingCall(slot));
    }

    // 해당 슬롯으로 뭘 때릴지 선택 동작
    private IEnumerator Slot_TargetSettingCall(Attack_Slot slot)
    {
        isExchangeTargetSelect = true;

        // 카메라 변경
        Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.Select, 0.35f);

        // 리스트 정리 -> 몹이 죽은 경우 null 뜰 수도 있으니
        enemyAttackList?.Clear(); // -> 이건 null 전파 기능이라는데 아마 삼항처럼 줄이는 용도인듯?

        // 몬스터 공격 슬롯 가져오기
        enemyAttackList = Player_Manager.instnace.turnManger.GetEnemyAttackSlot();

        // 타겟 선택 UI On
        enemyIndex = 0;
        Player_UI.instance.Turn_TargetSelect_DataSetting(true, enemyAttackList[enemyIndex].slotOwner.GetComponent<Enemy_Base>()); // -> 이거 왜 에러 발생?
        Player_UI.instance.Turn_TargetSelect(true);
        slot.Attack_LineSetting(true, enemyAttackList[enemyIndex].gameObject);

        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Player, slot, true);
        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Enemy, enemyAttackList[enemyIndex], true);

        // 승률 비교 UI On
        Player_UI.instance.Turn_WinningUI(true, slot, enemyAttackList[enemyIndex]);

        // 슬롯 발광 효과
        enemyAttackList[enemyIndex].Highlights_Effect(true);

        // 락온
        Turn_LockOn(true, enemyAttackList[enemyIndex].slotOwner.transform);

        // 바라보기 On
        LookAt(enemyAttackList[0].slotOwner);

        // 애니메이션 On
        if (selectAnimCoroutine != null)
        {
            StopCoroutine(selectAnimCoroutine);
        }
        selectAnimCoroutine = StartCoroutine(Slot_TargetSettingAnim(true));

        // 합 & 일방 텍스트
        if (slot.slotSpeed >= enemyAttackList[enemyIndex].slotSpeed)
        {
            Player_UI.instance.Turn_Exchange(true);
        }
        else
        {
            Player_UI.instance.Turn_Exchange(false);
        }

        // 타겟 선택까지 대기
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            if (enemyAttackList.Count > 1)
            {
                // 좌로 이동
                if (Input.GetKeyDown(KeyCode.A))
                {
                    enemyAttackList[enemyIndex].Highlights_Effect(false);

                    enemyIndex = (enemyIndex - 1 + enemyAttackList.Count) % enemyAttackList.Count;
                    Player_UI.instance.Turn_TargetSelect_DataSetting(true, enemyAttackList[enemyIndex].slotOwner.GetComponent<Enemy_Base>());
                    Player_UI.instance.Turn_EngageUI(Player_UI.Object.Player, slot, true);
                    Player_UI.instance.Turn_EngageUI(Player_UI.Object.Enemy, enemyAttackList[enemyIndex], true);
                    Player_UI.instance.Turn_WinningUI(true, slot, enemyAttackList[enemyIndex]);

                    // 합 & 일방 텍스트
                    if (slot.slotSpeed >= enemyAttackList[enemyIndex].slotSpeed)
                    {
                        Player_UI.instance.Turn_Exchange(true);
                    }
                    else
                    {
                        Player_UI.instance.Turn_Exchange(false);
                    }

                    // 슬롯 이펙트
                    slot.Attack_LineSetting(true, enemyAttackList[enemyIndex].gameObject);
                    enemyAttackList[enemyIndex].Highlights_Effect(true);
                    Turn_LockOn(true, enemyAttackList[enemyIndex].slotOwner.transform);

                    // 플레이어가 바라보는 타겟 셋팅
                    LookAt(enemyAttackList[enemyIndex].slotOwner);
                }

                // 우로 이동
                if (Input.GetKeyDown(KeyCode.D))
                {
                    enemyAttackList[enemyIndex].Highlights_Effect(false);

                    enemyIndex = (enemyIndex + 1) % enemyAttackList.Count;
                    Player_UI.instance.Turn_TargetSelect_DataSetting(true, enemyAttackList[enemyIndex].slotOwner.GetComponent<Enemy_Base>());
                    Player_UI.instance.Turn_EngageUI(Player_UI.Object.Player, slot, true);
                    Player_UI.instance.Turn_EngageUI(Player_UI.Object.Enemy, enemyAttackList[enemyIndex], true);
                    Player_UI.instance.Turn_WinningUI(true, slot, enemyAttackList[enemyIndex]);

                    // 합 & 일방 텍스트
                    if (slot.slotSpeed >= enemyAttackList[enemyIndex].slotSpeed)
                    {
                        Player_UI.instance.Turn_Exchange(true);
                    }
                    else
                    {
                        Player_UI.instance.Turn_Exchange(false);
                    }

                    // 슬롯 이펙트
                    slot.Attack_LineSetting(true, enemyAttackList[enemyIndex].gameObject);
                    enemyAttackList[enemyIndex].Highlights_Effect(true);
                    Turn_LockOn(true, enemyAttackList[enemyIndex].slotOwner.transform);

                    // 플레이어가 바라보는 타겟 셋팅
                    LookAt(enemyAttackList[enemyIndex].slotOwner);
                }
            }

            yield return null;
        }


        // 바라보기 Off
        LookAt(forwardTarget);


        // 카메라 변경
        Camera_Manager.instance.Camera_Setting(Camera_Manager.CameraType.None, 0.4f);


        // 애니메이션 Off
        if (selectAnimCoroutine != null)
        {
            StopCoroutine(selectAnimCoroutine);
        }
        selectAnimCoroutine = StartCoroutine(Slot_TargetSettingAnim(false));


        // 타겟 셋팅 -> 구조체 버전
        Player_Manager.instnace.turnManger.Exchange_Setting_Add(slot, enemyAttackList[enemyIndex]);
        //Player_Manager.instnace.turnManger.Exchange_Setting_Add2(slot, enemyAttackList[enemyIndex]);
        slot.targetSlot = enemyAttackList[enemyIndex];


        // 타겟 선택 UI Off
        Player_UI.instance.Turn_TargetSelect(false);
        Player_UI.instance.Turn_WinningUI(false, null, null);
        Player_UI.instance.Turn_EngageUI(Player_UI.Object.None, slot, false);
        slot.Attack_LineSetting(false, null);
        enemyAttackList[enemyIndex].Highlights_Effect(false);
        isExchangeTargetSelect = false;
        Turn_LockOn(false, null);


        // 전투 시작 체크
        Turn_FightButtonCheck();
    }

    // 타겟 선택 간 애니메이션 On/Off
    private IEnumerator Slot_TargetSettingAnim(bool isOn)
    {
        if(isOn)
        {
            // 애니메이션
            anim.SetFloat("Movement", movement);
            float start = movement;
            float end = 1;
            float timer = 0;
            while (movement < 1)
            {
                timer += Time.deltaTime * 1.2f;
                movement = Mathf.Lerp(start, end, EasingFunctions.OutExpo(timer));
                anim.SetFloat("Movement", movement);
                yield return null;
            }
            movement = 1;
            anim.SetFloat("Movement", movement);
        }
        else
        {
            // 애니메이션
            anim.SetFloat("Movement", movement);
            float start = movement;
            float end = 0;
            float timer = 0;
            while (movement > 0)
            {
                timer += Time.deltaTime * 1.5f;
                movement = Mathf.Lerp(start, end, EasingFunctions.OutExpo(timer));
                anim.SetFloat("Movement", movement);
                yield return null;
            }
            movement = 0;
            anim.SetFloat("Movement", movement);
        }
    }


    // 락온 이미지 on/off 위치 셋팅
    private void Turn_LockOn(bool isOn, Transform lockPos)
    {
        Debug.Log("Call LockOn" + isOn);
        target_Lockon.SetActive(isOn);
        if (isOn)
        {
            Vector3 pos = Vector3.Lerp(transform.position, lockPos.position, posF);
            pos.y = 1;
            target_Lockon.transform.position = pos;
        }
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
                return;
            }
        }

        // 모든 슬롯에 공격이 지정된 경우
        Player_UI.instance.Turn_AttackButton(true);
    }

    // 일방 공격 이동 호출
    public void Turn_OneSideMove(Transform movePos, float moveSpeed)
    {
        curCoroutine = StartCoroutine(Turn_OnesideMoveCall(movePos, moveSpeed));
    }

    // 일방 공격 이동 동작
    private IEnumerator Turn_OnesideMoveCall(Transform movePos, float moveSpeed)
    {
        isExchangeMove = true;

        // 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isExchangeMove", true);
        anim.SetBool("isExchangeMoveReady", true);

        // 이동 준비 동작 대기
        while (anim.GetBool("isExchangeMoveReady"))
        {
            yield return null;
        }

        // 사운드
        Player_Manager.instnace.Turn_Sound_Call(0);

        // 이동 코드
        Vector3 startPos = transform.position;
        Vector3 endPos = movePos.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        anim.SetBool("isExchangeMove", false);

        isExchangeMove = false;
    }
    

    // 합 이동 호출
    public void Turn_ExchangeMove(Transform movePos, float moveSpeed)
    {
        curCoroutine = StartCoroutine(Turn_EngageMoveCall(movePos, moveSpeed));
    }

    // 합 이동 동작
    private IEnumerator Turn_EngageMoveCall(Transform movePos, float moveSpeed)
    {
        isExchangeMove = true;

        // 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool("isExchangeMove", true);
        anim.SetBool("isExchangeMoveReady", true);

        // 이동 준비 동작 대기
        while (anim.GetBool("isExchangeMoveReady"))
        {
            yield return null;
        }

        // 사운드
        Player_Manager.instnace.Turn_Sound_Call(0);

        // 이동 코드
        Vector3 startPos = transform.position;
        Vector3 endPos = movePos.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        transform.position = endPos;

        isExchangeMove = false;
    }


    // 합 종료 후 복귀 이동 호출
    public void Turn_ExchangeEndMove(Transform movePos, float moveSpeed)
    {
        StartCoroutine(Turn_ExchangeEndMoveCall(movePos, moveSpeed));
    }

    // 합 종료 후 복귀 이동 동작
    private IEnumerator Turn_ExchangeEndMoveCall(Transform movePos, float moveSpeed)
    {
        isExchangeMove = true;

        // 타겟 바라보기
        //Player_Manager.instnace.player_Turn.LookAt(Player_Manager.instnace.turnManger.exchangeHolderPos.gameObject);

        // 애니메이션
        if (anim != null)
        {
            anim.SetTrigger("Action");
            anim.SetBool("isBackStep", true);
        }

        // 사운드
        Player_Manager.instnace.Turn_Sound_Call(1);

        // 이동
        Vector3 startPos = transform.position;
        Vector3 endPos = movePos.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        while(anim.GetBool("isBackStep"))
        {
            yield return null;
        }
        isExchangeMove = false;
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

        // 합 후 밀림 이동 동작
        StartCoroutine(Turn_ExchanageResuitMove(type));

        // 합 애니메이션
        if(anim != null)
        {
            // 이동 애니메이션 종료
            anim.SetBool("isExchangeMove", false);

            // 합 결과 애니메이션 호출 
            anim.SetTrigger("Action");
            switch (type)
            {
                case ExchangeResuit.Win:
                case ExchangeResuit.Draw:
                    int ran = Random.Range(0, 3);
                    anim.SetInteger("WinAnim", ran);
                    anim.SetBool("isExchangeWin", true);
                    while(anim.GetBool("isExchangeWin"))
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

    // 합 밀림 이동 동작
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


    // 필살기 호출 
    public void Turn_UndersideAttack()
    {
        if(Player_Manager.instnace.turnManger != null)
        {
            // 버튼 UI 종료
            Player_UI.instance.Turn_UndersideSkillOn(false);
            curAttackCount = 0;

            // 공격
            Turn_AttackData_Setting(Player_Manager.instnace.turnManger.enemyObjects, undersideData, false, undersideData.attackCount);
            Player_Manager.instnace.turnManger.Turn_UnderSideAttack();
        }
        else
        {
            Debug.Log("턴 메니져 없음!");
        }
    }



    // 공격 기능 호출 & 공격 데이터 입력 - 단일 공격
    /// <summary>
    /// 
    /// </summary>
    /// <param name="target">공격하는 대상</param>
    /// <param name="attack">공격 데이터</param>
    /// <param name="isExchange">합 여부</param>
    /// <param name="attackCount">남은 공격 횟수</param>
    public void Turn_AttackData_Setting(GameObject target, Attack_Base attack, bool isExchange, int attackCount)
    {
        Player_Manager.instnace.isAttack = true;
        Debug.Log("데이터 셋팅 호출 / " + target);

        // 데이터 셋팅
        attackTarget = target;
        myAttack = attack;
        curAttackCount = 0;
        this.isExchange = isExchange;

        Debug.Log("데이터 셋팅 호출 이후 데이터 / " + attackTarget);


        // 공격 UI 호출
        Player_UI.instance.TurnFight_ExchanageSummary(true, myAttack);

        // 공격 호출
        myAttack.UseAttack(Attack_Base.AttackOwner.Player, gameObject, target, isExchange, attackCount);
    }

    // 공격 기능 호출 & 공격 데이터 입력 - 다수 공격
    /// <summary>
    /// 
    /// </summary>
    /// <param name="target">공격하는 대상</param>
    /// <param name="attack">공격 데이터</param>
    /// <param name="isExchange">합 여부</param>
    /// <param name="attackCount">남은 공격 횟수</param>
    public void Turn_AttackData_Setting(List<GameObject> targets, Attack_Base attack, bool isExchange, int attackCount)
    {
        Player_Manager.instnace.isAttack = true;

        // 데이터 셋팅
        for (int i = 0; i < targets.Count; i++)
        {
            attackTargets.Add(targets[i]);
        }
        myAttack = attack;
        curAttackCount = 0;
        this.isExchange = isExchange;

        // 공격 UI 호출
        Player_UI.instance.TurnFight_ExchanageSummary(true, myAttack);

        // 공격 호출
        myAttack.UseAttack(Attack_Base.AttackOwner.Player, gameObject, targets, isExchange, attackCount);
    }


    // 공격 데이터 초기화
    public void Turn_AttackData_Reset()
    {
        attackTarget = null;
        attackTargets.Clear();
        myAttack = null;
        curAttackCount = 0;
        isExchange = false;
    }


    // 애니메이션 이벤트에서 호출하는 부분 - 공격 데이터를 사용해서 적에게 데미지
    public void Attack_Call()
    {
        // 데미지 계산
        (int pdamage, bool isCritical) = Player_Manager.instnace.DamageCal(myAttack, curAttackCount);

        // 데미지 UI
        Player_UI.instance.TurnFight_ExchangeSummary_ExchangeValue(pdamage, 0.1f);

        // 데미지 전달
        if (myAttack.isAOE)
        {
            for (int i = 0; i < Player_Manager.instnace.turnManger.enemyObjects.Count; i++)
            {
                for (int j = curAttackCount; j < myAttack.damageValue.Length; j++)
                {
                    if (attackTargets[i] != null)
                    {
                        attackTargets[i].GetComponent<Enemy_Base>().Take_Damage(pdamage, myAttack.damageType[curAttackCount] == Attack_Base.DamageType.physical ? Enemy_Base.DamageType.Physical : Enemy_Base.DamageType.Magical, isExchange, isCritical);
                    }
                }
            }
        }
        else
        {
            attackTarget.GetComponent<Enemy_Base>().Take_Damage(pdamage, myAttack.damageType[curAttackCount] == Attack_Base.DamageType.physical ? Enemy_Base.DamageType.Physical : Enemy_Base.DamageType.Magical, isExchange, isCritical);
        }

        // 공격 횟수 증가
        curAttackCount++;
    }

    // 애니메이션 1개 & 공격은 연타일 경우
    public void AttackCombo_Call()
    {
        StartCoroutine(AttackCombo());
    }

    private IEnumerator AttackCombo()
    {
        for (int i = 0; i < curAttackCount; i++)
        {
            (int pdamage, bool isCritical) = Player_Manager.instnace.DamageCal(myAttack, i);
            attackTarget.GetComponent<Enemy_Base>().Take_Damage(pdamage, Enemy_Base.DamageType.Physical, false, isCritical);
           
            // 딜레이
            yield return new WaitForSeconds(0.15f);
        }
    }


    // 필살기 10연타 공격
    public void Attack_UndersideCall()
    {
        // 사운드
        Player_Manager.instnace.Turn_Sound_Call(4);

        // 데미지 계산
        (int pdamage, bool isCritical) = Player_Manager.instnace.DamageCal(myAttack, curAttackCount);
        for (int i = 0; i < attackTargets.Count; i++)
        {
            attackTargets[i].GetComponent<Enemy_Base>().Take_Damage(pdamage, Enemy_Base.DamageType.Physical, false, isCritical);
        }
        curAttackCount++;
    }

    // 필살기 마지막 공격
    public void Attack_UndersideLastCall()
    {
        // 사운드
        Player_Manager.instnace.Turn_Sound_Call(10); //-> 인덱스 이슈

        // 데미지 계산
        Debug.Log($"필살기 막타 호출 횟수2 / {curAttackCount} 현재 공격 횟수");
        (int pdamage, bool isCritical) = Player_Manager.instnace.DamageCal(myAttack, curAttackCount);
        for (int i = 0;  i < attackTargets.Count; i++) 
        {
            attackTargets[i].GetComponent<Enemy_Base>().Take_Damage(pdamage, Enemy_Base.DamageType.Magical, false, isCritical);
        }
    }


    public void AttackAOE_Call(GameObject effectObj)
    {
        // 이펙트 출력
        for (int i = 0; i < Player_Manager.instnace.turnManger.enemyObjects.Count; i++)
        {
            Vector3 pos = new Vector3(attackTargets[i].transform.position.x, attackTargets[i].transform.position.y + effectObj.transform.position.y, attackTargets[i].transform.position.z);
            Instantiate(effectObj, pos, Quaternion.identity);
        }
    }

    // 전투 종료 애니메이션 호출 -> 승리/사망 모션 같은거
    public void Turn_End(EndType type)
    {
        StartCoroutine(Turn_EndCall(type));
    }


    // 전투 종료 애니메이션 -> 아직 애니메이션 이후 기능 안만듬!
    private IEnumerator Turn_EndCall(EndType type)
    {
        isEndAnim = true;

        // 애니메이션
        anim.SetTrigger("Action");
        anim.SetBool(type == EndType.Win ? "isWin" : "isDie", true);
        while (anim.GetBool(type == EndType.Win ? "isWin" : "isDie"))
        {
            yield return null;
        }

        isEndAnim = false;
    }
}
