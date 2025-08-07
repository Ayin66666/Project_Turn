using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Easing.Tweening;
using static Player_UI;
// using UnityEngine.UIElements; // 이거 있어야 마테리얼 쪽 만져서 효과 on/off 가능한듯?

public class Player_UI : MonoBehaviour
{
    public static Player_UI instance;


    [Header("=== UI State ===")]    
    public bool isFade;
    public bool isDialog;

    // 여기 두개 불값으로 옵션창 켜진거 확인 & 옵션창에서 나가기 버튼 누른건지 확인
    public bool isMenuOn;
    public bool isWeapon_ItemOn;
    public bool isSkillOn;
    public bool isTutorialOn;

    public enum Object { None, Player, Enemy }
    public enum Outline { TypeA, TypeB }
    private Outline outline;


    // (장비, 인벤, 스킬, 옵션, 튜토리얼) 창
    [Header("=== Option UI ===")]
    [SerializeField] private GameObject menuSet;
    [SerializeField] private GameObject weaponItemSet;
    [SerializeField] private GameObject skillSet;
    [SerializeField] private GameObject tutorialSet;

    public Item_Description_UI item_Description;
    public GameObject item_Description_UI;


    // 인벤토리 스테이터스
    [Header("=== Inventory Status UI ===")]
    [SerializeField] private Text inventory_Hp_Text;
    [SerializeField] private Text inventory_Damage_Text;
    [SerializeField] private Text inventory_Defense_Text;
    [SerializeField] private Text inventory_CriticalChance_Text;
    [SerializeField] private Text inventory_CriticalMultiplier_Text;


    [Header("=== 아이템 획득 UI ===")]
    [SerializeField] private GameObject itemUI;
    [SerializeField] private RectTransform[] itemUISpawnPos;


    [Header("=== 스킬포인트 획득 UI ===")]
    [SerializeField] private GameObject skillpointUI;
    [SerializeField] private CanvasGroup skillpointCanvasGroup;
    [SerializeField] private Text skillpointAddText;
    [SerializeField] private Text skillPointText;

    // 미니맵
    [Header("=== Mini Map UI ===")]
    [SerializeField] private GameObject miniMapSet;


    [Header("=== Stage Name UI ===")]
    [SerializeField] private GameObject stageNameset;
    [SerializeField] private CanvasGroup stageNameCanvasGroup;
    [SerializeField] private Text stageNameText;


    // 체력바 F & B
    [Header("=== Hp UI ===")]
    [SerializeField] private float delayTime;
    public GameObject hpSet;
    [SerializeField] private Slider hpBarF;
    [SerializeField] private Slider hpBarB;
    [SerializeField] private Text hpText;
    [SerializeField] private Coroutine hpCoroutine;


    // 스킬트리 노드 설명 UI
    [Header("=== 스킬트리 설명 UI ===")]
    [SerializeField] private GameObject skillDescriptionSet;
    [SerializeField] private Image skillDescriptionIcon;
    [SerializeField] private Text skillNameText;
    [SerializeField] private Text skillDamageText;
    [SerializeField] private Text skillDescriptionText;


    [Header("=== 스킬트리 습득 성공 or 실패 UI ===")]
    [SerializeField] private GameObject skillLearnResultSet;
    [SerializeField] private CanvasGroup skillLearnResultCanvasGroup;
    [SerializeField] private Text skillLearnResultText;
    private Coroutine skillLearnResultCoroutine;


    // 공격 포인트 UI
    [Header("=== Attack Point UI ===")]
    [SerializeField] private GameObject attackPointSet;
    [SerializeField] private GameObject[] pointImage;
    [SerializeField] private Text attackPointText;


    // 이면 게이지 & 버튼 UI
    [Header("=== Underside Gauge UI ===")]
    [SerializeField] private Slider undersideSlider;
    [SerializeField] private GameObject undersideButton;
    [SerializeField] private GameObject undersideSkill;
    private Coroutine gaugeCoroutine;


