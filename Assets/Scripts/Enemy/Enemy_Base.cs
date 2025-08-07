using Easing.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;


[System.Serializable] // 공격 패턴 체크
public class Attack_Pattern
{
    // 이거 스킬 이름 적으면 string 값을 enum과 대조해서 동작하게 할 예정!
    public string name;
    public int Weight;
    public int current_Weight;
    public int enumValue;
    public Vector2Int minMax_Value;

    // 가중치 조절 기능 -> 여기서 최대 최소치를 제한두는 값도 둠!
    public void Weight_Setting(int value)
    {
        current_Weight += value;
        current_Weight = Mathf.Clamp(current_Weight, minMax_Value.x, minMax_Value.y);
    }
}


[System.Serializable] // 일정 체력마다 다이얼로그 호출
public class EnemyDialog
{
    public int dialog_hp;
    public int dialog_Index;
}


[System.Serializable]
public class Drop_Item // 아이템 드랍
{
    public Item_Base item;
    public int drop_Probability;
    public Vector2Int dropCount; // -> 현재 미사용
}


[System.Serializable] // 이렇게 구현하고 Effexct() 함수를 이벤트로 구독하는 방식으로? -> 현재 미사용
public class Hit_Effect
{
    public EffectType type;
    public int startTurn;
    public int endTurn;

    public enum EffectType {a, b, c, d }
    public void Effect()
    {

    }
}


public abstract class Enemy_Base : MonoBehaviour
{
    public enum DamageType { Physical, Magical }
    public enum HitEfftectType { }
    public enum ExchangeResuit { Win, Draw, Lose }
    public enum EnemyType { Normal, Elite, Boss }


    [Header("=== State ===")]
    public EnemyType enemyType;
    public bool isSpawn;
    public bool isDie;
    public bool isAttack;
    public bool isExchangeMove;
    public bool isRecoilMove;
    public bool canAction;
    public bool isCutscene;
    protected Coroutine curCoroutine;
    private Coroutine lookCoroutine;
    private Coroutine hitMoveCoroutine;


    [Header("=== Status ===")]
    public Enemy_Status_SO status;
    public int hp;
    public float physicalDefense;
    public float magicalDefense;
    public int physicalDamage;
    public int magcialDamage;
    public float criticalChance;
    public float criticalMultiplier;
    private Vector2Int slotSpeed;


    [Header("=== Attack Setting ===")]
    public List<Attack_Slot> attack_Slots;
    public List<Attack_Base> attacklist;
    public List<Attack_Pattern> patternList;


    [Header("=== Dialog ===")]
    public bool haveDialog;
    [SerializeField] protected bool haveStartDialog;
    [SerializeField] protected bool haveEndDialog;
    [SerializeField] private List<EnemyDialog> dialogData;
    protected Coroutine dialogCheckCoroutine;


    [Header("=== Phase 2 ===")]
    public List<Attack_Pattern> patternList_Phase2; // 이건 보스 전용
    protected VideoClip phase2Clip;
    public bool havePhase2;
    public bool isPhase2;
    public int phase2Hp;


    [Header("=== Pos Setting ===")]
    [SerializeField] private Transform[] recoilPos;
    public Transform exchangePos;


    [Header("=== LookAt Setting ===")]
    public GameObject forwardTarget; // 몬스터를 주시하지 않을 때 바라볼 대상
    [SerializeField] private GameObject looktarget; // 주시할 대상


    [Header("=== Component ===")]
    public Enemy_UI enemyUI;
    public Animator anim = null;
    public TurnFight_Manager turnManager;
    [SerializeField] private GameObject body;
    [SerializeField] protected CharacterController controller;
    [SerializeField] private GameObject damageObj;
    [SerializeField] private Collider damageSpawnPosCollider;


    [Header("===데미지 이펙트===")]
    [SerializeField] private GameObject hitVFX;
    [SerializeField] private Collider hitVFXCollier;


    [Header("=== Sound Setting ===")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] clips;


    [Header("=== 아이템 드랍 ===")]
    [SerializeField] protected bool haveItemDrop;
    [SerializeField] private List<Drop_Item> dropItemList;


    // 공격 데이터
    [Header("=== Attack ===")]
    [SerializeField] private GameObject attackTarget;
    [SerializeField] protected Attack_Base myAttack;
    [SerializeField] protected int curAttackCount;
    [SerializeField] private int maxAttackCount;
    [SerializeField] private bool isExchange;


