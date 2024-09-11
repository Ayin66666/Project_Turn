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
    public bool isOptionOn;


    // (장비, 인벤, 스킬, 옵션) 창
    [Header("=== Option UI ===")]
    [SerializeField] private GameObject optionSet;


    // 체력바 F & B
    [Header("=== Hp UI ===")]
    [SerializeField] private GameObject hpSet;
    [SerializeField] private Slider hpFSlider;
    [SerializeField] private Slider hpBSlider;


    // 전투 자원
    [Header("=== Attack Point UI ===")]
    [SerializeField] private GameObject attackPointSet;
    [SerializeField] private GameObject[] pointImage;


    // 월드 <-> 턴제 이동 Fade
    [Header("=== Fade UI ===")]
    [SerializeField] private GameObject fadeSet;
    [SerializeField] private Image fadeImage;
    [SerializeField] private GameObject fadeText_Normal;
    [SerializeField] private GameObject fadeText_Boss;
    [SerializeField] private GameObject fadeText_Effect;
    [SerializeField] private Material background_Material;
    [SerializeField] private Material normalText_Material;
    [SerializeField] private Material bossText_Material;


    [Header("=== Select UI ===")]
    [SerializeField] private GameObject selectSet;


    [Header("=== Engage UI ===")]
    [SerializeField] private GameObject engageUISet;


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
    }

    // 이쪽만 작업하면 됨
    #region World UI
    public void Option()
    {

    }

    public void Hp()
    {

    }
    #endregion


    // 턴 UI는 내가 추가로 배치하고 작업하겠읍니다
    #region Turn UI

    // 플레이어 턴 UI On / Off
    public void TurnFight_Select(bool isOn)
    {
        selectSet.SetActive(isOn);
    }

    // 전투 자원 UI
    public void Attack_Point()
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

    // 플레이어 VS 애너미 공격 비교 UI
    public void EngageUI(Attack_Slot slot, bool isOn)
    {
        // 슬롯에서 데이터를 받아와서 동작
        // 플레이어 & 몬스터 공격 데이터를 받아와서 사용
        // 데이터는 플레이어의 공격 슬롯에 myAttack 과 target 에 있음!
        // 데이터를 받은 뒤, 각각 비교 UI에 삽입

        if (isOn)
        {
            engageUISet.SetActive(true);
        }
        else
        {
            engageUISet.SetActive(false);
        }
    }

    // 전투 시작 페이드
    public void TurnFight_Fade()
    {
        if(isFade)
        {
            return;
        }

        StartCoroutine(FadeCall());
    }

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