    // 월드 <-> 턴제 이동 Fade UI
    [Header("=== Fade UI ===")]
    [SerializeField] private GameObject fadeSet;
    [SerializeField] private Image fadeImage;
    [SerializeField] private GameObject fadeText_Normal;
    [SerializeField] private GameObject fadeText_Boss;
    [SerializeField] private GameObject fadeText_Effect;
    [SerializeField] private Material background_Material;
    [SerializeField] private Material normalText_Material;
    [SerializeField] private Material bossText_Material;


    // 턴 전투 시작 UI
    [Header("=== 턴 전투 시작 UI ===")]
    [SerializeField] private GameObject turnStartUISet;
    [SerializeField] private CanvasGroup turnStartResultCanvasGroup;


    // 턴 카운트 UI
    [Header("=== 턴 카운트 UI ===")]
    [SerializeField] private GameObject turnCountUISet;
    [SerializeField] private CanvasGroup turnCountCanvasGroup;
    [SerializeField] private Text turnCountText;


    // 턴 전투 플레이어 공격 선택 UI
    [Header("=== Turn Fight Attack Select UI ===")]
    [SerializeField] private GameObject selectListSet;
    [SerializeField] private GameObject attackSlotSet;
    [SerializeField] private Button turn_FightButton;
    [SerializeField] private CanvasGroup attackSlotCanvasGroup;
    public AttackContent_UI[] attackContent;


    // 턴 전투 플레이어 타겟 선택 UI
    [Header("=== Turn Fight Target Select UI ===")]
    [SerializeField] private GameObject targetSelectSet;
    [SerializeField] private Image targetSelectBackgroundImage;
    [SerializeField] private Text targetNameText;


    // 합 UI
    [Header("=== Turn Fight Exchange UI ===")]
    [SerializeField] private GameObject exchangeSet; // 일방 - 합 공격 표시 UI
    [SerializeField] private Text exchangeText;


    [Header("=== 합 유불리 UI ===")]
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


    // 실제 합을 진행할 때 나오는 간략 설명 UI
    [Header("=== Turn Fight Exchange Summary UI ===")]
    [SerializeField] private GameObject exchangeSummarySet;
    [SerializeField] private Text exchangeSummaryNameText;
    [SerializeField] private Text exchangeSummaryDescriptionText;
    [SerializeField] private Text exchangeSummaryDamageText;
    [SerializeField] private GameObject[] exchangeSummaryAttackCount;
    [SerializeField] private GameObject[] exchangeSummaryAttackCountborder;


    [Header("=== 합 진행 간 가장자리 이펙트 ===")]
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


    // 전투 승리 & 패배 UI
    [Header("=== Turn Fight Win & Lose UI ===")]
    [SerializeField] private GameObject winUISet;
    [SerializeField] private CanvasGroup winUICanvasGroup;
    [SerializeField] private GameObject loseUISet;
    [SerializeField] private CanvasGroup loseCanvasGroup;


    [Header("=== 스테이지 변경 UI ===")]
    [SerializeField] private GameObject nextSceneSet;
    [SerializeField] private Image nextSceneImage;
    [SerializeField] private Text nextSceneText;



    // 변경 사항
    // 싱글톤으로 변경함
    // UI 작업은 World UI만
    // UI 컴포넌트 추가할 때 헤더 붙여서

    // 슬롯 속도 집어넣는 기능은 슬롯 스크립트로 이동시킴!


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
        // 초기 UI 셋팅
        hpBarF.maxValue = Player_Manager.instnace.maxHp;
        hpBarF.value = Player_Manager.instnace.maxHp;
        hpBarB.maxValue = Player_Manager.instnace.maxHp;
        hpBarB.value = Player_Manager.instnace.maxHp;
        hpText.text = Player_Manager.instnace.curHp + " / " + Player_Manager.instnace.maxHp;

        // 스킬 포인트
        skillPointText.text = $"남은 포인트 : {Player_Skill_Manager.instance.SkillPoint}";