    [Header("=== 애니메이션 ===")]
    [SerializeField] private string[] animTrigger;
    [SerializeField] private string[] animBool;


    #region Property
    public float PhysicalDefense
    {
        get { return physicalDefense; }
        private set { physicalDefense = value; }
    }
    public float MagicDefense
    {
        get { return magicalDefense; }
        private set { magicalDefense = value; }
    }

    public int PhysicalDamage
    {
        get { return physicalDamage; }
        private set { physicalDamage = value; }
    }
    public int MagcialDamage
    {
        get { return magcialDamage; }
        private set { magcialDamage = value; }
    }

    public float CriticalChance
    {
        get { return criticalChance; }
        private set { criticalChance = value; }
    }
    public float CriticalMultiplier
    {
        get { return criticalMultiplier; }
        private set { criticalMultiplier = value; }
    }

    public Vector2Int SlotSpeed
    {
        get { return slotSpeed; }
        private set { slotSpeed = value; }
    }

    #endregion


    // 스테이터스 셋팅
    protected void Status_Setting()
    {
        hp = status.Hp;

        physicalDefense = status.PhysicalDefense;
        magicalDefense = status.MagicalDefense;

        physicalDamage = status.PhysicalDamage;
        magcialDamage = status.MagcialDamage;

        criticalChance = status.CriticalChance;
        CriticalMultiplier = status.CriticalMultiplier;

        SlotSpeed = status.SlotSpeed;
    }


    // 슬롯 속도 셋팅
    public void Slot_SpeedSetting()
    {
        for (int i = 0; i < attack_Slots.Count; i++)
        {
            attack_Slots[i].Speed_Setting(Random.Range(slotSpeed.x, slotSpeed.y));
        }
    }


    // 드랍 테이블 셋팅
    public void Drop_Table_Setting()
    {
        Inventory inventory;
        inventory = GameObject.Find("Inventory_Manager").GetComponent<Inventory>();
        for (int i = 0; i < dropItemList.Count; i++)
        {
            dropItemList[i].item = inventory.items[i];
        }
    }

    public void SoundCall(int index)
    {
        audioSource.PlayOneShot(clips[index]);
    }


    // 합 공격 설정 -> 이거 몬스터별 턴마다 특수공격까지 고려하면 각 몬스터마다 다르게 만드는게 맞을듯
    public abstract void Turn_AttackSetting(GameObject obj);


    // 해당 슬롯으로 누굴 공격할건지 셋팅
    public Attack_Slot Turn_AttackTargetSetting()
    {
        // 이거 공격이 한쪽에 안몰리게 하는 기능도 고려해야하는데...
        int ran = Random.Range(0, Player_Manager.instnace.player_Turn.attackSlot.Count);
        return Player_Manager.instnace.player_Turn.attackSlot[ran];
    }


    public void Animation_Reset()
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

    /// <summary>
    /// 공격 계수 중간 기준 데미지 산출 기능
    /// </summary>
    /// <param name="attackData">에너미 공격 데이터</param>
    public (int,int,int) Turn_WinningCheck(Attack_Base attackData)
    {
        // 최소 데미지 계산
        int minDamage =
            (int)(attackData.damageValue[0].x *
            (attackData.damageType[0] == Attack_Base.DamageType.physical ? physicalDamage : magcialDamage));

        // 최대 데미지 계산
        int maxDamage =
            (int)(attackData.damageValue[^1].y *
            (attackData.damageType[^1] == Attack_Base.DamageType.physical ? physicalDamage : magcialDamage));

        // 중간 데미지 계산
        int mmDamage = (int)Mathf.Lerp(minDamage, maxDamage, 0.5f);

        return (mmDamage, minDamage, maxDamage);
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
        while (timer < 1)
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
        while (anim.GetBool("isExchangeMoveReady"))
        {
            yield return null;
        }

        // 이동
        Vector3 startPos = transform.position;
        Vector3 endPos = Player_Manager.instnace.player_Turn.exchangePos.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 2.5f;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        anim.SetBool("isAttackMove", false);

        yield return new WaitForSeconds(0.05f);
        isExchangeMove = false;
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

        // 애니메이션
        if (anim != null)
        {
            anim.SetTrigger("Action");
            anim.SetBool("isBackstep", true);
        }

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

        anim.SetBool("isBackstep", false);
        isExchangeMove = false;
    }


