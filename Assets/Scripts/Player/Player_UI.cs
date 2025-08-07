using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Easing.Tweening;
using static Player_UI;
// using UnityEngine.UIElements; // �̰� �־�� ���׸��� �� ������ ȿ�� on/off �����ѵ�?

public class Player_UI : MonoBehaviour
{
    public static Player_UI instance;


    [Header("=== UI State ===")]    
    public bool isFade;
    public bool isDialog;

    // ���� �ΰ� �Ұ����� �ɼ�â ������ Ȯ�� & �ɼ�â���� ������ ��ư �������� Ȯ��
    public bool isMenuOn;
    public bool isWeapon_ItemOn;
    public bool isSkillOn;
    public bool isTutorialOn;

    public enum Object { None, Player, Enemy }
    public enum Outline { TypeA, TypeB }
    private Outline outline;


    // (���, �κ�, ��ų, �ɼ�, Ʃ�丮��) â
    [Header("=== Option UI ===")]
    [SerializeField] private GameObject menuSet;
    [SerializeField] private GameObject weaponItemSet;
    [SerializeField] private GameObject skillSet;
    [SerializeField] private GameObject tutorialSet;

    public Item_Description_UI item_Description;
    public GameObject item_Description_UI;


    // �κ��丮 �������ͽ�
    [Header("=== Inventory Status UI ===")]
    [SerializeField] private Text inventory_Hp_Text;
    [SerializeField] private Text inventory_Damage_Text;
    [SerializeField] private Text inventory_Defense_Text;
    [SerializeField] private Text inventory_CriticalChance_Text;
    [SerializeField] private Text inventory_CriticalMultiplier_Text;


    [Header("=== ������ ȹ�� UI ===")]
    [SerializeField] private GameObject itemUI;
    [SerializeField] private RectTransform[] itemUISpawnPos;


    [Header("=== ��ų����Ʈ ȹ�� UI ===")]
    [SerializeField] private GameObject skillpointUI;
    [SerializeField] private CanvasGroup skillpointCanvasGroup;
    [SerializeField] private Text skillpointAddText;
    [SerializeField] private Text skillPointText;

    // �̴ϸ�
    [Header("=== Mini Map UI ===")]
    [SerializeField] private GameObject miniMapSet;


    [Header("=== Stage Name UI ===")]
    [SerializeField] private GameObject stageNameset;
    [SerializeField] private CanvasGroup stageNameCanvasGroup;
    [SerializeField] private Text stageNameText;


    // ü�¹� F & B
    [Header("=== Hp UI ===")]
    [SerializeField] private float delayTime;
    public GameObject hpSet;
    [SerializeField] private Slider hpBarF;
    [SerializeField] private Slider hpBarB;
    [SerializeField] private Text hpText;
    [SerializeField] private Coroutine hpCoroutine;


    // ��ųƮ�� ��� ���� UI
    [Header("=== ��ųƮ�� ���� UI ===")]
    [SerializeField] private GameObject skillDescriptionSet;
    [SerializeField] private Image skillDescriptionIcon;
    [SerializeField] private Text skillNameText;
    [SerializeField] private Text skillDamageText;
    [SerializeField] private Text skillDescriptionText;


    [Header("=== ��ųƮ�� ���� ���� or ���� UI ===")]
    [SerializeField] private GameObject skillLearnResultSet;
    [SerializeField] private CanvasGroup skillLearnResultCanvasGroup;
    [SerializeField] private Text skillLearnResultText;
    private Coroutine skillLearnResultCoroutine;


    // ���� ����Ʈ UI
    [Header("=== Attack Point UI ===")]
    [SerializeField] private GameObject attackPointSet;
    [SerializeField] private GameObject[] pointImage;
    [SerializeField] private Text attackPointText;


    // �̸� ������ & ��ư UI
    [Header("=== Underside Gauge UI ===")]
    [SerializeField] private Slider undersideSlider;
    [SerializeField] private GameObject undersideButton;
    [SerializeField] private GameObject undersideSkill;
    private Coroutine gaugeCoroutine;


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


    // �� ���� ���� UI
    [Header("=== �� ���� ���� UI ===")]
    [SerializeField] private GameObject turnStartUISet;
    [SerializeField] private CanvasGroup turnStartResultCanvasGroup;


    // �� ī��Ʈ UI
    [Header("=== �� ī��Ʈ UI ===")]
    [SerializeField] private GameObject turnCountUISet;
    [SerializeField] private CanvasGroup turnCountCanvasGroup;
    [SerializeField] private Text turnCountText;


    // �� ���� �÷��̾� ���� ���� UI
    [Header("=== Turn Fight Attack Select UI ===")]
    [SerializeField] private GameObject selectListSet;
    [SerializeField] private GameObject attackSlotSet;
    [SerializeField] private Button turn_FightButton;
    [SerializeField] private CanvasGroup attackSlotCanvasGroup;
    public AttackContent_UI[] attackContent;


    // �� ���� �÷��̾� Ÿ�� ���� UI
    [Header("=== Turn Fight Target Select UI ===")]
    [SerializeField] private GameObject targetSelectSet;
    [SerializeField] private Image targetSelectBackgroundImage;
    [SerializeField] private Text targetNameText;


    // �� UI
    [Header("=== Turn Fight Exchange UI ===")]
    [SerializeField] private GameObject exchangeSet; // �Ϲ� - �� ���� ǥ�� UI
    [SerializeField] private Text exchangeText;