        // 필살기 게이지
        undersideSlider.maxValue = Player_Manager.instnace.MaxUndersideGauge;
        undersideSlider.value = Player_Manager.instnace.CurUndersideGauge;
    }

    private void Update()
    {
        // Fade 테스트용
        if (Input.GetKeyDown(KeyCode.V))
        {
            Player_Manager.instnace.UnderSideGaugeAdd(15);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Menu();
        }
    }


    // 이쪽만 작업하면 됨
    #region World UI

    // 플레이어 월드 UI On / Off
    public void World_UISetting(bool isOn)
    {
        // 미니맵
        miniMapSet.SetActive(isOn);

        // 퀘스트
        Quset_Manager.instance.QusetUI_Setting(isOn);
    }


    /// <summary>
    /// 스킬 습득 시도 시 성공 실패 여부 (0 = 성공 / 1 = 실패-코스트 부족 / 2 = 실패-선행스킬 안배움 / 3 = 실패-이미배움)
    /// </summary>
    /// <param name="resultIndex"> 0 = 성공 / 1 = 실패-코스트 부족 / 2 = 실패-선행스킬 안배움 / 3 = 실패-이미배움</param>
    public void Skill_LearnResultUI(Attack_Base attack, int resultIndex)
    {
        switch (resultIndex)
        {
            case 0:
                skillLearnResultText.text = attack.attackName + " 습득";
                break;

            case 1:
                skillLearnResultText.text = "코스트 부족";
                break;

            case 2:
                skillLearnResultText.text = "선행 스킬 미비";
                break;

            case 3:
                skillLearnResultText.text = "이미 습득하였습니다.";
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
        // UI 활성화
        skillLearnResultCanvasGroup.alpha = 1;
        skillLearnResultSet.SetActive(true);

        // UI 페이드
        float timer = 0;
        float start = 1;
        float end = 0;
        while (timer < 1)
        {
            timer += Time.unscaledDeltaTime * 0.3f;
            skillLearnResultCanvasGroup.alpha = Mathf.Lerp(start, end, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        // UI 종료
        skillLearnResultCanvasGroup.alpha = 0;
        skillLearnResultSet.SetActive(false);
    }


    // 스킬 노드에 마우스 오버 시 표시되는 설명 UI
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
            // 커서 잠금 해제
            Player_Manager.instnace.Cursor_Setting(false);

            isMenuOn = true;

            menuSet.SetActive(true);
            Option_Manager.instance.On_Menu();

            Time.timeScale = 0;
        }
        else
        {
            // 커서 잠금
            //Player_Manager.instnace.Cursor_Setting(true);

            isMenuOn = false;

            menuSet.SetActive(false);
            Option_Manager.instance.Off_Menu();

            Time.timeScale = 1;
        }
    }

    public void Menu_Click_Skill()
    {
        //옵션창 끄기
        Option_Manager.instance.OnOff_Option(false);
        //장비창 끄기
        OnOff_Weapon_Item(false);
        //튜토리얼 끄기
        OnOff_Tutorial(false);
        //스킬창 키기
        OnOff_Skill(true);
        // 클릭 사운드
        Player_Sound.instance.SFXPlay_OneShot(Player_Sound.instance.onClick);
    }

    public void Menu_Click_Weapon_Item()
    {
        //옵션창 끄기
        Option_Manager.instance.OnOff_Option(false);
        //장비창 키기
        OnOff_Weapon_Item(true);
        //스킬창 끄기
        OnOff_Skill(false);
        //튜토리얼 끄기
        OnOff_Tutorial(false);
        // 스테이터스창 설정
        Set_Inventory_Status_Text();
        // 클릭 사운드
        Player_Sound.instance.SFXPlay_OneShot(Player_Sound.instance.onClick);
    }

    public void Menu_Click_Tutorial()
    {
        //옵션창 끄기
        Option_Manager.instance.OnOff_Option(false);
        //장비창 끄기
        OnOff_Weapon_Item(false);
        //스킬창 끄기
        OnOff_Skill(false);
        //튜토리얼 켜기
        OnOff_Tutorial(true);
        // 클릭 사운드
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

    // 호출해야함
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


    // 턴 UI는 내가 추가로 배치하고 작업하겠읍니다
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

        // 턴 UI
        Turn_TurnCountUI(1);
    }


    // 턴 카운트 UI
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


    // 플레이어 턴 UI On / Off
    public void Turn_FightSelect(bool isOn)
    {
        // 공격 리스트
        selectListSet.SetActive(isOn);

        // 공격 슬롯
        StartCoroutine(Attack_SlotOnOff(isOn));

        // 공격 포인트
        attackPointSet.SetActive(isOn);

        // 체력바
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

    // 어택 포인트 소모 & 충전
    public void Turn_AttackPoint()
    {
        // UI 리셋
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

    // Hp UI 호출 부분
    public void HpBar()
    {
        if (hpCoroutine != null)
        {
            StopCoroutine(hpCoroutine);
        }

        hpCoroutine = StartCoroutine(HpBarCall());
    }

    // Hp UI 동작 부분
    private IEnumerator HpBarCall()
    {
        Debug.Log("피격 UI 호출");
        // 앞쪽 Hp
        hpBarF.value = Player_Manager.instnace.curHp;
        hpText.text = Player_Manager.instnace.curHp + " / " + Player_Manager.instnace.maxHp;

        // 최신화 딜레이
        float timer = delayTime;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        // 뒷쪽 Hp
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

    #region 이면 게이지 UI
    // 이면 게이지 UI On/Off
    public void Turn_UndersideGaugeSetting()
    {
        // 게이지 UI 최신화
        undersideSlider.value = Player_Manager.instnace.CurUndersideGauge;
        undersideSlider.maxValue = Player_Manager.instnace.MaxUndersideGauge;
    }

    /// <summary>
    /// 필살기를 주는 버튼 활성화
    /// </summary>
    /// <param name="isOn"></param>
    public void Turn_UndersideButtonOn(bool isOn)
    {
        undersideButton.SetActive(isOn);
    }

    /// <summary>
    /// 필살기 슬롯 활성화
    /// </summary>
    /// <param name="isOn">스킬 On / Off</param>
    /// 

    public void Turn_UndersideSkillOn(bool isOn)
    {
        undersideSkill.SetActive(isOn);
    }

    // 이면 게이지 UI 호출
    public void UndersideGaugeUI(float curGauge)
    {
        if(gaugeCoroutine != null)
            StopCoroutine(gaugeCoroutine);

        StartCoroutine(UndersideGaugeCall(curGauge));
    }

    // 이면 게이지 동작
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

    // 맵 이름 UI
    public void StageNameUI(string stageName)
    {
        StartCoroutine(StageNameUICall(stageName));
    }

    private IEnumerator StageNameUICall(string stageName)
    {
        stageNameset.SetActive(true);
        stageNameCanvasGroup.alpha = 0;
        stageNameText.text = stageName;

        // UI 등장
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * 0.75f;
            stageNameCanvasGroup.alpha = timer;
            yield return null;
        }

        // 딜레이
        yield return new WaitForSeconds(1f);

        // UI 종료
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


    // 플레이어 타겟 선택 UI On / Off
    public void Turn_TargetSelect(bool isOn)
    {
        targetSelectSet.SetActive(isOn);
    }

    // 플레이어 타겟 선택 데이터 받아오기 & 적용
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

    // 플레이어 VS 애너미 공격 비교 UI
    public void Turn_EngageUI(Object type, Attack_Slot slot, bool isOn)
    {
        // On Off 체크
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

            // UI 활성화
            exchangeSet.SetActive(true);

            // 타입 별 UI 활성화
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
            // UI 종료
            exchangeSet.SetActive(false);
            pExchangeUISet.SetActive(false);
            eExchangeUISet.SetActive(false);
        }
    }

    public void Turn_Exchange(bool isExchange)
    {
        exchangeText.text = isExchange ? "합 공격" : "일방 공격";
    }

    public void Turn_WinningUI(bool isOn, Attack_Slot player, Attack_Slot enemy)
    {
        winningrateSet.SetActive(isOn);
        if (isOn)
        {
            // 데미지 계산
            (int playerDamage, int pMinDamage, int pMaxDamage) = Player_Manager.instnace.Turn_WinningCheck(player.myAttack);
            (int enemyDamage, int eMinDamage, int eMaxDamage) = enemy.enemy.Turn_WinningCheck(enemy.myAttack);

            // 최소 ~ 최대 데미지 입력
            playerMinMaxDamageText.text = $"{pMinDamage} ~ {pMaxDamage}";
            enemyMinMaxDamageText.text = $"{eMinDamage} ~ {eMaxDamage}";

            // 유불리 체크 후 텍스트 입력
            float ratio = (float)playerDamage / enemyDamage;
            // int damageDiff = Mathf.Abs(playerDamage - enemyDamage);
            int damageDiff = playerDamage - enemyDamage;
            Debug.Log(damageDiff);
            if (damageDiff >= 10)
            {
                winningrateText.color = new Color(0.3176471f, 0.5921569f, 0.8000001f, 1);
                winningrateText.text = "우세";
            }
            else if (damageDiff <= 10)
            {
                winningrateText.color = new Color(1, 0.2783019f, 0.2783019f, 1);
                winningrateText.text = "불리";
            }
            else
            {
                winningrateText.color = new Color(1, 1, 1, 1);
                winningrateText.text = "균형";
            }
        }
    }

    private void Turn_Winningrate_UI(bool isOn, int pDamage, int eDamage)
    {
        // 유불리 체크 후 텍스트 입력
        float ratio = (float)pDamage / eDamage;
        int damageDiff = Mathf.Abs(pDamage - eDamage);
        if (ratio >= 1.3f || damageDiff >= 15)
        {
            winningrateText.text = "우세";
        }
        else if (ratio <= 0.75f || damageDiff >= 15)
        {
            winningrateText.text = "불리";
        }
        else
        {
            winningrateText.text = "균형";
        }

        winningrateSet.SetActive(isOn);
    }


    // 전투 시작 버튼 활성화
    public void Turn_AttackButton(bool isOn)
    {
        turn_FightButton.interactable = isOn;
    }

    // 전투 시작 버튼 클릭
    public void Turn_AttackButtonChick()
    {
        // 사운드
        Player_Manager.instnace.UI_Sound_Call(2);

        Player_Manager.instnace.player_Turn.isSelect = false;
    }



    // 전투 시작 페이드 호출
    public void TurnFight_Fade(bool isBossFight)
    {
        if(isFade)
        {
            return;
        }

        StartCoroutine(FadeCall(isBossFight));
    }

    // 전투 시작 페이드 동작
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


        // 보스전이 아닐때만 페이드 아웃 기능 동작
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



    // 합 UI 호출
    public void TurnFight_ExchanageSummary(bool isOn, Attack_Base attackData)
    {
        if(isOn)
        {
            // UI 활성화
            exchangeSummarySet.SetActive(true);
            exchangeSummaryNameText.text = attackData.attackName;
            exchangeSummaryDescriptionText.text = attackData.summaryDescriptionText;
            exchangeSummaryDamageText.text = null;

            // 공격 카운트 활성화
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
    /// 합 도중 현재 공격하는 슬롯 색 변경 -> 기본 : 흰색 / 합 공격 : 빨강 / 파괴 : 검정
    /// </summary>
    /// <param name="attackCount">붉은색으로 표시해야 하는 현재 공격 코인</param>
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


    // 합 격돌에서 데미지 표기
    public void TurnFight_ExchangeSummary_ExchangeValue(int damage, float time)
    {
        StartCoroutine(TurnFightExchangeSummaryExchangeValueCall(damage, time));
    }

    private IEnumerator TurnFightExchangeSummaryExchangeValueCall(int damage, float time)
    {
        // 지정한 시간동안 어택 카운트 무작위 숫자 표기
        float timer = 0;
        while (timer < time)
        {
            exchangeSummaryDamageText.text = Random.Range(0, 9).ToString();
            timer += Time.deltaTime;
            yield return null;
        }

        // 데미지 표기
        exchangeSummaryDamageText.text = damage.ToString();
    }


    // 공격에서 데미지 표기
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
    /// 아웃라인 스피드 업
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

    // 전투 시 아웃라인 연출
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
        // UI 활성화
        if(isOn)
        {
            outlineAset.SetActive(true);

            // 위 UI
            Vector2 startPos1 = outlineAU_Pos[0].position;
            Vector2 endPos1 = outlineAU_Pos[1].position;

            // 아래 UI
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
            // UI 비활성화
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
        // UI 활성화
        if (isOn)
        {
            outlineBset.SetActive(true);

            // 위 UI
            Vector2 startPos1 = outlineBU_Pos[0].position;
            Vector2 endPos1 = outlineBU_Pos[1].position;

            // 아래 UI
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
            // UI 비활성화
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


    // 아이템 드랍 UI
    public void ItemDropUI(Item_Base itemData, int count)
    {
        // UI 소환
        GameObject obj = Instantiate(itemUI, itemUISpawnPos[0].position, Quaternion.identity);
        obj.transform.parent = transform;

        // UI 동작 셋팅
        obj.GetComponent<Player_ItemUI>().UISetting(itemData, itemUISpawnPos[1], count);
    }
    public void World_SkillPoint()
    {
        skillPointText.text = $"남은 포인트 : {Player_Skill_Manager.instance.SkillPoint}";
    }

    /// <summary>
    /// 턴 전투 승리 후 스킬 포인트 UI
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
        skillpointAddText.text = $"{point} 스킬 포인트를 획득하였습니다.";
        skillPointText.text = $"남은 포인트 : {Player_Skill_Manager.instance.SkillPoint}";

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


    // 스테이지 변경 UI
    public void Stage_StartEnd(bool isStart, string stageName)
    { 
        StartCoroutine(isStart ? StageStartCall(stageName) : StageEndCall());
    }

    private IEnumerator StageStartCall(string stageName)
    {
        isFade = true;

        // 페이드 아웃
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

        // UI 활성화
        nextSceneSet.SetActive(true);
        nextSceneText.color = new Color(nextSceneText.color.r, nextSceneText.color.g, nextSceneText.color.b, 0);
        nextSceneImage.color = new Color(nextSceneText.color.r, nextSceneText.color.g, nextSceneText.color.b, 0);

        // 가장자리 효과 On
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

        // 딜레이
        yield return new WaitForSeconds(0.25f);

        // 화면 전체 페이드
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


    // 전투 승리 UI 호출
    public void TurnFight_Win()
    {
        StartCoroutine(TurnFightWinCall());
    }

    // 전투 승리 UI 동작
    private IEnumerator TurnFightWinCall()
    {
        isFade = true;

        // 텍스트 페이드 인
        winUISet.SetActive(true);
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * 1.5f;
            winUICanvasGroup.alpha = EasingFunctions.OutExpo(timer);
            yield return null;
        }

        // 딜레이
        yield return new WaitForSeconds(0.5f);


        // 텍스트 페이드 아웃
        timer = 1;
        while(timer > 0)
        {
            timer -= Time.deltaTime * 1.5f;
            winUICanvasGroup.alpha = EasingFunctions.OutExpo(timer);
            yield return null;
        }
        winUISet.SetActive(false);


        // 화면 페이드 인
        fadeSet.SetActive(true);
        timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * 1.5f;
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        // 딜레이
        yield return new WaitForSeconds(0.5f);

        // 플레이어 원위치
        Player_Manager.instnace.Turn_Fight_End();

        // 월드 UI ON
        World_UISetting(true);

        // 화면 페이드 아웃
        timer = 1;
        while(timer > 0)
        {
            timer -= Time.deltaTime * 1.5f;
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        isFade = false;
    }


    // 전투 패배 UI 호출
    public void TurnFight_Lose()
    {
        StartCoroutine(TurnFightLoseCall());
    }

    // 전투 패배 UI 동작 -> 패배 후 체크포인트 or 메인 화면 중 1택
    private IEnumerator TurnFightLoseCall()
    {
        isFade = true;

        // 텍스트 페이드 인
        loseUISet.SetActive(true);
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.5f;
            loseCanvasGroup.alpha = EasingFunctions.OutExpo(timer);
            yield return null;
        }

        // 딜레이
        yield return new WaitForSeconds(0.5f);

        // 화면 페이드 인
        fadeSet.SetActive(true);
        timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 1.5f;
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, EasingFunctions.OutExpo(timer));
            yield return null;
        }

        // 전투 종료 후 어디로 넘어갈건지?
        // 1. 체크포인트 구역
        // 2. 메인 화면
        Scene_Loading.LoadScene("Start_Scene");

        // 넘어갈 때 플레이어 파괴 기능 추가해야함!
        Destroy(Player_Manager.instnace.gameObject);
    }
    #endregion
}
