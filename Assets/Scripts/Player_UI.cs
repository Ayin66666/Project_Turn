using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal; // �̰� �־�� ���׸��� �� ������ ȿ�� on/off �����ѵ�?

public class Player_UI : MonoBehaviour
{
    public static Player_UI instance;

    [Header("=== UI State ===")]
    public bool isFade;

    // ���� �ΰ� �Ұ����� �ɼ�â ������ Ȯ�� & �ɼ�â���� ������ ��ư �������� Ȯ��
    public bool isOptionOn;
    public bool isExitOn;


    // (���, �κ�, ��ų, �ɼ�) â
    [Header("=== Option UI ===")]
    [SerializeField] private GameObject optionSet;
    [SerializeField] private GameObject waponItemSet;
    [SerializeField] private GameObject skillSet;
    [SerializeField] private GameObject exitSet;


    // ü�¹� F & B
    [Header("=== Hp UI ===")]
    [SerializeField] private GameObject hpSet;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Text hpText;


    // ���� �ڿ�
    [Header("=== Attack Point UI ===")]
    [SerializeField] private GameObject attackPointSet;
    [SerializeField] private GameObject[] pointImage;


    // ���� <-> ���� �̵� Fade UI
    [Header("=== Fade UI ===")]
    [SerializeField] private GameObject fadeSet;
    [SerializeField] private Image fadeImage;
    [SerializeField] private GameObject fadeText_Normal;
    [SerializeField] private GameObject fadeText_Boss;
    [SerializeField] private GameObject fadeText_Effect;
    [SerializeField] private Material background_Material;
    [SerializeField] private Material normalText_Material;
    [SerializeField] private Material bossText_Material;


    // �� ���� �÷��̾� ���� ���� UI
    [Header("=== Turn Fight Select UI ===")]
    [SerializeField] private GameObject selectListSet;
    [SerializeField] private GameObject attackSlotSet;
    [SerializeField] private Button turn_FightButton;


    // �� ���� �÷��̾� Ÿ�� ���� UI
    [Header("=== Turn Fight Target Select UI ===")]
    [SerializeField] private GameObject targetSelectSet;
    [SerializeField] private Image targetSelectBackgroundImage;
    [SerializeField] private Text targetNameText;


    // �� UI
    [Header("=== Turn Fight Exchange UI ===")]
    // �Ϲ� - �� ���� ǥ�� UI
    [SerializeField] private GameObject exchangeSet;
    [SerializeField] private Text exchangeText;

    // �÷��̾� �� UI
    [SerializeField] private GameObject pExchangeUISet;
    [SerializeField] private Image exchange_pIconImage;
    [SerializeField] private Text exchange_pAttackNameText;
    [SerializeField] private Text exchange_pdamageText;
    [SerializeField] private Text exchange_pDescriptionText;
    // ���ʹ� �� UI
    [SerializeField] private GameObject eExchangeUISet;
    [SerializeField] private Image exchange_eIconImage;
    [SerializeField] private Text exchange_eAttackNameText;
    [SerializeField] private Text exchange_edamageText;
    [SerializeField] private Text exchange_eDescriptionText;


    // ���� ����
    // �̱������� ������
    // UI �۾��� World UI��
    // UI ������Ʈ �߰��� �� ��� �ٿ���


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Fade �׽�Ʈ��
        if (Input.GetKeyDown(KeyCode.V))
        {
            TurnFight_Fade();
        }

        Option();