    // 공격 기능 호출 & 공격 데이터 입력
    /// <summary>
    /// 
    /// </summary>
    /// <param name="target">공격하는 대상</param>
    /// <param name="attack">공격 데이터</param>
    /// <param name="isExchange">합 여부</param>
    /// <param name="attackCount">남은 공격 횟수</param>
    public void Turn_AttackData_Setting(GameObject target, Attack_Base attack, bool isExchange, int attackCount)
    {
        isAttack = true;

        // 데이터 셋팅
        attackTarget = target;
        myAttack = attack;
        curAttackCount = 0;
        this.isExchange = isExchange;

        // 공격 UI 호출
        enemyUI.TurnFight_ExchanageSummary(true, myAttack);

        // 공격 호출
        myAttack.UseAttack(Attack_Base.AttackOwner.Enemy, gameObject, target, isExchange, attackCount);
    }


    // 애니메이션 이벤트에서 호출하는 부분 - 공격 데이터를 사용해서 적에게 데미지
    public void Attack_Call()
    {
        // 데미지 계산
        (int pdamage, bool isCritical) = DamageCal(myAttack, curAttackCount);

        // 데미지 UI -> 변경버전
        enemyUI.TurnFight_ExchangeSummary_Damage(true, pdamage, curAttackCount, myAttack.attackCost, myAttack);

        // 데미지 전달
        Player_Manager.instnace.Take_Damage(pdamage, myAttack.damageType[curAttackCount] == Attack_Base.DamageType.physical ? Player_Manager.DamageType.Physical : Player_Manager.DamageType.Magical, isCritical);

        // 공격 횟수 증가
        curAttackCount++;
    }


    public (int, bool) DamageCal(Attack_Base attack, int count)
    {
        // 데미지 공식 (기초 데미지 * 버프1 * 버프2 ... ) * 공격 배율 * 치명타 배율
        Debug.Log("공격 계산 호출 / 호출 카운트 : " + count + " / 공격 종류 : " + attack);
        // 물 & 마 데미지 인풋
        int baseDamage = attack.damageType[count] == Attack_Base.DamageType.physical ? physicalDamage : magcialDamage;

        // 버프 계산


        // 공격 배율 계산
        float valueDamage = baseDamage * Random.Range(attack.damageValue[count].x, attack.damageValue[count].y);

        // 크리티컬 데미지 계산
        int ran = Random.Range(0, 100);
        valueDamage = ran <= criticalChance ? valueDamage *= criticalMultiplier : valueDamage;

        Debug.Log("계산된 데미지 : " + (int)valueDamage);

        // 데미지 & 크리티컬 여부 반환
        return ((int)valueDamage, ran <= criticalChance ? true : false);
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
        Vector3 endPos = recoilPos[type == ExchangeResuit.Win ? 0 : (type == ExchangeResuit.Draw ? 1 : 2)].position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.5f;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
    }



    // 몬스터 데미지 기능
    public void Take_Damage(int damage, DamageType type, bool isExchange, bool isCritical)
    {
        // 사망 체크
        if(isDie)
        {
            return;
        }

        // 카메라 흔들림 효과
        Camera_Manager.instance.Turn_CameraShack(5f, 0.1f);

        // 몸 흔들림 효과
        Hit_Shaking();

        // 피격 이펙트
        HitVFX();

        // 피격 사운드
        SoundCall(1);

        // 피격 아웃라인 효과
        Player_UI.instance.TurnFight_Ouutline_SpeedUp();

        // 페이즈 2 조건이 충독 + 아직 변신 안함 의 경우 데미지 무효
        if (havePhase2 && !isPhase2 && hp <= phase2Hp)
        {
            return;
        }

        // 데미지 계산
        int calDamage = type == DamageType.Physical ? (int)(damage * physicalDefense) : (int)(damage * magicalDefense);
        Turn_HitDamageUI(isCritical, calDamage);
        hp -= calDamage;

        // 체력 UI 최신화
        enemyUI.HpBar();

        // 플레이어 필살기 게이지 충전
        if (isExchange)
        {
            Player_Manager.instnace.UnderSideGaugeAdd((int)(0.25f * calDamage));
        }

        // 사망 체크
        if (hp <= 0 && !isDie)
        {
            Die();

            // 아이템 드랍 체크
            if (haveItemDrop)
            {
                Item_Drop();
            }
        }
        else
        {
            // 피격 애니메이션
            HitAnim();
        }

        // 페이즈 2 변환 체크
        if (havePhase2)
        {
            if(hp <= phase2Hp && !isPhase2)
            {
                turnManager.Phase2();
            }
        }
    }