    [Header("=== �� ���Ҹ� UI ===")]
    [SerializeField] private GameObject winningrateSet;
    [SerializeField] private Text winningrateText;
    [SerializeField] private Text playerMinMaxDamageText;
    [SerializeField] private Text enemyMinMaxDamageText;


    [Header("=== Turn Fight Exchange Player UI ===")]
    [SerializeField] private GameObject pExchangeUISet;
    [SerializeField] private Image exchange_pIconImage;
    [SerializeField] private Text exchange_pAttackNameText;
    [SerializeField] private Text exchange_pdamageText;
    [SerializeField] private Text exchange_pDescriptionText;
    [SerializeField] private Text exchange_pSlotSpeedText;


    [Header("=== Turn Fight Exchange Enemy UI ===")]
    [SerializeField] private GameObject eExchangeUISet;
    [SerializeField] private Image exchange_eIconImage;
    [SerializeField] private Text exchange_eAttackNameText;
    [SerializeField] private Text exchange_edamageText;
    [SerializeField] private Text exchange_eDescriptionText;
    [SerializeField] private Text exchange_eSlotSpeedText;


    // ���� ���� ������ �� ������ ���� ���� UI
    [Header("=== Turn Fight Exchange Summary UI ===")]
    [SerializeField] private GameObject exchangeSummarySet;
    [SerializeField] private Text exchangeSummaryNameText;
    [SerializeField] private Text exchangeSummaryDescriptionText;
    [SerializeField] private Text exchangeSummaryDamageText;
    [SerializeField] private GameObject[] exchangeSummaryAttackCount;
    [SerializeField] private GameObject[] exchangeSummaryAttackCountborder;


    [Header("=== �� ���� �� �����ڸ� ����Ʈ ===")]
    [SerializeField] private GameObject outlineAset;
    [SerializeField] private Image[] outlineA_Image;
    [SerializeField] private RectTransform[] outlineAU_Pos;
    [SerializeField] private RectTransform[] outlineAD_Pos;

    [SerializeField] private GameObject outlineBset;
    [SerializeField] private Image[] outlineB_Image;
    [SerializeField] private RectTransform[] outlineBU_Pos;
    [SerializeField] private RectTransform[] outlineBD_Pos;

    [SerializeField] private PMove[] pmoves_UD;
    [SerializeField] private PMove[] pmoves_SS;
    [SerializeField] private Text totalDamage_Text;


    // ���� �¸� & �й� UI
    [Header("=== Turn Fight Win & Lose UI ===")]
    [SerializeField] private GameObject winUISet;
    [SerializeField] private CanvasGroup winUICanvasGroup;
    [SerializeField] private GameObject loseUISet;
    [SerializeField] private CanvasGroup loseCanvasGroup;


    [Header("=== �������� ���� UI ===")]
    [SerializeField] private GameObject nextSceneSet;
    [SerializeField] private Image nextSceneImage;
    [SerializeField] private Text nextSceneText;



    // ���� ����
    // �̱������� ������
    // UI �۾��� World UI��
    // UI ������Ʈ �߰��� �� ��� �ٿ���

    // ���� �ӵ� ����ִ� ����� ���� ��ũ��Ʈ�� �̵���Ŵ!


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

    private void Start()
    {
        // �ʱ� UI ����
        hpBarF.maxValue = Player_Manager.instnace.maxHp;
        hpBarF.value = Player_Manager.instnace.maxHp;
        hpBarB.maxValue = Player_Manager.instnace.maxHp;
        hpBarB.value = Player_Manager.instnace.maxHp;
        hpText.text = Player_Manager.instnace.curHp + " / " + Player_Manager.instnace.maxHp;

        // ��ų ����Ʈ
        skillPointText.text = $"���� ����Ʈ : {Player_Skill_Manager.instance.SkillPoint}";

        // �ʻ�� ������
        undersideSlider.maxValue = Player_Manager.instnace.MaxUndersideGauge;
        undersideSlider.value = Player_Manager.instnace.CurUndersideGauge;
    }

    private void Update()
    {
        // Fade �׽�Ʈ��
        if (Input.GetKeyDown(KeyCode.V))
        {
            Player_Manager.instnace.UnderSideGaugeAdd(15);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Menu();
        }
    }


    // ���ʸ� �۾��ϸ� ��
    #region World UI

    // �÷��̾� ���� UI On / Off
    public void World_UISetting(bool isOn)
    {
        // �̴ϸ�
        miniMapSet.SetActive(isOn);

        // ����Ʈ
        Quset_Manager.instance.QusetUI_Setting(isOn);
    }


    /// <summary>
    /// ��ų ���� �õ� �� ���� ���� ���� (0 = ���� / 1 = ����-�ڽ�Ʈ ���� / 2 = ����-���ེų �ȹ�� / 3 = ����-�̹̹��)
    /// </summary>
    /// <param name="resultIndex"> 0 = ���� / 1 = ����-�ڽ�Ʈ ���� / 2 = ����-���ེų �ȹ�� / 3 = ����-�̹̹��</param>
    public void Skill_LearnResultUI(Attack_Base attack, int resultIndex)
    {
        switch (resultIndex)
        {
            case 0:
                skillLearnResultText.text = attack.attackName + " ����";
                break;

            case 1:
                skillLearnResultText.text = "�ڽ�Ʈ ����";
                break;

            case 2:
                skillLearnResultText.text = "���� ��ų �̺�";
                break;

            case 3:
                skillLearnResultText.text = "�̹� �����Ͽ����ϴ�.";
                break;
        }

        if(skillLearnResultCoroutine != null)
        {
            StopCoroutine(skillLearnResultCoroutine);
        }
        skillLearnResultCoroutine = StartCoroutine(SkillLearnResultCall());
    }

