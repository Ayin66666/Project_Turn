using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing.Tweening;
using UnityEngine.UIElements;


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
    public bool isEngageMove;
    public bool isRecoilMove;

    public enum RecoilType { Win, Draw, Lose }

    [Header("=== Attack Setting ===")]
    public List<Attack_Slot> attackSlot;


    [Header("=== Pos Setting ===")]
    [SerializeField] private Transform[] recoilPos;


    [Header("=== Component ===")]
    [SerializeField] private Animator anim;
    private Coroutine curCoroutine;


    // 맨 처음 전투 시작할 때 호출 -> 등장 모션 같은거
    public void Turn_StartAnim()
    {
        curCoroutine = StartCoroutine(StartAnimCall());
    }

    private IEnumerator StartAnimCall()
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

    // 공격 선택
    public void Turn_Select()
    {
        isSelect = true;
        
        // 슬롯 정리 -> 남아있는 공격 있을 경우 대비
        for (int i = 0; i < attackSlot.Count; i++)
        {
            attackSlot[i].ResetSlot();
        }

        // 선택 UI On
        Player_UI.instance.TurnFight_Select(true);
    }

    // 합 이동
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

    // 맨 마지막 전투 종료 할 때 호출 -> 승리/사망 모션 같은거
    public void Turn_End()
    {
        StartCoroutine(Turn_EndCall());
    }

    // 애니메이션 -> 아직 애니메이션 이후 기능 안만듬!
    private IEnumerator Turn_EndCall()
    {
        anim.SetTrigger("TurnEnd");
        anim.SetBool("isTurnEndAnim", true);
        while(anim.GetBool("isTurnEndAnim"))
        {
            yield return null;
        }
    }
}
