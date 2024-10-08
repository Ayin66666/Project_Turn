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
    public Exchange myExchange;

    public enum SlotType { Player, Enemy }
    public enum AttackType { None, Oneside_Attack, Exchange_Attacks }


    [Header("=== Cur Attack Setting ===")]
    [SerializeField] private int curAttackCount;


    [Header("=== Slot UI ===")]
    [SerializeField] private Image iconBorder;
    [SerializeField] private Image iconImage;
    [SerializeField] private Text speedText;
    [SerializeField] private Image highlightImage;


    [Header("=== Attack Iine ===")]
    [SerializeField] private LineRenderer line = null;
    private GameObject lineTarget;
    public int curveResolution = 20; // ��� �ػ� (����Ʈ ����)
    public float curveHeight = 2.0f; // ��� �ִ� ���� (���)


    [Header("=== Coroutine ===")]
    private Coroutine highlightCoroutine;



    // ���� ���� - ���Կ� ���� ����
    public void Attack_Setting(Attack_Base myAttack)
    {
        if(myAttack != null)
        {
            // ���� ������ ����
            haveAttack = true;

            // ���� ���� UI ����
            this.myAttack = myAttack;
            iconImage.sprite = myAttack.icon;
        }
        else
        {
            haveAttack = false;

            myAttack = null;
            iconImage.sprite = null;
        }
    }


    // ���� ���� - ���� Ÿ�� & Ÿ�� ����
    public void Attack_Setting(AttackType type, Attack_Slot target)
    {
        Debug.Log("Call Target SEtting");
        // ���� ���� & Ÿ�� ���� ����
        attackType = type;
        targetSlot = target;
    }


    // ���� �ӵ� ����
    public void Speed_Setting(int speed)
    {
        slotSpeed = speed;
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

        // ���� ���� Ƚ�� ����
        curAttackCount = 0;

        // �������ͽ� ����
        attackType = AttackType.None;
        haveAttack = false;
        line.enabled = false;
        myAttack = null;
        targetSlot = null;
        lineTarget = null;
        myExchange = null;

        // UI ����
        iconImage.sprite = null;
        speedText.text = "0";
    }


    // �Ϲ���� or �� ���°� �Ǿ��� �� ǥ�� -> �̰� � ǥ�� �ؾ���!
    public void Attack_LineSetting(bool isOn, GameObject target)
    {
        if (isOn)
        {
            line.enabled = true;
            line.positionCount = curveResolution;
            Vector3 startPoint = transform.position;
            Vector3 endPoint = target.transform.position;
            Vector3 controlPoint = (startPoint + endPoint) / 2 + Vector3.up * curveHeight; // �����̴��� ���� ��� ����

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
                timer += Time.deltaTime * 2f;
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
                        Debug.Log("�÷��̾� ���� ǥ�� ȣ�� / none");
                        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Player, this, true);
                        break;

                    case AttackType.Oneside_Attack:
                        Debug.Log("�÷��̾� ���� ǥ�� ȣ�� / oneside");
                        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Player, this, true);
                        Attack_LineSetting(true, targetSlot.gameObject);
                        break;

                    case AttackType.Exchange_Attacks:
                        Debug.Log("�÷��̾� ���� ǥ�� ȣ�� / exchange");
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
                        Debug.Log("���ʹ� ���� ǥ�� ȣ�� / None");
                        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Enemy, this, true);
                        break;

                    case AttackType.Oneside_Attack:
                        Debug.Log("���ʹ� ���� ǥ�� ȣ�� / Oneside");
                        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Enemy, this, true);
                        Attack_LineSetting(true, targetSlot.gameObject);
                        break;

                    case AttackType.Exchange_Attacks:
                        Debug.Log("���ʹ� ���� ǥ�� ȣ�� / exchange");
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
                if(myAttack != null)
                {
                    Player_Manager.instnace.turnManger.Exchange_Setting_Remove(this, targetSlot, myExchange);
                    //Player_Manager.instnace.turnManger.Exchange_Setting_Remove(this, targetSlot);
                }
                break;

            case SlotType.Enemy:
                // ���� �����Ұ� ����!
                break;
        }
    }
    #endregion
}