    public IEnumerator SkillLearnResultCall()
    {
        // UI Ȱ��ȭ
        skillLearnResultCanvasGroup.alpha = 1;
        skillLearnResultSet.SetActive(true);

        // UI ���̵�
        float timer = 0;
        float start = 1;
        float end = 0;
        while (timer < 1)
        {
            timer += Time.unscaledDeltaTime * 0.3f;
            skillLearnResultCanvasGroup.alpha = Mathf.Lerp(start, end, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        // UI ����
        skillLearnResultCanvasGroup.alpha = 0;
        skillLearnResultSet.SetActive(false);
    }


    // ��ų ��忡 ���콺 ���� �� ǥ�õǴ� ���� UI
    public void Skill_DescriptionUI(bool isOn, Attack_Base attack, RectTransform nodePos)
    {
        if(isOn)
        {
            skillDescriptionSet.GetComponent<RectTransform>().position = nodePos.position;
            skillDescriptionSet.SetActive(isOn);
            skillDescriptionIcon.sprite = attack.icon;
            skillNameText.text = attack.attackName;
            skillDescriptionText.text = attack.attackDescription_Text;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < attack.damageValue.Length; i++)
            {
                sb.Append($"{attack.damageValue[i].x} ~ {attack.damageValue[i].y}");
                if (i < attack.damageValue.Length - 1)
                {
                    sb.Append("\n");
                }
            }
            skillDamageText.text = sb.ToString();
        }
        else
        {
            skillDescriptionSet.SetActive(isOn);
        }
    }

    public void Menu()
    {
        if (SceneManager.GetActiveScene().name == "Start_Scene")
        {
            return;
        }

        if(Option_Manager.instance.isExitOn)
        {
            Option_Manager.instance.isExitOn = false;
            Option_Manager.instance.exitSet.SetActive(false);

            return;
        }

        if (!isMenuOn)
        {
            // Ŀ�� ��� ����
            Player_Manager.instnace.Cursor_Setting(false);

            isMenuOn = true;

            menuSet.SetActive(true);
            Option_Manager.instance.On_Menu();

            Time.timeScale = 0;
        }
        else
        {
            // Ŀ�� ���
            //Player_Manager.instnace.Cursor_Setting(true);

            isMenuOn = false;

            menuSet.SetActive(false);
            Option_Manager.instance.Off_Menu();

            Time.timeScale = 1;
        }
    }

    public void Menu_Click_Skill()
    {
        //�ɼ�â ����
        Option_Manager.instance.OnOff_Option(false);
        //���â ����
        OnOff_Weapon_Item(false);
        //Ʃ�丮�� ����
        OnOff_Tutorial(false);
        //��ųâ Ű��
        OnOff_Skill(true);
        // Ŭ�� ����
        Player_Sound.instance.SFXPlay_OneShot(Player_Sound.instance.onClick);
    }

    public void Menu_Click_Weapon_Item()
    {
        //�ɼ�â ����
        Option_Manager.instance.OnOff_Option(false);
        //���â Ű��
        OnOff_Weapon_Item(true);
        //��ųâ ����
        OnOff_Skill(false);
        //Ʃ�丮�� ����
        OnOff_Tutorial(false);
        // �������ͽ�â ����
        Set_Inventory_Status_Text();
        // Ŭ�� ����
        Player_Sound.instance.SFXPlay_OneShot(Player_Sound.instance.onClick);
    }

    public void Menu_Click_Tutorial()
    {
        //�ɼ�â ����
        Option_Manager.instance.OnOff_Option(false);
        //���â ����
        OnOff_Weapon_Item(false);
        //��ųâ ����
        OnOff_Skill(false);
        //Ʃ�丮�� �ѱ�
        OnOff_Tutorial(true);
        // Ŭ�� ����
        Player_Sound.instance.SFXPlay_OneShot(Player_Sound.instance.onClick);
    }

    public void OnOff_Weapon_Item(bool isOn)
    {
        isWeapon_ItemOn = isOn;
        weaponItemSet.SetActive(isOn);
    }

    public void OnOff_Skill(bool isOn)
    {
        isSkillOn = isOn;
        skillSet.SetActive(isOn);
    }

    public void OnOff_Tutorial(bool isOn)
    {
        isTutorialOn = isOn;
        tutorialSet.SetActive(isOn);
    }

    // ȣ���ؾ���
    public void Set_Inventory_Status_Text()
    {
        inventory_Hp_Text.text = Player_Manager.instnace.curHp + " / " + Player_Manager.instnace.maxHp;
        inventory_Damage_Text.text = Player_Manager.instnace.PhysicalDamage + " & " + Player_Manager.instnace.MagcialDamage;
        inventory_Defense_Text.text = Player_Manager.instnace.PhysicalDefense + " & " + Player_Manager.instnace.PhysicalDefense;
        inventory_CriticalChance_Text.text = Player_Manager.instnace.CriticalChance.ToString();
        inventory_CriticalMultiplier_Text.text = Player_Manager.instnace.CriticalMultiplier.ToString();
    }

    public void Exit_Game()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion


    // �� UI�� ���� �߰��� ��ġ�ϰ� �۾��ϰ����ϴ�
    #region Turn UI

    public void Turn_StartUI()
    {
        StartCoroutine(TurnStartUICall());
    }

    private IEnumerator TurnStartUICall()
    {
        turnStartUISet.SetActive(true);
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime;
            turnStartResultCanvasGroup.alpha = timer;
            yield return null;
        }
        turnStartResultCanvasGroup.alpha = 1;

        yield return new WaitForSeconds(1f);

        timer = 1;
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            turnStartResultCanvasGroup.alpha = timer;
            yield return null;
        }
        turnStartResultCanvasGroup.alpha = 0;
        turnStartUISet.SetActive(false);

