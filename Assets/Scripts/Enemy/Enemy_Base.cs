using Easing.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;


[System.Serializable] // ���� ���� üũ
public class Attack_Pattern
{
    // �̰� ��ų �̸� ������ string ���� enum�� �����ؼ� �����ϰ� �� ����!
    public string name;
    public int Weight;
    public int current_Weight;
    public int enumValue;
    public Vector2Int minMax_Value;

    // ����ġ ���� ��� -> ���⼭ �ִ� �ּ�ġ�� ���ѵδ� ���� ��!
    public void Weight_Setting(int value)
    {
        current_Weight += value;
        current_Weight = Mathf.Clamp(current_Weight, minMax_Value.x, minMax_Value.y);
    }
}


[System.Serializable] // ���� ü�¸��� ���̾�α� ȣ��
public class EnemyDialog
{
    public int dialog_hp;
    public int dialog_Index;
}


[System.Serializable]
public class Drop_Item // ������ ���
{
    public Item_Base item;
    public int drop_Probability;
    public Vector2Int dropCount; // -> ���� �̻��
}


[System.Serializable] // �̷��� �����ϰ� Effexct() �Լ��� �̺�Ʈ�� �����ϴ� �������? -> ���� �̻��
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
    public List<Attack_Pattern> patternList_Phase2; // �̰� ���� ����
    protected VideoClip phase2Clip;
    public bool havePhase2;
    public bool isPhase2;
    public int phase2Hp;


    [Header("=== Pos Setting ===")]
    [SerializeField] private Transform[] recoilPos;
    public Transform exchangePos;


    [Header("=== LookAt Setting ===")]
    public GameObject forwardTarget; // ���͸� �ֽ����� ���� �� �ٶ� ���
    [SerializeField] private GameObject looktarget; // �ֽ��� ���


    [Header("=== Component ===")]
    public Enemy_UI enemyUI;
    public Animator anim = null;
    public TurnFight_Manager turnManager;
    [SerializeField] private GameObject body;
    [SerializeField] protected CharacterController controller;
    [SerializeField] private GameObject damageObj;
    [SerializeField] private Collider damageSpawnPosCollider;


    [Header("===������ ����Ʈ===")]
    [SerializeField] private GameObject hitVFX;
    [SerializeField] private Collider hitVFXCollier;


    [Header("=== Sound Setting ===")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] clips;


    [Header("=== ������ ��� ===")]
    [SerializeField] protected bool haveItemDrop;
    [SerializeField] private List<Drop_Item> dropItemList;


    // ���� ������
    [Header("=== Attack ===")]
    [SerializeField] private GameObject attackTarget;
    [SerializeField] protected Attack_Base myAttack;
    [SerializeField] protected int curAttackCount;
    [SerializeField] private int maxAttackCount;
    [SerializeField] private bool isExchange;


    [Header("=== �ִϸ��̼� ===")]
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


    // �������ͽ� ����
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


    // ���� �ӵ� ����
    public void Slot_SpeedSetting()
    {
        for (int i = 0; i < attack_Slots.Count; i++)
        {
            attack_Slots[i].Speed_Setting(Random.Range(slotSpeed.x, slotSpeed.y));
        }
    }


    // ��� ���̺� ����
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


    // �� ���� ���� -> �̰� ���ͺ� �ϸ��� Ư�����ݱ��� ����ϸ� �� ���͸��� �ٸ��� ����°� ������
    public abstract void Turn_AttackSetting(GameObject obj);


    // �ش� �������� ���� �����Ұ��� ����
    public Attack_Slot Turn_AttackTargetSetting()
    {
        // �̰� ������ ���ʿ� �ȸ����� �ϴ� ��ɵ� ����ؾ��ϴµ�...
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
    /// ���� ��� �߰� ���� ������ ���� ���
    /// </summary>
    /// <param name="attackData">���ʹ� ���� ������</param>
    public (int,int,int) Turn_WinningCheck(Attack_Base attackData)
    {
        // �ּ� ������ ���
        int minDamage =
            (int)(attackData.damageValue[0].x *
            (attackData.damageType[0] == Attack_Base.DamageType.physical ? physicalDamage : magcialDamage));

        // �ִ� ������ ���
        int maxDamage =
            (int)(attackData.damageValue[^1].y *
            (attackData.damageType[^1] == Attack_Base.DamageType.physical ? physicalDamage : magcialDamage));

        // �߰� ������ ���
        int mmDamage = (int)Mathf.Lerp(minDamage, maxDamage, 0.5f);

        return (mmDamage, minDamage, maxDamage);
    }

    // �ٶ󺸱� ��� ����
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


    // ���� �� Ÿ�� ��ġ�� �̵�
    public void Turn_AttackMove()
    {
        StartCoroutine(Turn_AttackMoveCall());
    }

    private IEnumerator Turn_AttackMoveCall()
    {
        isExchangeMove = true;

        // �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isAttackMove", true);
        anim.SetBool("isExchangeMoveReady", true);

        // �̵� �غ� �ִϸ��̼� ���
        while (anim.GetBool("isExchangeMoveReady"))
        {
            yield return null;
        }

        // �̵�
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


    // �Ϲ� ���� �̵� ȣ��
    public void Turn_OneSideMove(Transform movePos, float moveSpeed)
    {
        curCoroutine = StartCoroutine(Turn_OnesideMoveCall(movePos, moveSpeed));
    }

    // �Ϲ� ���� �̵� ����
    private IEnumerator Turn_OnesideMoveCall(Transform movePos, float moveSpeed)
    {
        isExchangeMove = true;

        // �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isExchangeMove", true);
        anim.SetBool("isExchangeMoveReady", true);

        // �̵� �غ� ���� ���
        while (anim.GetBool("isExchangeMoveReady"))
        {
            yield return null;
        }

        // �̵� �ڵ�
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


    // �� �̵� ȣ��
    public void Turn_ExchangeMove(Transform movePos, float moveSpeed)
    {
        curCoroutine = StartCoroutine(Turn_EngageMoveCall(movePos, moveSpeed));
    }


    // �� �̵� ����
    private IEnumerator Turn_EngageMoveCall(Transform movePos, float moveSpeed)
    {
        isExchangeMove = true;

        // �ִϸ��̼�
        anim.SetTrigger("Action");
        anim.SetBool("isExchangeMove", true);
        anim.SetBool("isExchangeMoveReady", true);

        // �̵� �غ� ���� ���
        while (anim.GetBool("isExchangeMoveReady"))
        {
            yield return null;
        }

        // �̵� �ڵ�
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


    // �� ���� �� ���� �̵� ȣ��
    public void Turn_ExchangeEndMove(Transform movePos, float moveSpeed)
    {
        StartCoroutine(Turn_ExchangeEndMoveCall(movePos, moveSpeed));
    }


    // �� ���� �� ���� �̵� ����
    private IEnumerator Turn_ExchangeEndMoveCall(Transform movePos, float moveSpeed)
    {
        isExchangeMove = true;

        // �ִϸ��̼�
        if (anim != null)
        {
            anim.SetTrigger("Action");
            anim.SetBool("isBackstep", true);
        }

        // �̵�
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


    // ���� ��� ȣ�� & ���� ������ �Է�
    /// <summary>
    /// 
    /// </summary>
    /// <param name="target">�����ϴ� ���</param>
    /// <param name="attack">���� ������</param>
    /// <param name="isExchange">�� ����</param>
    /// <param name="attackCount">���� ���� Ƚ��</param>
    public void Turn_AttackData_Setting(GameObject target, Attack_Base attack, bool isExchange, int attackCount)
    {
        isAttack = true;

        // ������ ����
        attackTarget = target;
        myAttack = attack;
        curAttackCount = 0;
        this.isExchange = isExchange;

        // ���� UI ȣ��
        enemyUI.TurnFight_ExchanageSummary(true, myAttack);

        // ���� ȣ��
        myAttack.UseAttack(Attack_Base.AttackOwner.Enemy, gameObject, target, isExchange, attackCount);
    }


    // �ִϸ��̼� �̺�Ʈ���� ȣ���ϴ� �κ� - ���� �����͸� ����ؼ� ������ ������
    public void Attack_Call()
    {
        // ������ ���
        (int pdamage, bool isCritical) = DamageCal(myAttack, curAttackCount);

        // ������ UI -> �������
        enemyUI.TurnFight_ExchangeSummary_Damage(true, pdamage, curAttackCount, myAttack.attackCost, myAttack);

        // ������ ����
        Player_Manager.instnace.Take_Damage(pdamage, myAttack.damageType[curAttackCount] == Attack_Base.DamageType.physical ? Player_Manager.DamageType.Physical : Player_Manager.DamageType.Magical, isCritical);

        // ���� Ƚ�� ����
        curAttackCount++;
    }


    public (int, bool) DamageCal(Attack_Base attack, int count)
    {
        // ������ ���� (���� ������ * ����1 * ����2 ... ) * ���� ���� * ġ��Ÿ ����
        Debug.Log("���� ��� ȣ�� / ȣ�� ī��Ʈ : " + count + " / ���� ���� : " + attack);
        // �� & �� ������ ��ǲ
        int baseDamage = attack.damageType[count] == Attack_Base.DamageType.physical ? physicalDamage : magcialDamage;

        // ���� ���


        // ���� ���� ���
        float valueDamage = baseDamage * Random.Range(attack.damageValue[count].x, attack.damageValue[count].y);

        // ũ��Ƽ�� ������ ���
        int ran = Random.Range(0, 100);
        valueDamage = ran <= criticalChance ? valueDamage *= criticalMultiplier : valueDamage;

        Debug.Log("���� ������ : " + (int)valueDamage);

        // ������ & ũ��Ƽ�� ���� ��ȯ
        return ((int)valueDamage, ran <= criticalChance ? true : false);
    }


    // �� ��� �ִϸ��̼� ȣ��
    public void Turn_ExchangeResuitAnim(ExchangeResuit type)
    {
        curCoroutine = StartCoroutine(Turn_ExchangeResuitAnimCall(type));
    }


    // �� ��� �ִϸ��̼� ����
    private IEnumerator Turn_ExchangeResuitAnimCall(ExchangeResuit type)
    {
        isRecoilMove = true;

        // �� �� �и� �̵� ����
        StartCoroutine(Turn_ExchanageResuitMove(type));

        // �� �ִϸ��̼�
        if(anim != null)
        {
            // �̵� �ִϸ��̼� ����
            anim.SetBool("isExchangeMove", false);

            // �� ��� �ִϸ��̼� ȣ��
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


    // �� �и� �̵� ����
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



    // ���� ������ ���
    public void Take_Damage(int damage, DamageType type, bool isExchange, bool isCritical)
    {
        // ��� üũ
        if(isDie)
        {
            return;
        }

        // ī�޶� ��鸲 ȿ��
        Camera_Manager.instance.Turn_CameraShack(5f, 0.1f);

        // �� ��鸲 ȿ��
        Hit_Shaking();

        // �ǰ� ����Ʈ
        HitVFX();

        // �ǰ� ����
        SoundCall(1);

        // �ǰ� �ƿ����� ȿ��
        Player_UI.instance.TurnFight_Ouutline_SpeedUp();

        // ������ 2 ������ �浶 + ���� ���� ���� �� ��� ������ ��ȿ
        if (havePhase2 && !isPhase2 && hp <= phase2Hp)
        {
            return;
        }

        // ������ ���
        int calDamage = type == DamageType.Physical ? (int)(damage * physicalDefense) : (int)(damage * magicalDefense);
        Turn_HitDamageUI(isCritical, calDamage);
        hp -= calDamage;

        // ü�� UI �ֽ�ȭ
        enemyUI.HpBar();

        // �÷��̾� �ʻ�� ������ ����
        if (isExchange)
        {
            Player_Manager.instnace.UnderSideGaugeAdd((int)(0.25f * calDamage));
        }

        // ��� üũ
        if (hp <= 0 && !isDie)
        {
            Die();

            // ������ ��� üũ
            if (haveItemDrop)
            {
                Item_Drop();
            }
        }
        else
        {
            // �ǰ� �ִϸ��̼�
            HitAnim();
        }

        // ������ 2 ��ȯ üũ
        if (havePhase2)
        {
            if(hp <= phase2Hp && !isPhase2)
            {
                turnManager.Phase2();
            }
        }
    }

    /// <summary>
    /// �ǰ� ����Ʈ
    /// </summary>
    /// <param name="type"></param>
    private void HitVFX()
    {
        Instantiate(hitVFX, HitVFXPos(), Quaternion.identity);
    }

    // �ǰ� ����Ʈ ��ȯ ��ġ ���
    private Vector3 HitVFXPos()
    {
        Vector3 originPosition = hitVFXCollier.transform.position;

        // �ݶ��̴��� ����� �������� bound.size ���
        float range_X = hitVFXCollier.bounds.size.x;
        float range_Y = hitVFXCollier.bounds.size.y;

        range_X = Random.Range((range_X / 2) * -1, range_X / 2);
        range_Y = Random.Range((range_Y / 2) * -1, range_Y / 2);
        Vector3 RandomPostion = new Vector3(range_X, range_Y);

        Vector3 respawnPosition = originPosition + RandomPostion;
        return respawnPosition;
    }

    /// <summary>
    /// �ǰ� �� ĳ���� ���� ��鸲 ����Ʈ
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

    // ���̾�α� üũ ���
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


    // �ǰ� ������ UI
    public void Turn_HitDamageUI(bool isCritical, int damage)
    {
        GameObject obj = Instantiate(damageObj, DamageUIPos(), Quaternion.identity);
        obj.GetComponent<Damage_UI>().DamageUI_Setting(isCritical ? Damage_UI.DamageType.Critical : Damage_UI.DamageType.Normal, damage);
    }

    // �ǰ� ������ UI ��ȯ ��ġ ����
    private Vector3 DamageUIPos()
    {
        Vector3 originPosition = damageSpawnPosCollider.transform.position;

        // �ݶ��̴��� ����� �������� bound.size ���
        float range_X = damageSpawnPosCollider.bounds.size.x;
        float range_Y = damageSpawnPosCollider.bounds.size.y;

        range_X = Random.Range((range_X / 2) * -1, range_X / 2);
        range_Y = Random.Range((range_Y / 2) * -1, range_Y / 2);
        Vector3 RandomPostion = new Vector3(range_X, range_Y);

        Vector3 respawnPosition = originPosition + RandomPostion;
        return respawnPosition;
    }


    // ��� ������ ���
    protected void Item_Drop()
    {
        // ����ġ ���� ���
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
                // ������ ������ ��� 1���� ������!
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


    // ���� ȣ��
    public abstract void Spawn();


    // �ǰ� �����
    public void HitEffect()
    {

    }


    // �ǰ� ȣ��
    public abstract void HitAnim();


    // 2������
    public abstract void Phase2();


    // ��� ȣ��
    public abstract void Die();
}