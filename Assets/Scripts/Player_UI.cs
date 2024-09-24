using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal; // 이거 있어야 마테리얼 쪽 만져서 효과 on/off 가능한듯?

public class Player_UI : MonoBehaviour
{
    public static Player_UI instance;

    [Header("=== UI State ===")]
    public bool isFade;

    // 여기 두개 불값으로 옵션창 켜진거 확인 & 옵션창에서 나가기 버튼 누른건지 확인
    public bool isOptionOn;
    public bool isExitOn;


    // (장비, 인벤, 스킬, 옵션) 창
    [Header("=== Option UI ===")]
    [SerializeField] private GameObject optionSet;
    [SerializeField] private GameObject waponItemSet;
    [SerializeField] private GameObject skillSet;
    [SerializeField] private GameObject exitSet;


    // 체력바 F & B
    [Header("=== Hp UI ===")]
    [SerializeField] private GameObject hpSet;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Text hpText;


    // 전투 자원
    [Header("=== Attack Point UI ===")]
    [SerializeField] private GameObject attackPointSet;
    [SerializeField] private GameObject[] pointImage;


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
    [Header("=== Turn Fight Select UI ===")]
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

    // 플레이어 합 UI
    [SerializeField] private GameObject pExchangeUISet;
    [SerializeField] private Image exchange_pIconImage;
    [SerializeField] private Text exchange_pAttackNameText;
    [SerializeField] private Text exchange_pdamageText;
    [SerializeField] private Text exchange_pDescriptionText;
    // 에너미 합 UI
    [SerializeField] private GameObject eExchangeUISet;
    [SerializeField] private Image exchange_eIconImage;
    [SerializeField] private Text exchange_eAttackNameText;
    [SerializeField] private Text exchange_edamageText;
    [SerializeField] private Text exchange_eDescriptionText;


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
            TurnFight_Fade();
        }

        Option();

        // HP 테스트용
        Hp();
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
    #endregion


    // 턴 UI는 내가 추가로 배치하고 작업하겠읍니다
    #region Turn UI

    // 플레이어 턴 UI On / Off
    public void Turn_FightSelect(bool isOn)
    {
        selectListSet.SetActive(isOn);
        attackSlotSet.SetActive(isOn);
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

    // 전투 자원 UI
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

    // 플레이어 VS 애너미 공격 비교 UI -> 여기 작업중 -> 동작 테스트 필요함!
    public void Turn_EngageUI(Attack_Slot slot, bool isOn)
    {
        // 슬롯에서 데이터를 받아와서 동작
        // 플레이어 & 몬스터 공격 데이터를 받아와서 사용
        // 데이터는 플레이어의 공격 슬롯에 myAttack 과 target 에 있음!
        // 데이터를 받은 뒤, 각각 비교 UI에 삽입

        if(isOn)
        {
            // 플레이어 & 합 여부 UI 셋팅
            if (slot.myAttack != null)
            {
                // 합 여부 UI 셋팅
                exchangeSet.SetActive(true);
                switch (slot.attackType)
                {
                    case Attack_Slot.AttackType.Oneside_Attack:
                        exchangeText.text = "일방 공격";
                        break;

                    case Attack_Slot.AttackType.Exchange_Attacks:
                        exchangeText.text = "합 공격";
                        break;
                }

                // 플레이어 UI 셋팅
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


            // 몬스터 UI 셋팅
            if (slot.targetSlot != null)
            {
                if (slot.attackType == Attack_Slot.AttackType.Exchange_Attacks)
                {
                    // 몬스터 합 공격 받아오기
                    Attack_Base enemyAttack = slot.targetSlot.myAttack;

                    // UI 셋팅
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

    #endregion
}
