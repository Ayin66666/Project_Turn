using Easing.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
// using UnityEngine.UIElements; // 이거 있어야 마테리얼 쪽 만져서 효과 on/off 가능한듯?

public class Player_UI : MonoBehaviour
{
    public static Player_UI instance;


    [Header("=== UI State ===")]
    public bool isFade;

    // 여기 두개 불값으로 옵션창 켜진거 확인 & 옵션창에서 나가기 버튼 누른건지 확인
    public bool isMenuOn;
    public bool isWeapon_ItemOn;
    public bool isSkillOn;
    public bool isOptionOn;
    public bool isExitOn;
    public bool isFpsOn;
    public enum Object { None, Player, Enemy }


    // (장비, 인벤, 스킬, 옵션) 창
    [Header("=== Option UI ===")]
    [SerializeField] private GameObject menuSet;
    [SerializeField] private GameObject weaponItemSet;
    [SerializeField] private GameObject optionSet;
    [SerializeField] private GameObject skillSet;
    [SerializeField] private GameObject exitSet;
    [SerializeField] private Toggle fpsToggle;

    private float deltaTime = 0f;


    // 체력바 F & B
    [Header("=== Hp UI ===")]
    [SerializeField] private GameObject hpSet;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Text hpText;


    // 스킬트리 창 설명 UI
    [Header("=== Skill UI ===")]
    [SerializeField] private GameObject skillDescriptionSet;
    [SerializeField] private Image skillDescriptionIcon;
    [SerializeField] private Text skillNameText;
    [SerializeField] private Text skillDamageText;
    [SerializeField] private Text skillDescriptionText;


    // 공격 포인트 UI
    [Header("=== Attack Point UI ===")]
    [SerializeField] private GameObject attackPointSet;
    [SerializeField] private GameObject[] pointImage;


    // 이면 게이지 & 버튼 UI
    [Header("=== Underside Gauge UI ===")]
    [SerializeField] private GameObject undersideSet;
    [SerializeField] private Slider undersideSlider;
    [SerializeField] private GameObject undersideButton;
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


    // 턴 전투 플레이어 공격 선택 UI
    [Header("=== Turn Fight Attack Select UI ===")]
    [SerializeField] private GameObject selectListSet;
    [SerializeField] private GameObject attackSlotSet;
    [SerializeField] private Button turn_FightButton;


    // 턴 전투 플레이어 타겟 선택 UI
    [Header("=== Turn Fight Target Select UI ===")]
    [SerializeField] private GameObject targetSelectSet;
    [SerializeField] private Image targetSelectBackgroundImage;
    [SerializeField] private Text targetNameText;


    // 합 UI
    [Header("=== Turn Fight Exchange UI ===")]
    // 일방 - 합 공격 표시 UI
    [SerializeField] private GameObject exchangeSet; 
    [SerializeField] private Text exchangeText;


    [Header("=== Turn Fight Exchange Player UI ===")]
    [SerializeField] private GameObject pExchangeUISet;
    [SerializeField] private Image exchange_pIconImage;
    [SerializeField] private Text exchange_pAttackNameText;
    [SerializeField] private Text exchange_pdamageText;
    [SerializeField] private Text exchange_pDescriptionText;


    [Header("=== Turn Fight Exchange Enemy UI ===")]
    [SerializeField] private GameObject eExchangeUISet;
    [SerializeField] private Image exchange_eIconImage;
    [SerializeField] private Text exchange_eAttackNameText;
    [SerializeField] private Text exchange_edamageText;
    [SerializeField] private Text exchange_eDescriptionText;


    // 전투 승리 & 패배 UI
    [Header("=== Turn Fight Win & Lose UI ===")]
    [SerializeField] private GameObject winUISet;
    [SerializeField] private CanvasGroup winUICanvasGroup;
    [SerializeField] private GameObject loseUISet;
    [SerializeField] private CanvasGroup loseCanvasGroup;

    // 변경 사항
    // 싱글톤으로 변경함
    // UI 작업은 World UI만
    // UI 컴포넌트 추가할 때 헤더 붙여서


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
        // Fade 테스트용
        if (Input.GetKeyDown(KeyCode.V))
        {
            TurnFight_Lose();
        }

        Menu();

        //FPS 표시 및 설정용
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    // 이쪽만 작업하면 됨
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
        // 월드에서는 한번만 호출하는용 만들기
        // 턴에서만 업데이트

        hpSlider.value = ((float)Player_Manager.instnace.curHp / (float)Player_Manager.instnace.maxHp);

