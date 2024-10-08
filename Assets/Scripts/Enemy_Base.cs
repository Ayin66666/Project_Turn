using Easing.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // ���� ����ġ ���� �κ�
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


[System.Serializable] // �̷��� �����ϰ� Effexct() �Լ��� �̺�Ʈ�� �����ϴ� �������? 
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
    public enum DamageType { physical, magical }
    public enum HitEfftectType { }
    public enum ExchangeResuit { Win, Draw, Lose }

    [Header("=== State ===")]
    public bool isDie;
    public bool canAction;
    public bool isAttack;
    public bool isExchangeMove;
    public bool isRecoilMove;
    protected Coroutine curCoroutine;


    [Header("=== Status ===")]
    public Enemy_Status_SO status;
    public int hp;
    public int physicalDefense;
    public int magicalDefense;

    public int physicalDamage;
    public int magcialDamage;

    public float criticalChance;
    public float criticalMultiplier;

    private Vector2Int slotSpeed;

    [Header("=== Attack Setting ===")]
    public List<Attack_Slot> attack_Slots;
    public List<Attack_Base> attacklist;
    public List<Attack_Pattern> patternList;
    public int curAttackCount;


    [Header("=== Pos Setting ===")]
    [SerializeField] private Transform[] recoilPos;
    public Transform exchangePos;


    [Header("=== Component ===")]
    [SerializeField] protected Enemy_UI enemyUI;
    public TurnFight_Manager turnManager;
    public Animator anim = null;

    #region Property
    public int PhysicalDefense
    {
        get { return physicalDefense; }
        private set { physicalDefense = value; }
    }
    public int MagicDefense
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

        SlotSpeed = status.SlotSpeed;
    }


    // ���� �ӵ� ����
    public void Slot_SpeedSetting()
    {
        for (int i = 0; i < attack_Slots.Count; i++)
        {
            attack_Slots[i].slotSpeed = Random.Range(slotSpeed.x, slotSpeed.y);
        }
    }


    // �� ���� ���� -> �̰� ���ͺ� �ϸ��� Ư�����ݱ��� ����ϸ� �� ���͸��� �ٸ��� ����°� ������
    public abstract void Turn_AttackSetting();


    // �ش� �������� ���� �����Ұ��� ����
    public Attack_Slot Turn_AttackTargetSetting()
    {
        // �̰� ������ ���ʿ� �ȸ����� �ϴ� ��ɵ� ����ؾ��ϴµ�...
        int ran = Random.Range(0, Player_Manager.instnace.player_Turn.attackSlot.Count);
        return Player_Manager.instnace.player_Turn.attackSlot[ran];
    }


    // �� �̵� ȣ��
    public void Turn_ExchangeMove(Transform movePos)
    {
        curCoroutine = StartCoroutine(Turn_ExchangeMoveCall(movePos));
    }


    // �� �̵� ����
    private IEnumerator Turn_ExchangeMoveCall(Transform movePos)
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


    // ���� ȣ��
    public void Turn_Attack(GameObject target, Attack_Base attack, int attackCount)
    {
        attack.UseAttack(Attack_Base.AttackOwner.Player, gameObject, target, attackCount);
    }


    // ������ ��� -> Ʃ�� ��� (1�� �̻��� ���� ��ȯ�� �� / C# 7.0 �̻���� ����!)
    public (int, bool) DamageCal(Attack_Base attack, int count)
    {
        // ������ ���� (���� ������ * ����1 * ����2 ... ) * ���� ���� * ġ��Ÿ ����

        // �� & �� ������ ��ǲ
        int baseDamage = attack.damageType[count] == Attack_Base.DamageType.physical ? physicalDamage : magcialDamage;

        // ���� ���� ���
        float valueDamage = baseDamage * Random.Range(attack.damageValue[count].x, baseDamage * attack.damageValue[count].y);

        // ũ��Ƽ�� ������ ���
        int ran = Random.Range(0, 100);
        valueDamage = ran <= criticalChance ? valueDamage *= criticalChance : valueDamage;

        // ������ & ũ��Ƽ�� ���� ��ȯ
        return ((int)valueDamage, ran <= criticalChance ? true : false);
    }


    // �� ���� �ִϸ��̼�
    public void Turn_ExchangeStartAnim()
    {
        if(anim != null)
        {
            anim.SetTrigger("Exchange");
            anim.SetBool("isExchange", true);
        }
    }


    // �� ��� �ִϸ��̼� ȣ��
    public void Turn_ExchangeResuitAnim(ExchangeResuit type)
    {
        curCoroutine = StartCoroutine(Turn_ExchangeResuitMoveCall(type));
    }


    // �� ��� �ִϸ��̼� ����
    private IEnumerator Turn_ExchangeResuitAnimCall(ExchangeResuit type)
    {
        isRecoilMove = true;

        // �� �� �и� �̵� ����
        StartCoroutine(Turn_ExchangeResuitMoveCall(type));

        // �� �ִϸ��̼�
        if (anim != null)
        {
            anim.SetBool("isExchange", false);
            switch (type)
            {
                case ExchangeResuit.Win:
                    anim.SetBool("isExchangeWin", true);
                    while (anim.GetBool("isExchangeWin"))
                    {
                        yield return null;
                    }
                    break;

                case ExchangeResuit.Draw:
                    anim.SetBool("isExchangeDraw", true);
                    while (anim.GetBool("isExchangeDraw"))
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


    // �� �и� ���� �ٽ� �� ��ġ�� �̵� ȣ��
    public void Turn_ExchangeMove_After(Vector3 pos)
    {
        StartCoroutine(Turn_ExchangeMoveAfterCall(pos));
    }


    // �� �и� ���� �ٽ� �� ��ġ�� �̵� ����
    private IEnumerator Turn_ExchangeMoveAfterCall(Vector3 pos)
    {
        isExchangeMove = true;

        Vector3 startPos = transform.position;
        Vector3 endPos = pos;

        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 4f;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        isExchangeMove = false;
    }


    // �� �̵� ����
    private IEnumerator Turn_ExchangeResuitMoveCall(ExchangeResuit type)
    {
        isRecoilMove = true;

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

        // Delay
        yield return new WaitForSeconds(Random.Range(0.15f, 0.25f));

        isRecoilMove = false;
    }


    // ���� ������ ���
    public void TakeDamage(int damage, DamageType type)
    {
        // ��� üũ
        if(isDie)
        {
            return;
        }

        // ������ ���
        switch (type)
        {
            case DamageType.physical:
                hp -= damage * physicalDamage;
                break;

            case DamageType.magical:
                hp -= damage * magicalDefense;
                break;
        }

        // ��� üũ
        if (hp <= 0)
        {
            Die();
            return;
        }

        // �ǰ� �ִϸ��̼�
        HitAnim();
    }


    // �ǰ� �����
    public void HitEffect()
    {

    }


    // �ǰ� ȣ��
    public abstract void HitAnim();


    // ��� ȣ��
    public abstract void Die();
}