        // �� UI
        Turn_TurnCountUI(1);
    }


    // �� ī��Ʈ UI
    public void Turn_TurnCountUI(int count)
    {
        StartCoroutine(TurnCountUICall(count));
    }

    private IEnumerator TurnCountUICall(int count)
    {
        turnCountUISet.SetActive(true);
        turnCountText.text = $"Turn {count}";

        // Fade In
        float timer = 0;
        float start = 0;
        float end = 1;
        while(timer < 1)
        {
            timer += Time.deltaTime * 1.5f;
            turnCountCanvasGroup.alpha = Mathf.Lerp(start, end, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        turnCountCanvasGroup.alpha = 1;

        // Delay
        yield return new WaitForSeconds(0.85f);

        // Fade Out
        timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * 0.75f;
            turnCountCanvasGroup.alpha = Mathf.Lerp(end, start, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        turnCountCanvasGroup.alpha = 0;

        turnCountUISet.SetActive(false);
    }


    // �÷��̾� �� UI On / Off
    public void Turn_FightSelect(bool isOn)
    {
        // ���� ����Ʈ
        selectListSet.SetActive(isOn);

        // ���� ����
        StartCoroutine(Attack_SlotOnOff(isOn));

        // ���� ����Ʈ
        attackPointSet.SetActive(isOn);

        // ü�¹�
        hpSet.SetActive(isOn);
    }

    private IEnumerator Attack_SlotOnOff(bool isOn)
    {
        attackSlotSet.SetActive(true);

        float timer = 0;
        while (timer < 1)
        {
            timer += 2.5f * Time.deltaTime;
            attackSlotCanvasGroup.alpha = Mathf.Lerp((isOn ? 0 : 1), (isOn ? 1 : 0), EasingFunctions.OutExpo(timer));
            yield return null;
        }
        attackSlotCanvasGroup.alpha = isOn ? 1 : 0;

        attackSlotSet.SetActive(isOn);
    }

    // ���� ����Ʈ �Ҹ� & ����
    public void Turn_AttackPoint()
    {
        // UI ����
        for (int i = 0; i < pointImage.Length; i++)
        {
            pointImage[i].SetActive(false);
        }

        // UI On
        for (int i = 0; i < Player_Manager.instnace.curAttackPoint; i++)
        {
            pointImage[i].SetActive(true);
        }
        attackPointText.text = $"{Player_Manager.instnace.curAttackPoint} / {Player_Manager.instnace.maxAttackPoint}";

    }

    // Hp UI ȣ�� �κ�
    public void HpBar()
    {
        if (hpCoroutine != null)
        {
            StopCoroutine(hpCoroutine);
        }

        hpCoroutine = StartCoroutine(HpBarCall());
    }

    // Hp UI ���� �κ�
    private IEnumerator HpBarCall()
    {
        Debug.Log("�ǰ� UI ȣ��");
        // ���� Hp
        hpBarF.value = Player_Manager.instnace.curHp;
        hpText.text = Player_Manager.instnace.curHp + " / " + Player_Manager.instnace.maxHp;

        // �ֽ�ȭ ������
        float timer = delayTime;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        // ���� Hp
        float start = hpBarB.value;
        float end = Player_Manager.instnace.curHp;
        timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 4f;
            hpBarB.value = Mathf.Lerp(start, end, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        hpBarB.value = end;
        hpBarF.value = end;
    }

    #region �̸� ������ UI
    // �̸� ������ UI On/Off
    public void Turn_UndersideGaugeSetting()
    {
        // ������ UI �ֽ�ȭ
        undersideSlider.value = Player_Manager.instnace.CurUndersideGauge;
        undersideSlider.maxValue = Player_Manager.instnace.MaxUndersideGauge;
    }

    /// <summary>
    /// �ʻ�⸦ �ִ� ��ư Ȱ��ȭ
    /// </summary>
    /// <param name="isOn"></param>
    public void Turn_UndersideButtonOn(bool isOn)
    {
        undersideButton.SetActive(isOn);
    }

    /// <summary>
    /// �ʻ�� ���� Ȱ��ȭ
    /// </summary>
    /// <param name="isOn">��ų On / Off</param>
    /// 

    public void Turn_UndersideSkillOn(bool isOn)
    {
        undersideSkill.SetActive(isOn);
    }

    // �̸� ������ UI ȣ��
    public void UndersideGaugeUI(float curGauge)
    {
        if(gaugeCoroutine != null)
            StopCoroutine(gaugeCoroutine);

        StartCoroutine(UndersideGaugeCall(curGauge));
    }

    // �̸� ������ ����
    private IEnumerator UndersideGaugeCall(float curGauge)
    {
        float startGauge = undersideSlider.value;
        float endGauge = curGauge;
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime;
            undersideSlider.value = Mathf.Lerp(startGauge, endGauge, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        undersideSlider.value = endGauge;
    }
    #endregion

    // �� �̸� UI
    public void StageNameUI(string stageName)
    {
        StartCoroutine(StageNameUICall(stageName));
    }

    private IEnumerator StageNameUICall(string stageName)
    {
        stageNameset.SetActive(true);
        stageNameCanvasGroup.alpha = 0;
        stageNameText.text = stageName;

        // UI ����
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * 0.75f;
            stageNameCanvasGroup.alpha = timer;
            yield return null;
        }

        // ������
        yield return new WaitForSeconds(1f);

        // UI ����
        timer = 1;
        while (timer > 0)
        {
            timer -= Time.deltaTime * 0.75f;
            stageNameCanvasGroup.alpha = timer;
            yield return null;
        }

        stageNameCanvasGroup.alpha = 0;
        stageNameset.SetActive(false);
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

    // �÷��̾� VS �ֳʹ� ���� �� UI
    public void Turn_EngageUI(Object type, Attack_Slot slot, bool isOn)
    {
        // On Off üũ
        if (isOn)
        {
            if(slot == null)
            {
                return;
            }

            if(slot.myAttack == null)
            {
                return;
            }

            // UI Ȱ��ȭ
            exchangeSet.SetActive(true);

            // Ÿ�� �� UI Ȱ��ȭ
            switch (type)
            {
                case Object.Player:
                    pExchangeUISet.SetActive(true);
                    exchange_pIconImage.sprite = slot.myAttack.icon;
                    exchange_pAttackNameText.text = slot.myAttack.attackName;
                    exchange_pDescriptionText.text = slot.myAttack.attackDescription_Text;
                    exchange_pSlotSpeedText.text = slot.slotSpeed.ToString();
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
                    break;

                case Object.Enemy:
                    eExchangeUISet.SetActive(true);
                    exchange_eIconImage.sprite = slot.myAttack.icon;
                    exchange_eAttackNameText.text = slot.myAttack.attackName;
                    exchange_eDescriptionText.text = slot.myAttack.attackDescription_Text;
                    exchange_eSlotSpeedText.text = slot.slotSpeed.ToString();
                    System.Text.StringBuilder sb2 = new System.Text.StringBuilder();
                    for (int i = 0; i < slot.myAttack.damageValue.Length; i++)
                    {
                        sb2.Append($"{slot.myAttack.damageValue[i].x} ~ {slot.myAttack.damageValue[i].y}");
                        if (i < slot.myAttack.damageValue.Length - 1)
                        {
                            sb2.Append("\n");
                        }
                    }
                    exchange_edamageText.text = sb2.ToString();
                    break;
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

    public void Turn_Exchange(bool isExchange)
    {
        exchangeText.text = isExchange ? "�� ����" : "�Ϲ� ����";
    }

    public void Turn_WinningUI(bool isOn, Attack_Slot player, Attack_Slot enemy)
    {
        winningrateSet.SetActive(isOn);
        if (isOn)
        {
            // ������ ���
            (int playerDamage, int pMinDamage, int pMaxDamage) = Player_Manager.instnace.Turn_WinningCheck(player.myAttack);
            (int enemyDamage, int eMinDamage, int eMaxDamage) = enemy.enemy.Turn_WinningCheck(enemy.myAttack);

            // �ּ� ~ �ִ� ������ �Է�
            playerMinMaxDamageText.text = $"{pMinDamage} ~ {pMaxDamage}";
            enemyMinMaxDamageText.text = $"{eMinDamage} ~ {eMaxDamage}";

            // ���Ҹ� üũ �� �ؽ�Ʈ �Է�
            float ratio = (float)playerDamage / enemyDamage;
            // int damageDiff = Mathf.Abs(playerDamage - enemyDamage);
            int damageDiff = playerDamage - enemyDamage;
            Debug.Log(damageDiff);
            if (damageDiff >= 10)
            {
                winningrateText.color = new Color(0.3176471f, 0.5921569f, 0.8000001f, 1);
                winningrateText.text = "�켼";
            }
            else if (damageDiff <= 10)
            {
                winningrateText.color = new Color(1, 0.2783019f, 0.2783019f, 1);
                winningrateText.text = "�Ҹ�";
            }
            else
            {
                winningrateText.color = new Color(1, 1, 1, 1);
                winningrateText.text = "����";
            }
        }
    }

    private void Turn_Winningrate_UI(bool isOn, int pDamage, int eDamage)
    {
        // ���Ҹ� üũ �� �ؽ�Ʈ �Է�
        float ratio = (float)pDamage / eDamage;
        int damageDiff = Mathf.Abs(pDamage - eDamage);
        if (ratio >= 1.3f || damageDiff >= 15)
        {
            winningrateText.text = "�켼";
        }
        else if (ratio <= 0.75f || damageDiff >= 15)
        {
            winningrateText.text = "�Ҹ�";
        }
        else
        {
            winningrateText.text = "����";
        }

        winningrateSet.SetActive(isOn);
    }


    // ���� ���� ��ư Ȱ��ȭ
    public void Turn_AttackButton(bool isOn)
    {
        turn_FightButton.interactable = isOn;
    }

    // ���� ���� ��ư Ŭ��
    public void Turn_AttackButtonChick()
    {
        // ����
        Player_Manager.instnace.UI_Sound_Call(2);

        Player_Manager.instnace.player_Turn.isSelect = false;
    }



    // ���� ���� ���̵� ȣ��
    public void TurnFight_Fade(bool isBossFight)
    {
        if(isFade)
        {
            return;
        }

        StartCoroutine(FadeCall(isBossFight));
    }

    // ���� ���� ���̵� ����
    private IEnumerator FadeCall(bool isBossFight)
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
        yield return new WaitForSeconds(0.15f);


        // Backgrouund Effect On
        a = 0;
        float start = 0;
        float end = 1.5f;
        float cur = 0;
        while (a < 1)
        {
            a += Time.deltaTime * 2.5f;
            cur = Mathf.Lerp(start, end, a);
            background_Material.SetFloat("_VignetteIntensity", cur);
            yield return null;
        }


        // Delay
        yield return new WaitForSeconds(0.5f);


        // Text On
        fadeText_Normal.SetActive(true);


        // Delay
        yield return new WaitForSeconds(0.25f);


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


        // �������� �ƴҶ��� ���̵� �ƿ� ��� ����
        if (!isBossFight)
        {
            a = 1;
            start = 1.5f;
            end = 0;
            cur = 2;
            while (a > 0)
            {
                a -= Time.deltaTime * 1.25f;
                cur = Mathf.Lerp(end, start, a);
                background_Material.SetFloat("_VignetteIntensity", cur);
                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, a);
                yield return null;
            }
        }
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);
        fadeSet.SetActive(false);

        isFade = false;
    }



    // �� UI ȣ��
    public void TurnFight_ExchanageSummary(bool isOn, Attack_Base attackData)
    {
        if(isOn)
        {
            // UI Ȱ��ȭ
            exchangeSummarySet.SetActive(true);
            exchangeSummaryNameText.text = attackData.attackName;
            exchangeSummaryDescriptionText.text = attackData.summaryDescriptionText;
            exchangeSummaryDamageText.text = null;

            // ���� ī��Ʈ Ȱ��ȭ
            for (int i = 0; i < exchangeSummaryAttackCount.Length; i++)
            {
                exchangeSummaryAttackCountborder[i].SetActive(false);
                exchangeSummaryAttackCount[i].SetActive(false);
            }
            for (int i = 0; i < attackData.attackCount; i++)
            {
                exchangeSummaryAttackCountborder[i].SetActive(true);
                exchangeSummaryAttackCount[i].SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < exchangeSummaryAttackCount.Length; i++)
            {
                exchangeSummaryAttackCount[i].GetComponent<Image>().color = Color.white;
            }
            exchangeSummaryNameText.text = null;
            exchangeSummaryDamageText.text = null;
            exchangeSummaryDescriptionText.text = null;
            exchangeSummarySet.SetActive(false);
        }
    }

    /// <summary>
    /// �� ���� ���� �����ϴ� ���� �� ���� -> �⺻ : ��� / �� ���� : ���� / �ı� : ����
    /// </summary>
    /// <param name="attackCount">���������� ǥ���ؾ� �ϴ� ���� ���� ����</param>
    public void TurnFight_ExchangeSummary_AttackCount(int attackCount)
    {
        for (int i = 0; i < exchangeSummaryAttackCount.Length; i++)
        {
            if (i < attackCount)
            {
                exchangeSummaryAttackCount[i].GetComponent<Image>().color = Color.black;
            }
            else
            {
                break;
            }
        }

        exchangeSummaryAttackCount[attackCount].GetComponent<Image>().color = Color.red;
    }


    // �� �ݵ����� ������ ǥ��
    public void TurnFight_ExchangeSummary_ExchangeValue(int damage, float time)
    {
        StartCoroutine(TurnFightExchangeSummaryExchangeValueCall(damage, time));
    }

    private IEnumerator TurnFightExchangeSummaryExchangeValueCall(int damage, float time)
    {
        // ������ �ð����� ���� ī��Ʈ ������ ���� ǥ��
        float timer = 0;
        while (timer < time)
        {
            exchangeSummaryDamageText.text = Random.Range(0, 9).ToString();
            timer += Time.deltaTime;
            yield return null;
        }

        // ������ ǥ��
        exchangeSummaryDamageText.text = damage.ToString();
    }


    // ���ݿ��� ������ ǥ��
    public void TurnFight_ExchangeSummary_Damage(bool isOn, int damage, int curCount, int maxCount, Attack_Base attack)
    {
        if(isOn)
        {
            exchangeSummarySet.SetActive(true);
            exchangeSummaryNameText.text = attack.attackName;
            exchangeSummaryDescriptionText.text = attack.summaryDescriptionText;
            exchangeSummaryDamageText.text = damage.ToString();

            for (int i = 0; i < maxCount; i++)
            {
                exchangeSummaryAttackCount[i].GetComponent<Image>().color = Color.white;
                exchangeSummaryAttackCountborder[i].SetActive(true);
                exchangeSummaryAttackCount[i].SetActive(true);
            }
            exchangeSummaryAttackCount[curCount].GetComponent<Image>().color = Color.white;
            exchangeSummaryAttackCountborder[curCount].SetActive(true);
            exchangeSummaryAttackCount[curCount].SetActive(true);
        }
        else
        {
            exchangeSummarySet.SetActive(false);
        }
    }


    #region Outline
    /// <summary>
    /// �ƿ����� ���ǵ� ��
    /// </summary>
    public void TurnFight_Ouutline_SpeedUp()
    {
        switch (outline)
        {
            case Outline.TypeA:
                for (int i = 0; i < pmoves_UD.Length; i++)
                {
                    pmoves_UD[i].SpeedUp();
                }
                break;
            case Outline.TypeB:
                for (int i = 0; i < pmoves_SS.Length; i++)
                {
                    pmoves_SS[i].SpeedUp();
                }
                break;
        }
    }

    // ���� �� �ƿ����� ����
    public void TurnFight_Outline_Setting(Outline type, bool isOn)
    {
        switch (type)
        {
            case Outline.TypeA:
                outline = Outline.TypeA;
                StartCoroutine(OutlineOnACall(isOn));
                break;

            case Outline.TypeB:
                outline = Outline.TypeB;
                StartCoroutine(OutlineOnBCall(isOn));
                break;
        }
    }

    private IEnumerator OutlineOnACall(bool isOn)
    {
        // UI Ȱ��ȭ
        if(isOn)
        {
            outlineAset.SetActive(true);

            // �� UI
            Vector2 startPos1 = outlineAU_Pos[0].position;
            Vector2 endPos1 = outlineAU_Pos[1].position;

            // �Ʒ� UI
            Vector2 startPos2 = outlineAD_Pos[0].position;
            Vector2 endPos2 = outlineAD_Pos[1].position;

            float timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime * 0.75f;
                outlineA_Image[0].rectTransform.position = Vector2.Lerp(startPos1, endPos1, EasingFunctions.OutExpo(timer));
                outlineA_Image[1].rectTransform.position = Vector2.Lerp(startPos2, endPos2, EasingFunctions.OutExpo(timer));
                yield return null;
            }
            outlineA_Image[0].rectTransform.position = endPos1;
            outlineA_Image[1].rectTransform.position = endPos2;
        }
        else
        {
            // UI ��Ȱ��ȭ
            Vector2 startPos1 = outlineAU_Pos[1].position;
            Vector2 endPos1 = outlineAU_Pos[0].position;

            Vector2 startPos2 = outlineAD_Pos[1].position;
            Vector2 endPos2 = outlineAD_Pos[0].position;

            float timer = 0;
            while(timer < 1)
            {
                timer += Time.deltaTime * 0.5f;
                outlineA_Image[0].rectTransform.position = Vector2.Lerp(startPos1, endPos1, EasingFunctions.OutExpo(timer));
                outlineA_Image[1].rectTransform.position = Vector2.Lerp(startPos2, endPos2, EasingFunctions.OutExpo(timer));
                yield return null;
            }
            outlineA_Image[0].rectTransform.position = endPos1;
            outlineA_Image[1].rectTransform.position = endPos2;

            outlineAset.SetActive(false);
        }
    }

    private IEnumerator OutlineOnBCall(bool isOn)
    {
        // UI Ȱ��ȭ
        if (isOn)
        {
            outlineBset.SetActive(true);

            // �� UI
            Vector2 startPos1 = outlineBU_Pos[0].position;
            Vector2 endPos1 = outlineBU_Pos[1].position;

            // �Ʒ� UI
            Vector2 startPos2 = outlineBD_Pos[0].position;
            Vector2 endPos2 = outlineBD_Pos[1].position;

            float timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime * 0.75f;
                outlineB_Image[0].rectTransform.position = Vector2.Lerp(startPos1, endPos1, EasingFunctions.OutExpo(timer));
                outlineB_Image[1].rectTransform.position = Vector2.Lerp(startPos2, endPos2, EasingFunctions.OutExpo(timer));
                yield return null;
            }
            outlineB_Image[0].rectTransform.position = endPos1;
            outlineB_Image[1].rectTransform.position = endPos2;
        }
        else
        {
            // UI ��Ȱ��ȭ
            Vector2 startPos1 = outlineBU_Pos[1].position;
            Vector2 endPos1 = outlineBU_Pos[0].position;

            Vector2 startPos2 = outlineBD_Pos[1].position;
            Vector2 endPos2 = outlineBD_Pos[0].position;

            float timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime * 0.5f;
                outlineB_Image[0].rectTransform.position = Vector2.Lerp(startPos1, endPos1, EasingFunctions.OutExpo(timer));
                outlineB_Image[1].rectTransform.position = Vector2.Lerp(startPos2, endPos2, EasingFunctions.OutExpo(timer));
                yield return null;
            }
            outlineB_Image[0].rectTransform.position = endPos1;
            outlineB_Image[1].rectTransform.position = endPos2;

            outlineBset.SetActive(false);
        }
    }
    #endregion


    // ������ ��� UI
    public void ItemDropUI(Item_Base itemData, int count)
    {
        // UI ��ȯ
        GameObject obj = Instantiate(itemUI, itemUISpawnPos[0].position, Quaternion.identity);
        obj.transform.parent = transform;

        // UI ���� ����
        obj.GetComponent<Player_ItemUI>().UISetting(itemData, itemUISpawnPos[1], count);
    }
    public void World_SkillPoint()
    {
        skillPointText.text = $"���� ����Ʈ : {Player_Skill_Manager.instance.SkillPoint}";
    }

    /// <summary>
    /// �� ���� �¸� �� ��ų ����Ʈ UI
    /// </summary>
    /// <param name="point"></param>
    public void SkillPointUI(int point)
    {
        StartCoroutine(SkillPointUICall(point));
    }

    private IEnumerator SkillPointUICall(int point)
    {
        skillpointUI.SetActive(true);
        skillpointCanvasGroup.alpha = 0;
        skillpointAddText.text = $"{point} ��ų ����Ʈ�� ȹ���Ͽ����ϴ�.";
        skillPointText.text = $"���� ����Ʈ : {Player_Skill_Manager.instance.SkillPoint}";

        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * 2f;
            skillpointCanvasGroup.alpha = Mathf.Lerp(0, 1, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        yield return new WaitForSeconds(0.85f);

        timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * 0.75f;
            skillpointCanvasGroup.alpha = Mathf.Lerp(1, 0, timer);
            yield return null;
        }

        skillpointUI.SetActive(false);
    }


    // �������� ���� UI
    public void Stage_StartEnd(bool isStart, string stageName)
    { 
        StartCoroutine(isStart ? StageStartCall(stageName) : StageEndCall());
    }

    private IEnumerator StageStartCall(string stageName)
    {
        isFade = true;

        // ���̵� �ƿ�
        if (!fadeSet.activeSelf)
            fadeSet.SetActive(true);

        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);
        float timer = 1;
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, timer);
            yield return null;
        }
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);

        StageNameUI(stageName);

        fadeSet.SetActive(false);
        isFade = false;
    }


    private IEnumerator StageEndCall()
    {
        Debug.Log("Call End");
        isFade = true;

        // UI Ȱ��ȭ
        nextSceneSet.SetActive(true);
        nextSceneText.color = new Color(nextSceneText.color.r, nextSceneText.color.g, nextSceneText.color.b, 0);
        nextSceneImage.color = new Color(nextSceneText.color.r, nextSceneText.color.g, nextSceneText.color.b, 0);

        // �����ڸ� ȿ�� On
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime;
            nextSceneText.color = new Color(nextSceneText.color.r, nextSceneText.color.g, nextSceneText.color.b, timer);
            nextSceneImage.color = new Color(nextSceneText.color.r, nextSceneText.color.g, nextSceneText.color.b, timer);
            yield return null;
        }
        nextSceneText.color = new Color(nextSceneText.color.r, nextSceneText.color.g, nextSceneText.color.b, 1);
        nextSceneImage.color = new Color(nextSceneText.color.r, nextSceneText.color.g, nextSceneText.color.b, 1);

        // ������
        yield return new WaitForSeconds(0.25f);

        // ȭ�� ��ü ���̵�
        fadeSet.SetActive(true);
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);
        timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime;
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, timer);
            yield return null;
        }

        nextSceneSet.SetActive(false);
        isFade = false;
    }


    // ���� �¸� UI ȣ��
    public void TurnFight_Win()
    {
        StartCoroutine(TurnFightWinCall());
    }

    // ���� �¸� UI ����
    private IEnumerator TurnFightWinCall()
    {
        isFade = true;

        // �ؽ�Ʈ ���̵� ��
        winUISet.SetActive(true);
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * 1.5f;
            winUICanvasGroup.alpha = EasingFunctions.OutExpo(timer);
            yield return null;
        }

        // ������
        yield return new WaitForSeconds(0.5f);


        // �ؽ�Ʈ ���̵� �ƿ�
        timer = 1;
        while(timer > 0)
        {
            timer -= Time.deltaTime * 1.5f;
            winUICanvasGroup.alpha = EasingFunctions.OutExpo(timer);
            yield return null;
        }
        winUISet.SetActive(false);


        // ȭ�� ���̵� ��
        fadeSet.SetActive(true);
        timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * 1.5f;
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        // ������
        yield return new WaitForSeconds(0.5f);

        // �÷��̾� ����ġ
        Player_Manager.instnace.Turn_Fight_End();

        // ���� UI ON
        World_UISetting(true);

        // ȭ�� ���̵� �ƿ�
        timer = 1;
        while(timer > 0)
        {
            timer -= Time.deltaTime * 1.5f;
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        isFade = false;
    }


    // ���� �й� UI ȣ��
    public void TurnFight_Lose()
    {
        StartCoroutine(TurnFightLoseCall());
    }

    // ���� �й� UI ���� -> �й� �� üũ����Ʈ or ���� ȭ�� �� 1��
    private IEnumerator TurnFightLoseCall()
    {
        isFade = true;

        // �ؽ�Ʈ ���̵� ��
        loseUISet.SetActive(true);
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.5f;
            loseCanvasGroup.alpha = EasingFunctions.OutExpo(timer);
            yield return null;
        }

        // ������
        yield return new WaitForSeconds(0.5f);

        // ȭ�� ���̵� ��
        fadeSet.SetActive(true);
        timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.5f;
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        // ���� ���� �� ���� �Ѿ����?
        // 1. üũ����Ʈ ����
        // 2. ���� ȭ��
        Scene_Loading.LoadScene("Start_Scene");

        // �Ѿ �� �÷��̾� �ı� ��� �߰��ؾ���!
        Destroy(Player_Manager.instnace.gameObject);
    }
    #endregion
}
