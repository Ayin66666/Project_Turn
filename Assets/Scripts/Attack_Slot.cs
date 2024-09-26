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


    // ���� ���� - Player ����
    public void Attack_Setting_Player(Attack_Base attack)
    {
        Debug.Log("call attack Setting");

        // ���� ���� UI ����
        iconImage.sprite = attack.icon;
        myAttack = attack;
        haveAttack = true;
    }

    // ���� ���� - Enemy ����
    public void Attack_Setting_Enemy(Attack_Slot targetSlot, Attack_Base attack)
    {
        Debug.Log("call Enemy attack Setting");

        // Ÿ�� ����
        Attack_TargetSetting(targetSlot);

        // ���� ���� UI ����
        iconImage.sprite = attack.icon;
        myAttack = attack;
        haveAttack = true;
    }

    // �Ϲ���� or �� ���°� �Ǿ��� �� ǥ�� -> �̰� ���� �̿�
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

    // ���� �ӵ� ����
    public void Speed_Setting(int speed)
    {
        slotSpeed = speed;
    }


    // �ش� �������� ���� �����Ұ��� ����
    public void Attack_TargetSetting(Attack_Slot target)
    {
        targetSlot = target;

        // �� ���� ����
        switch (slotType)
        {
            case SlotType.Player:
                // �ӵ��� ���� �� & �Ϲ� ���� ���� ����
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
                // ���ʹ� ������ ���� ������ �����ϴ� ������ �Ϲ���!
                if(targetSlot.myAttack != null)
                {
                    attackType = AttackType.Oneside_Attack;
                }
                else
                {
                    // �ӵ��� ���� �� & �Ϲ� ���� ���� ����
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


    // ���� ���� �� ���� ����
    public void ResetSlot()
    {
        haveAttack = false;
        myAttack = null;
        lineTarget = null;
        line.enabled = false;
    }



    #region ���콺 �̺�Ʈ
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
                // �߰��Ѵٸ� ���� �ʱ�ȭ ���?
                break;

            case SlotType.Enemy:
                // ���� �����Ұ� ����!
                break;
        }
    }
    #endregion
}