    /// <summary>
    /// 피격 이펙트
    /// </summary>
    /// <param name="type"></param>
    private void HitVFX()
    {
        Instantiate(hitVFX, HitVFXPos(), Quaternion.identity);
    }

    // 피격 이펙트 소환 위치 계산
    private Vector3 HitVFXPos()
    {
        Vector3 originPosition = hitVFXCollier.transform.position;

        // 콜라이더의 사이즈를 가져오는 bound.size 사용
        float range_X = hitVFXCollier.bounds.size.x;
        float range_Y = hitVFXCollier.bounds.size.y;

        range_X = Random.Range((range_X / 2) * -1, range_X / 2);
        range_Y = Random.Range((range_Y / 2) * -1, range_Y / 2);
        Vector3 RandomPostion = new Vector3(range_X, range_Y);

        Vector3 respawnPosition = originPosition + RandomPostion;
        return respawnPosition;
    }

    /// <summary>
    /// 피격 시 캐릭터 몸통 흔들림 이펙트
    /// </summary>
    private void Hit_Shaking()
    {
        if (hitMoveCoroutine != null) StopCoroutine(hitMoveCoroutine);
        hitMoveCoroutine = StartCoroutine(HitShakingCall());
    }

    private IEnumerator HitShakingCall()
    {
        Vector3 originalPosition = body.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < 0.15f)
        {
            float xOffset = Random.Range(-1f, 1f) * 0.25f;
            float zOffset = Random.Range(-1f, 1f) * 0.25f;
            body.transform.localPosition = originalPosition + new Vector3(xOffset, 0, zOffset);

            elapsed += Time.deltaTime;
            yield return null;
        }

        body.transform.localPosition = originalPosition;
    }

    // 다이얼로그 체크 기능
    public IEnumerator Dialog_Check()
    {
        while(true)
        {
            for (int i = 0; i < dialogData.Count; i++)
            {
                if (hp <= dialogData[i].dialog_hp)
                {
                    // Dailog Call
                    Dialog_Manager.instance.Dialog(dialogData[i].dialog_Index);

                    // Remove List
                    dialogData.Remove(dialogData[i]);
                }
            }

            yield return new WaitForSeconds(1.5f);
        }
    }


    // 피격 데미지 UI
    public void Turn_HitDamageUI(bool isCritical, int damage)
    {
        GameObject obj = Instantiate(damageObj, DamageUIPos(), Quaternion.identity);
        obj.GetComponent<Damage_UI>().DamageUI_Setting(isCritical ? Damage_UI.DamageType.Critical : Damage_UI.DamageType.Normal, damage);
    }

    // 피격 데미지 UI 소환 위치 셋팅
    private Vector3 DamageUIPos()
    {
        Vector3 originPosition = damageSpawnPosCollider.transform.position;

        // 콜라이더의 사이즈를 가져오는 bound.size 사용
        float range_X = damageSpawnPosCollider.bounds.size.x;
        float range_Y = damageSpawnPosCollider.bounds.size.y;

        range_X = Random.Range((range_X / 2) * -1, range_X / 2);
        range_Y = Random.Range((range_Y / 2) * -1, range_Y / 2);
        Vector3 RandomPostion = new Vector3(range_X, range_Y);

        Vector3 respawnPosition = originPosition + RandomPostion;
        return respawnPosition;
    }


    // 사망 아이템 드랍
    protected void Item_Drop()
    {
        // 가중치 랜덤 계산
        int total = 0;
        foreach (var item in dropItemList)
        {
            total += item.drop_Probability;
        }

        int ran = Random.Range(0, total);
        foreach (var item in dropItemList)
        {
            if (ran <= item.drop_Probability)
            {
                // 지금은 아이템 드랍 1개로 고정임!
                Player_UI.instance.ItemDropUI(item.item, 1);
                Inventory.instance.SetItem(item.item);
                break;
            }
            else
            {
                ran -= item.drop_Probability;
            }
        }
    }


    // 스폰 호출
    public abstract void Spawn();


    // 피격 디버프
    public void HitEffect()
    {

    }


    // 피격 호출
    public abstract void HitAnim();


    // 2페이즈
    public abstract void Phase2();


    // 사망 호출
    public abstract void Die();
}