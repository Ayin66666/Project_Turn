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
    public int remainingAttackCount;
    public enum SlotType { Player, Enemy }
    public enum AttackType { None, Oneside_Attack, Exchange_Attacks }


    [Header("=== Slot UI ===")]
    [SerializeField] private Image iconBorder;
    [SerializeField] private Image iconImage;
    [SerializeField] private Text speedText;


    [Header("=== Attack Iine ===")]
    [SerializeField] private LineRenderer line = null;
    private GameObject lineTarget;


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

    // 이거도 따지고 보면 슬롯이 아니라 플레이어가 계산해야 하는거 아닌가?
    public int DamageCal(int count)
    {
        // 여기에 버프에 따른 데미지 증감 구현 필요함!
        // 지금은 기본값만 들가게 만듬!


        // 기본 데미지
        int minDamage = myAttack.damageValue[count].x;
        int maxDamage = myAttack.damageValue[count].y;

        // 버프 계산
        

        // 최종 데미지 결정
        int damage = Random.Range(minDamage, maxDamage);
        return damage;
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
                }
                else if (slotSpeed < targetSlot.slotSpeed)
                {
                    attackType = AttackType.Oneside_Attack;
                }
                break;

            case SlotType.Enemy:
                // 몬스터는 무조건 먼저 공격을 셋팅하니 무조건 일방임!
                attackType = AttackType.Oneside_Attack;
                break;
        }
    }

    public void Attack()
    {

    }

    // 전투 종료 후 슬롯 리셋
    public void ResetSlot()
    {
        haveAttack = false;
        myAttack = null;
        lineTarget = null;
        line.enabled = false;
        remainingAttackCount = 0;
    }



    #region 마우스 이벤트
    public void OnPointerEnter(PointerEventData eventData)
    {
        switch (slotType)
        {
            case SlotType.Player:
                iconBorder.color = Color.gray;
                Player_UI.instance.Turn_EngageUI(this, true);
                break;

            case SlotType.Enemy:
                Player_UI.instance.Turn_EngageUI(this, true);
                break;
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        switch (slotType)
        {
            case SlotType.Player:
                iconBorder.color = Color.white;
                Player_UI.instance.Turn_EngageUI(this, false);
                break;

            case SlotType.Enemy:
                Player_UI.instance.Turn_EngageUI(this, false);
                break;
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (slotType)
        {
            case SlotType.Player:
                // 추가한다면 슬롯 초기화 기능?
                break;

            case SlotType.Enemy:
                // 여긴 동작할거 없음!
                break;
        }
    }
    #endregion
}