        hpText.text = Player_Manager.instnace.curHp + " / " + Player_Manager.instnace.maxHp;
    }

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isMenuOn)
            {
                isMenuOn = true;
                menuSet.SetActive(true);
            }
            else
            {
                isMenuOn = false;
                menuSet.SetActive(false);
            }
        }
    }

    public void Menu_Click_Exit()
    {
        isExitOn = true;
        exitSet.SetActive(true);
    }

    public void Menu_Click_ExitOff()
    {
        isExitOn = false;
        exitSet.SetActive(false);
    }

    public void Menu_Click_Option()
    {
        isOptionOn = true;
        optionSet.SetActive(true);
    }

    public void Menu_Click_Skill()
    {
        isSkillOn = true;
        skillSet.SetActive(true);
    }

    public void Menu_Click_Weapon_Item()
    {
        isWeapon_ItemOn = true;
        weaponItemSet.SetActive(true);
    }

    public void Weapon_ItemOff()
    {
        isWeapon_ItemOn = false;
        weaponItemSet.SetActive(false);
    }

    public void SkillOff()
    {
        isSkillOn = false;
        skillSet.SetActive(false);
    }

    public void OptionOff()
    {
        isOptionOn = false;
        optionSet.SetActive(false);
    }

    public void FpsOn()
    {
        // 작업중
        if (fpsToggle.isOn)
        {
            isFpsOn = true;
        }
        else
        {
            isFpsOn = false;
        }
    }

    private void OnGUI()
    {
        if (isFpsOn)
        {
            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(30, 30, Screen.width, Screen.height);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = 25;
            style.normal.textColor = Color.red;

            float ms = deltaTime * 1000f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.} FPS ({1:0.0} ms)", fps, ms);

            GUI.Label(rect, text, style);
        }
        else
        {
            return;
        }
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

    // 플레이어 턴 UI On / Off
    public void Turn_FightSelect(bool isOn)
    {
        selectListSet.SetActive(isOn);
        attackSlotSet.SetActive(isOn);
    }

    // 슬롯 UI 호출 & 동작 -> 여기부터 작업!
    public void Turn_SlotSpeed()
    {
        for (int i = 0; i < Player_Manager.instnace.player_Turn.attackSlot.Count; i++)
        {

        }
    }

    // 어택 포인트 UI 호출 & 동작
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

    // 이면 게이지 UI 호출
    public void UndersideGaugeUI(float addGauge)
    {
        if(gaugeCoroutine != null)
            StopCoroutine(gaugeCoroutine);

        StartCoroutine(UndersideGaugeCall(addGauge));
    }

    // 이면 게이지 동작
    private IEnumerator UndersideGaugeCall(float addGauge)
    {
        float startGauge = undersideSlider.value;
        float endGauge = addGauge;
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime;
            undersideSlider.value = Mathf.Lerp(startGauge, endGauge, timer);
            yield return null;
        }

        undersideSlider.value = endGauge;
    }

    // 이번 게이지 공격 버튼 UI 활성화
    public void UndersideButtonOn(bool isOn)
    {
        undersideButton.SetActive(isOn);
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
                    break;

                case Object.Enemy:
                    eExchangeUISet.SetActive(true);
                    exchange_eIconImage.sprite = slot.myAttack.icon;
                    exchange_eAttackNameText.text = slot.myAttack.attackName;
                    exchange_eDescriptionText.text = slot.myAttack.attackName;
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

            // 합 여부 UI 셋팅
            switch (slot.attackType)
            {
                case Attack_Slot.AttackType.Oneside_Attack:
                    exchangeText.text = "일방 공격";
                    break;

                case Attack_Slot.AttackType.Exchange_Attacks:
                    exchangeText.text = "합 공격";
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



    // 전투 시작 버튼 활성화
    public void Turn_AttackButton(bool isOn)
    {
        turn_FightButton.interactable = isOn;
    }

    // 전투 시작 버튼 클릭
    public void Turn_AttackButtonChick()
    {
        Player_Manager.instnace.player_Turn.isSelect = false;
    }



    // 전투 시작 페이드 호출
    public void TurnFight_Fade()
    {
        if(isFade)
        {
            return;
        }

        StartCoroutine(FadeCall());
    }

    // 전투 시작 페이드 동작
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

        // 화면 페이드 아웃
        timer = 1;
        while(timer > 0)
        {
            timer -= Time.deltaTime * 1.5f;
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, EasingFunctions.OutExpo(timer));
            yield return null;
        }
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
    }
    #endregion
}
