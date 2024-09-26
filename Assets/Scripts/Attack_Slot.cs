using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


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


    [Header("=== Attack Iine ===")]
    [SerializeField] private LineRenderer line = null;
    private GameObject lineTarget;


    [Header("=== Attack Setting===")]
    private int minDamage;
    private int maxDamage;


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

    // 일방공격 or 합 상태가 되었을 때 표시 -> 이거 아직 미완
    public void Attack_LineSetting(bool isOn, bool isEngage)
    {
        if (isOn)
        {
            line.enabled = true;
            if (isEngage)
            {
                lineTarget = targetSlot.gameObject;
            }
            else
            {

            }
        }
        else
        {
            lineTarget = null;
            line.enabled = false;
        }
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
        haveAttack = false;
        myAttack = null;
        lineTarget = null;
        line.enabled = false;
    }



    #region 마우스 이벤트
    public void OnPointerEnter(PointerEventData eventData)
    {
        switch (slotType)
        {
            case SlotType.Player:
                iconBorder.color = Color.gray;
                switch (attackType)
                {
                    case AttackType.None:
                        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Player, this, true);
                        break;

                    case AttackType.Oneside_Attack:
                        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Player, this, true);
                        break;

                    case AttackType.Exchange_Attacks:
                        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Player, this, true);
                        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Enemy, targetSlot, true);
                        break;
                }
                break;

            case SlotType.Enemy:
                switch (attackType)
                {
                    case AttackType.None:
                        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Enemy, this, true);
                        break;

                    case AttackType.Oneside_Attack:
                        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Enemy, this, true);
                        break;

                    case AttackType.Exchange_Attacks:
                        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Enemy, this, true);
                        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Player, targetSlot, true);
                        break;
                }
                break;
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        switch (slotType)
        {
            case SlotType.Player:
                iconBorder.color = Color.white;
                Player_UI.instance.Turn_EngageUI(Player_UI.Object.None, this, false);
                break;

            case SlotType.Enemy:
                Player_UI.instance.Turn_EngageUI(Player_UI.Object.None, this, false);
                break;
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
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
