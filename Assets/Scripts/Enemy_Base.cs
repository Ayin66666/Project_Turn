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


    [Header("=== Attack Setting ===")]
    public List<Attack_Slot> attack_Slots;
    public List<Attack_Base> attacklist;
    public List<Attack_Pattern> patternList;
    public int curAttackCount;


    [Header("=== Pos Setting ===")]
    [SerializeField] private Transform[] recoilPos;


    [Header("=== Component ===")]
    [SerializeField] protected Enemy_UI enemyUI;
    [SerializeField] protected Animator anim = null;


    // �� ���� ���� -> �̰� ���ͺ� �ϸ��� Ư�����ݱ��� ����ϸ� �� ���͸��� �ٸ��� ����°� ������
    public abstract void Turn_AttackSetting();

    // �ش� �������� ���� �����Ұ��� ����
    public Attack_Slot Trun_TargetSetting()
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


    // �� ���
    public int DamageCal(int slotCount, int attackCount)
    {
        int damage = Random.Range(attacklist[slotCount].damageValue[attackCount].x, attacklist[attackCount].damageValue[attackCount].y);
        return damage;
    }


    // �� ���� �ִϸ��̼�
    public void Turn_ExchangeStartAnim()
    {
        anim.SetTrigger("Exchange");
        anim.SetBool("isExchange", true);
    }


    // �� ��� �ִϸ��̼� ȣ��
    public void Turn_ExchangeResuitAnim(ExchangeResuit type)
    {
        curCoroutine = StartCoroutine(Turn_ExchangeRessuitAnimCall(type));
    }


    // �� ���� �� �¸�, ���º�, �й� �ִϸ��̼� ����
    private IEnumerator Turn_ExchangeRessuitAnimCall(ExchangeResuit type)
    {
        isRecoilMove = true;

        anim.SetTrigger("Attack");
        anim.SetBool("EngageAnim", true);

        // Delay
        yield return new WaitForSeconds(Random.Range(0.5f, 1f));

        // Recoil Move
        StartCoroutine(Turn_ExchangeResuitMoveCall(type));

        // Win & Lose Animation
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

        // Delay
        yield return new WaitForSeconds(Random.Range(0.25f, 0.55f));

        isRecoilMove = false;
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
