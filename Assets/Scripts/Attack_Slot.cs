using System.Collections;
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
    [HideInInspector] public Enemy_Base enemy;
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
    public float curveHeight = 2.0f; // ��� �ִ� ���� (���)
    public int curveResolution = 20; // ��� �ػ� (����Ʈ ����)
    [SerializeField] private LineRenderer line = null;
    private GameObject lineTarget;


    [Header("=== Coroutine ===")]
    private Coroutine highlightCoroutine;
    private Coroutine lineCoroutine;
    private void Awake()
    {
        if(slotType == SlotType.Enemy)
        {
            enemy = slotOwner.GetComponent<Enemy_Base>();
        }
    }

    // ���� ���� - ���Կ� ���� ����
    public void Attack_Setting(Attack_Base myAttack)
    {
        if(myAttack != null)
        {
            // ���� ������ ����
            haveAttack = true;

            // ���� ���� UI ����
            this.myAttack = myAttack;
            iconImage.color = new Color(iconImage.color.r, iconImage.color.g, iconImage.color.b, 1);
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
        // ���� ���� & Ÿ�� ���� ����
        attackType = type;
        targetSlot = target;
    }


    // ���� �ӵ� ����
    public void Speed_Setting(int speed)
    {
        slotSpeed = speed;
        speedText.text = speed.ToString();
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
        iconImage.color = new Color(iconImage.color.r, iconImage.color.g, iconImage.color.b, 0);

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
        // speedText.text = "0";
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
        float end = 1;
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

                // ����
                Player_Manager.instnace.UI_Sound_Call(0);

                switch (attackType)
                {
                    case AttackType.None:
                        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Player, this, true);
                        break;

                    case AttackType.Oneside_Attack:
                        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Player, this, true);
                        Player_UI.instance.Turn_Exchange(false);
                        Attack_LineSetting(true, targetSlot.gameObject);
                        break;

                    case AttackType.Exchange_Attacks:
                        Player_UI.instance.Turn_WinningUI(true, this, targetSlot);
                        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Player, this, true);
                        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Enemy, targetSlot, true);
                        Player_UI.instance.Turn_Exchange(true);
                        Attack_LineSetting(true, targetSlot.gameObject);
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
                        Player_UI.instance.Turn_Exchange(false);
                        Attack_LineSetting(true, targetSlot.gameObject);
                        break;

                    case AttackType.Exchange_Attacks:
                        Player_UI.instance.Turn_WinningUI(true, targetSlot, this);
                        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Enemy, this, true);
                        Player_UI.instance.Turn_EngageUI(Player_UI.Object.Player, targetSlot, true);
                        Player_UI.instance.Turn_Exchange(true);
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

        Player_UI.instance.Turn_EngageUI(Player_UI.Object.None, this, false);
        Player_UI.instance.Turn_WinningUI(false, null, null);

        /* -> �̰� ���� �����̸� ���� �ʿ䰡?
        switch (slotType)
        {
            case SlotType.Player:
                Player_UI.instance.Turn_EngageUI(Player_UI.Object.None, this, false);
                break;

            case SlotType.Enemy:
                Player_UI.instance.Turn_EngageUI(Player_UI.Object.None, this, false);
                break;
        }
        */
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
                    Player_Manager.instnace.turnManger.Exchange_Setting_Remove(this, targetSlot, myExchange);
                break;

            case SlotType.Enemy:
                break;
        }
    }
    #endregion
}
