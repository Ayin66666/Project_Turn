using Easing.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player_Turn;

public abstract class Enemy_Base : MonoBehaviour
{
    [Header("=== State ===")]
    public bool canAction;
    public bool isEngageMove;
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
    public List<Attack_Base> attackSlot;
    public int curAttackCount;


    [Header("=== Pos Setting ===")]
    [SerializeField] private Transform[] recoilPos;


    [Header("=== Component ===")]
    [SerializeField] protected Animator anim = null;


    public void Turn_EngageMove(Transform movePos)
    {
        curCoroutine = StartCoroutine(Turn_EngageMoveCall(movePos));
    }

    private IEnumerator Turn_EngageMoveCall(Transform movePos)
    {
        isEngageMove = true;

        Vector3 startPos = transform.position;
        Vector3 endPos = movePos.position;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.5f;
            transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        isEngageMove = false;
    }

    // 합 계산
    public int DamageCal(int slotCount, int attackCount)
    {
        int damage = Random.Range(attackSlot[slotCount].damageValue[attackCount].x, attackSlot[attackCount].damageValue[attackCount].y);
        return damage;
    }
    
    // 합 애니메이션
    public void EngageResuit(RecoilType type)
    {
        curCoroutine = StartCoroutine(EngageAnimCall(type));
    }

    // 합 종료 후 승리, 무승부, 패배 애니메이션
    private IEnumerator EngageAnimCall(RecoilType type)
    {
        isRecoilMove = true;

        anim.SetTrigger("Attack");
        anim.SetBool("EngageAnim", true);

        // Delay
        yield return new WaitForSeconds(Random.Range(0.5f, 1f));

        // Recoil Move
        StartCoroutine(RecoilMove(type));

        // Win & Lose Animation
        switch (type)
        {
            case RecoilType.Win:
                anim.SetBool("EngageWin", true);
                while (anim.GetBool("EngageWin"))
                {
                    yield return null;
                }
                break;

            case RecoilType.Draw:
                anim.SetBool("EngageDraw", true);
                while (anim.GetBool("EngageDraw"))
                {
                    yield return null;
                }
                break;

            case RecoilType.Lose:
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

    // 합 이동 기능
    private IEnumerator RecoilMove(RecoilType type)
    {
        isRecoilMove = true;

        // Recoil Move
        Vector3 startPos = transform.position;
        Vector3 endPos = recoilPos[type == RecoilType.Win ? 0 : (type == RecoilType.Draw ? 1 : 2)].position;
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
}
