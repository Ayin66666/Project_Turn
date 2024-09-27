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


    [Header("=== Coroutine ===")]
    private Coroutine highlightCoroutine;


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


    // �Ϲ���� or �� ���°� �Ǿ��� �� ǥ�� -> �̰� � ǥ�� �ؾ���!
    public void Attack_LineSetting(AttackType type, bool isOn, GameObject target)
    {
        if(isOn)
        {
            line.enabled = true;
            line.SetPosition(0, transform.position);

            switch (type)
            {
                case AttackType.None:
                    break;

                case AttackType.Oneside_Attack:
                    line.SetPosition(1, target.transform.position);
                    break;

                case AttackType.Exchange_Attacks:
                    line.SetPosition(1, target.transform.position);
                    break;
            }
        }
        else
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, transform.position);
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
        // ����Ʈ ����
        Highlights_Effect(false);
        if(targetSlot != null)
        {
            targetSlot.Highlights_Effect(false);
        }


        // �������ͽ� ����
        attackType = AttackType.None;
        haveAttack = false;
        line.enabled = false;
        myAttack = null;
        targetSlot = null;
        lineTarget = null;

        // UI ����
        iconImage.sprite = null;
        speedText.text = "0";
    }


    // ���� ���� �� ���� ������ ȿ�� ȣ��
    public void Highlights_Effect(bool isOn)
    {
        if(highlightCoroutine != null)
        {
            StopCoroutine(highlightCoroutine);
        }

        highlightCoroutine = StartCoroutine(HighLight(isOn));
    }


    // ���� ���� �� ���� ������ ȿ�� ����
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


    #region ���콺 �̺�Ʈ
    public void OnPointerEnter(PointerEventData eventData)
    {
        Highlights_Effect(true);

        switch (slotType)
        {
            case SlotType.Player:
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
                        targetSlot.Highlights_Effect(true);
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
        Highlights_Effect(false);

        if(targetSlot != null)
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