        // HP �׽�Ʈ��
        Hp();
    }

    // ���ʸ� �۾��ϸ� ��
    #region World UI
    public void Option()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!isOptionOn)
            {
                isOptionOn = true;
                optionSet.SetActive(true);
            }
            else
            {
                isOptionOn = false;
                optionSet.SetActive(false);
            }
        }
    }

    public void Option_Click_Exit()
    {
        isExitOn = true;
        exitSet.SetActive(true);
    }

    public void Option_Click_ExitOff()
    {
        isExitOn = false;
        exitSet.SetActive(false);
    }

    public void Hp()
    {
        // ���忡���� �ѹ��� ȣ���ϴ¿� �����
        // �Ͽ����� ������Ʈ

        hpSlider.value = ((float)Player_Manager.instnace.curHp / (float)Player_Manager.instnace.maxHp);

        hpText.text = Player_Manager.instnace.curHp + " / " + Player_Manager.instnace.maxHp;
    }
    #endregion


    // �� UI�� ���� �߰��� ��ġ�ϰ� �۾��ϰ����ϴ�
    #region Turn UI

    // �÷��̾� �� UI On / Off
    public void Turn_FightSelect(bool isOn)
    {
        selectListSet.SetActive(isOn);
        attackSlotSet.SetActive(isOn);
    }

    // �÷��̾� Ÿ�� ���� UI On / Off
    public void Turn_TargetSelect(bool isOn)
    {
        targetSelectSet.SetActive(isOn);
    }

    // �÷��̾� Ÿ�� ���� ������ �޾ƿ��� & ����
    public void Turn_TargetSelect_DataSetting(bool isOn, Enemy_Base enemy)
    {
        if(isOn)
        {
            targetNameText.text = enemy.status.EnemyName;
        }
        else
        {
            targetNameText.text = null;
        }
    }

    // ���� �ڿ� UI
    public void Turn_AttackPoint()
    {
        // Reset UI
        for (int i = 0; i < pointImage.Length; i++)
        {
            pointImage[i].SetActive(false);
        }

        // UI On
        for (int i = 0; i < Player_Manager.instnace.AttackPoint; i++)
        {
            pointImage[i].SetActive(true);
        }
    }

    // �÷��̾� VS �ֳʹ� ���� �� UI -> ���� �۾��� -> ���� �׽�Ʈ �ʿ���!
    public void Turn_EngageUI(Attack_Slot slot, bool isOn)
    {
        // ���Կ��� �����͸� �޾ƿͼ� ����
        // �÷��̾� & ���� ���� �����͸� �޾ƿͼ� ���
        // �����ʹ� �÷��̾��� ���� ���Կ� myAttack �� target �� ����!
        // �����͸� ���� ��, ���� �� UI�� ����

        if(isOn)
        {
            // �÷��̾� & �� ���� UI ����
            if (slot.myAttack != null)
            {
                // �� ���� UI ����
                exchangeSet.SetActive(true);
                switch (slot.attackType)
                {
                    case Attack_Slot.AttackType.Oneside_Attack:
                        exchangeText.text = "�Ϲ� ����";
                        break;

                    case Attack_Slot.AttackType.Exchange_Attacks:
                        exchangeText.text = "�� ����";
                        break;
                }

                // �÷��̾� UI ����
                pExchangeUISet.SetActive(true);
                exchange_pIconImage.sprite = slot.myAttack.icon;
                exchange_pAttackNameText.text = slot.myAttack.attackName;
                exchange_pDescriptionText.text = slot.myAttack.attackName;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < slot.myAttack.damageValue.Length; i++)
                {
                    sb.Append($"{slot.myAttack.damageValue[i].x} ~ {slot.myAttack.damageValue[i].y}");
                    if (i < slot.myAttack.damageValue.Length - 1)
                    {
                        sb.Append("\n");
                    }
                }
                exchange_pdamageText.text = sb.ToString();
            }


            // ���� UI ����
            if (slot.targetSlot != null)
            {
                if (slot.attackType == Attack_Slot.AttackType.Exchange_Attacks)
                {
                    // ���� �� ���� �޾ƿ���
                    Attack_Base enemyAttack = slot.targetSlot.myAttack;

                    // UI ����
                    eExchangeUISet.SetActive(true);
                    exchange_eIconImage.sprite = enemyAttack.icon;
                    exchange_eAttackNameText.text = enemyAttack.attackName;
                    exchange_eDescriptionText.text = enemyAttack.attackName;
                    System.Text.StringBuilder sb2 = new System.Text.StringBuilder();
                    for (int i = 0; i < enemyAttack.damageValue.Length; i++)
                    {
                        sb2.Append($"{enemyAttack.damageValue[i].x} ~ {enemyAttack.damageValue[i].y}");
                        if (i < enemyAttack.damageValue.Length - 1)
                        {
                            sb2.Append("\n");
                        }
                    }
                    exchange_edamageText.text = sb2.ToString();
                }
            }
        }
        else
        {
            // UI ����
            exchangeSet.SetActive(false);
            pExchangeUISet.SetActive(false);
            eExchangeUISet.SetActive(false);
        }
    }

    // ���� ���� ��ư Ȱ��ȭ
    public void Turn_AttackButton(bool isOn)
    {
        turn_FightButton.interactable = isOn;
    }

    // ���� ���� ���̵� ȣ��
    public void TurnFight_Fade()
    {
        if(isFade)
        {
            return;
        }

        StartCoroutine(FadeCall());
    }

    // ���� ���� ���̵� ����
    private IEnumerator FadeCall()
    {
        isFade = true;
        background_Material.SetFloat("_VignetteIntensity", 0);
        normalText_Material.SetFloat("_DissolveAmount", 1);


        // Fade In
        fadeSet.SetActive(true);
        float a = 0;
        while (a < 1)
        {
            a += Time.deltaTime * 5f;
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, a);
            yield return null;
        }
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);


        // Delay
        yield return new WaitForSeconds(0.25f);


        // Backgrouund Effect On
        a = 0;
        float start = 0;
        float end = 1.5f;
        float cur = 0;
        while (a < 1)
        {
            a += Time.deltaTime * 1.5f;
            cur = Mathf.Lerp(start, end, a);
            background_Material.SetFloat("_VignetteIntensity", cur);
            yield return null;
        }


        // Delay
        yield return new WaitForSeconds(1f);


        // Text On
        fadeText_Normal.SetActive(true);


        // Delay
        yield return new WaitForSeconds(0.75f);


        // Dissovle Effect On & Text Off
        a = 1;
        int count = 1;
        while (a > 0)
        {
            if(a < 0.75f && count > 0)
            {
                fadeText_Effect.SetActive(true);
                count--;
            }

            a -= Time.deltaTime * 0.25f;
            normalText_Material.SetFloat("_DissolveAmount", a);
            yield return null;
        }
        normalText_Material.SetFloat("_DissolveAmount", -0.12f);


        // Delay
        yield return new WaitForSeconds(1f);
        fadeText_Normal.SetActive(false);


        // Fade Out
        a = 1;
        start = 1.5f;
        end = 0;
        cur = 2;
        while (a > 0)
        {
            a -= Time.deltaTime * 0.85f;
            cur = Mathf.Lerp(end, start, a);
            background_Material.SetFloat("_VignetteIntensity", cur);
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, a);
            yield return null;
        }
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);
        fadeSet.SetActive(false);


        // Delay
        yield return new WaitForSeconds(0.15f);

        isFade = false;
    }

    #endregion
}
