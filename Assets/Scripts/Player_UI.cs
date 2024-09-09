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
    public bool isOptionOn;


    // (���, �κ�, ��ų, �ɼ�) â
    [Header("=== Option UI ===")]
    [SerializeField] private GameObject optionSet;


    // ü�¹� F & B
    [Header("=== Hp UI ===")]
    [SerializeField] private GameObject hpSet;
    [SerializeField] private Slider hpFSlider;
    [SerializeField] private Slider hpBSlider;


    // ���� �ڿ�
    [Header("=== Attack Point UI ===")]
    [SerializeField] private GameObject attackPointSet;
    [SerializeField] private Image[] pointImage;


    // ���� <-> ���� �̵� Fade
    [Header("=== Fade UI ===")]
    [SerializeField] private GameObject fadeSet;
    [SerializeField] private Image fadeImage;
    [SerializeField] private GameObject fadeText_Normal;
    [SerializeField] private GameObject fadeText_Boss;
    [SerializeField] private GameObject fadeText_Effect;
    [SerializeField] private Material background_Material;
    [SerializeField] private Material normalText_Material;
    [SerializeField] private Material bossText_Material;


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
            Fade();
        }
    }

    // ���ʸ� �۾��ϸ� ��
    #region World UI
    public void Option()
    {

    }

    public void Hp()
    {

    }
    #endregion

    // �� UI�� ���� �߰��� ��ġ�ϰ� �۾��ϰ����ϴ�
    #region Turn UI
    public void Attack_Point()
    {

    }

    public void Fade()
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
