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

    // �̰ŵ� ������ ���� ������ �ƴ϶� �÷��̾ ����ؾ� �ϴ°� �ƴѰ�?
    public int DamageCal(int count)
    {
        // ���⿡ ������ ���� ������ ���� ���� �ʿ���!
        // ������ �⺻���� �鰡�� ����!


        // �⺻ ������
        int minDamage = myAttack.damageValue[count].x;
        int maxDamage = myAttack.damageValue[count].y;

        // ���� ���
        

        // ���� ������ ����
        int damage = Random.Range(minDamage, maxDamage);
        return damage;
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
                }
                else if (slotSpeed < targetSlot.slotSpeed)
                {
                    attackType = AttackType.Oneside_Attack;
                }
                break;

            case SlotType.Enemy:
                // ���ʹ� ������ ���� ������ �����ϴ� ������ �Ϲ���!
                attackType = AttackType.Oneside_Attack;
                break;
        }
    }

    public void Attack()
    {

    }

    // ���� ���� �� ���� ����
    public void ResetSlot()
    {
        haveAttack = false;
        myAttack = null;
        lineTarget = null;
        line.enabled = false;
        remainingAttackCount = 0;
    }



    #region ���콺 �̺�Ʈ
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
                // �߰��Ѵٸ� ���� �ʱ�ȭ ���?
                break;

            case SlotType.Enemy:
                // ���� �����Ұ� ����!
                break;
        }
    }
    #endregion
}
