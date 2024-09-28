using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Easing.Tweening;


public class Attack_Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("=== Slot Setting ===")]
    public SlotType slotType;
    public AttackType attackType;
    public Attack_Base myAttack;
    public Attack_Slot targetSlot;
    public GameObject slotOwner;
    public bool haveAttack;
    public int slotSpeed;

    public enum SlotType { Player, Enemy }
    public enum AttackType { None, Oneside_Attack, Exchange_Attacks }


    [Header("=== Slot UI ===")]
    [SerializeField] private Image iconBorder;
    [SerializeField] private Image iconImage;
    [SerializeField] private Text speedText;
    [SerializeField] private Image highlightImage;


    [Header("=== Attack Iine ===")]
    [SerializeField] private LineRenderer line = null;
    private GameObject lineTarget;
    public int curveResolution = 20; // 곡선의 해상도 (포인트 개수)
    public float curveHeight = 2.0f; // 곡선의 최대 높이 (곡률)


    [Header("=== Coroutine ===")]
    private Coroutine highlightCoroutine;


    // 공격 셋팅 - Player 버전
    public void Attack_Setting_Player(Attack_Base attack)
    {
        Debug.Log("call attack Setting");

        // 공격 슬롯 UI 삽입
        iconImage.sprite = attack.icon;
        myAttack = attack;
        haveAttack = true;
    }


    // 공격 셋팅 - Enemy 버전
    public void Attack_Setting_Enemy(Attack_Slot targetSlot, Attack_Base attack)
    {
        Debug.Log("call Enemy attack Setting");

        // 타겟 셋팅
        Attack_TargetSetting(targetSlot);

        // 공격 슬롯 UI 삽입
        iconImage.sprite = attack.icon;
        myAttack = attack;
        haveAttack = true;
    }


    // 슬롯 속도 셋팅
    public void Speed_Setting(int speed)
    {
        slotSpeed = speed;
    }


    // 해당 슬롯으로 누굴 공격할건지 셋팅
    public void Attack_TargetSetting(Attack_Slot target)
    {
        targetSlot = target;

        // 합 상태 셋팅
        switch (slotType)
        {
            case SlotType.Player:
                // 속도에 따른 합 & 일방 공격 상태 설정
                if (slotSpeed >= targetSlot.slotSpeed)
                {
                    attackType = AttackType.Exchange_Attacks;
                    target.Attack_TargetSetting(this);
                }
                else if (slotSpeed < targetSlot.slotSpeed)
                {
                    attackType = AttackType.Oneside_Attack;
                }
                break;

            case SlotType.Enemy:
                // 몬스터는 무조건 먼저 공격을 셋팅하니 무조건 일방임!
                if(targetSlot.myAttack != null)
                {
                    attackType = AttackType.Oneside_Attack;
                }
                else
                {
                    // 속도에 따른 합 & 일방 공격 상태 설정
                    if (slotSpeed >= targetSlot.slotSpeed)
                    {
                        attackType = AttackType.Exchange_Attacks;
                    }
                    else if (slotSpeed < targetSlot.slotSpeed)
                    {
                        attackType = AttackType.Oneside_Attack;
                    }
                }
                break;
        }
    }


    // 전투 종료 후 슬롯 리셋
    public void ResetSlot()
    {
        // 이펙트 리셋
        Highlights_Effect(false);
        if(targetSlot != null)
        {
            targetSlot.Highlights_Effect(false);
        }


        // 스테이터스 리셋
        attackType = AttackType.None;
        haveAttack = false;
        line.enabled = false;
        myAttack = null;
        targetSlot = null;
        lineTarget = null;

        // UI 리셋
        iconImage.sprite = null;
        speedText.text = "0";
    }


    // 일방공격 or 합 상태가 되었을 때 표시 -> 이거 곡선 표시 해야함!
    public void Attack_LineSetting(bool isOn, GameObject target)
    {
        if (isOn)
        {
            line.enabled = true;
            line.positionCount = curveResolution;
            Vector3 startPoint = transform.position;
            Vector3 endPoint = target.transform.position;
            Vector3 controlPoint = (startPoint + endPoint) / 2 + Vector3.up * curveHeight; // 슬라이더에 따라 곡률 변경

            for (int i = 0; i < curveResolution; i++)
            {
                float t = i / (float)(curveResolution - 1);
                Vector3 curvePoint = CalculateQuadraticBezierPoint(t, startPoint, controlPoint, endPoint);
                line.SetPosition(i, curvePoint);
            }
        }
        else
        {
            line.positionCount = 0;
            line.enabled = false;
        }
    }

    Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }


    // 공격 선택 중 슬롯 빛나는 효과 호출
    public void Highlights_Effect(bool isOn)
    {
        if(highlightCoroutine != null)
        {
            StopCoroutine(highlightCoroutine);
        }

        highlightCoroutine = StartCoroutine(HighLight(isOn));
    }


    // 공격 선택 중 슬롯 빛나는 효과 동작
    private IEnumerator HighLight(bool isOn)
    {
        float start = 0;
        float end = 0.5f;
        float timer = 0;

        if (isOn)
        {
            highlightImage.gameObject.SetActive(true);

            while (timer < 1)
            {
                timer += Time.deltaTime;
                highlightImage.color = new Color(highlightImage.color.r, highlightImage.color.g, highlightImage.color.b, Mathf.Lerp(start, end, EasingFunctions.OutExpo(timer)));
                yield return null;
            }
        }
        else
        {
            if(highlightImage.gameObject.activeSelf)
            {
                start = highlightImage.color.a;
                end = 0;
                while (timer < 1)
                {
                    timer += Time.deltaTime * 5f;
                    highlightImage.color = new Color(highlightImage.color.r, highlightImage.color.g, highlightImage.color.b, Mathf.Lerp(start, end, EasingFunctions.OutExpo(timer)));
                    yield return null;
                }
            }
            highlightImage.gameObject.SetActive(false);
        }
    }


    #region 마우스 이벤트
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Player_Manager.instnace.player_Turn.isExchangeTargetSelect)
        {
            return;
        }

        Highlights_Effect(true);

        switch (slotType)
        {
            case SlotType.Player:
                switch (attackType)
                {
                    case AttackType.None:
                        // Player_UI.instance.Turn_EngageUI(Player_UI.Object.Player, this, true);
                        break;

                    case AttackType.Oneside_Attack:
                        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Player, this, true);
                        Attack_LineSetting(true, targetSlot.gameObject);
                        break;

                    case AttackType.Exchange_Attacks:
                        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Player, this, true);
                        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Enemy, targetSlot, true);
                        Attack_LineSetting(true, targetSlot.gameObject);
                        targetSlot.Highlights_Effect(true);
                        break;
                }
                break;

            case SlotType.Enemy:
                switch (attackType)
                {
                    case AttackType.None:
                        // Player_UI.instance.Turn_EngageUI(Player_UI.Object.Enemy, this, true);
                        break;

                    case AttackType.Oneside_Attack:
                        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Enemy, this, true);
                        Attack_LineSetting(true, targetSlot.gameObject);
                        break;

                    case AttackType.Exchange_Attacks:
                        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Enemy, this, true);
                        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Player, targetSlot, true);
                        Attack_LineSetting(true, targetSlot.gameObject);
                        targetSlot.Highlights_Effect(true);
                        break;
                }
                break;
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(Player_Manager.instnace.player_Turn.isExchangeTargetSelect)
        {
            return;
        }

        Attack_LineSetting(false, null);
        Highlights_Effect(false);
        if (targetSlot != null)
            targetSlot.Highlights_Effect(false);

        switch (slotType)
        {
            case SlotType.Player:
                Player_UI.instance.Turn_EngageUI(Player_UI.Object.None, this, false);
                break;

            case SlotType.Enemy:
                Player_UI.instance.Turn_EngageUI(Player_UI.Object.None, this, false);
                break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Player_Manager.instnace.player_Turn.isExchangeTargetSelect)
        {
            return;
        }

        switch (slotType)
        {
            case SlotType.Player:
                ResetSlot();
                // 추가한다면 슬롯 초기화 기능?
                break;

            case SlotType.Enemy:
                // 여긴 동작할거 없음!
                break;
        }
    }
    #endregion
}
