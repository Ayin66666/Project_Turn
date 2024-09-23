using Easing.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // 공격 가중치 동작 부분
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


[System.Serializable] // 이렇게 구현하고 Effexct() 함수를 이벤트로 구독하는 방식으로? 
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


    // 합 공격 설정 -> 이거 몬스터별 턴마다 특수공격까지 고려하면 각 몬스터마다 다르게 만드는게 맞을듯
    public abstract void Turn_AttackSetting();

    // 해당 슬롯으로 누굴 공격할건지 셋팅
    public Attack_Slot Trun_TargetSetting()
    {
        // 이거 공격이 한쪽에 안몰리게 하는 기능도 고려해야하는데...
        int ran = Random.Range(0, Player_Manager.instnace.player_Turn.attackSlot.Count);
        return Player_Manager.instnace.player_Turn.attackSlot[ran];
    }


    // 합 이동 호출
    public void Turn_ExchangeMove(Transform movePos)
    {
        curCoroutine = StartCoroutine(Turn_ExchangeMoveCall(movePos));
    }


    // 합 이동 동작
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


    // 합 계산
    public int DamageCal(int slotCount, int attackCount)
    {
        int damage = Random.Range(attacklist[slotCount].damageValue[attackCount].x, attacklist[attackCount].damageValue[attackCount].y);
        return damage;
    }


    // 합 시작 애니매이션
    public void Turn_ExchangeStartAnim()
    {
        anim.SetTrigger("Exchange");
        anim.SetBool("isExchange", true);
    }


    // 합 결과 애니메이션 호출
    public void Turn_ExchangeResuitAnim(ExchangeResuit type)
    {
        curCoroutine = StartCoroutine(Turn_ExchangeRessuitAnimCall(type));
    }


    // 합 종료 후 승리, 무승부, 패배 애니메이션 동작
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


    // 합 이동 동작
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


    // 몬스터 데미지 기능
    public void TakeDamage(int damage, DamageType type)
    {
        // 사망 체크
        if(isDie)
        {
            return;
        }

        // 데미지 계산
        switch (type)
        {
            case DamageType.physical:
                hp -= damage * physicalDamage;
                break;

            case DamageType.magical:
                hp -= damage * magicalDefense;
                break;
        }

        // 사망 체크
        if (hp <= 0)
        {
            Die();
            return;
        }

        // 피격 애니메이션
        HitAnim();
    }


    // 피격 디버프
    public void HitEffect()
    {

    }


    // 피격 호출
    public abstract void HitAnim();


    // 사망 호출
    public abstract void Die();
}
