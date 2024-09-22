using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Easing.Tweening;

public class Enemy_UI : MonoBehaviour
{
    [SerializeField] private Enemy_Base enemy;
    private CanvasGroup canvasGroup;


    [Header("=== Hp Bar UI ===")]
    [SerializeField] private float delayTime;
    [SerializeField] private float curTime;
    [SerializeField] private GameObject hpBarSet;
    [SerializeField] private Slider hpBarF;
    [SerializeField] private Slider hpBarB;
    private Coroutine hpCortoutine;


    [Header("=== Name UI ===")]
    [SerializeField] private GameObject nameSet;
    [SerializeField] private Text nameText;


    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Hp UI ȣ�� �κ�
    public void HpBar()
    {
        if(hpCortoutine != null)
        {
            StopCoroutine(hpCortoutine);
        }

        hpCortoutine = StartCoroutine(HpBarCall());
    }

    // Hp UI ���� �κ�
    private IEnumerator HpBarCall()
    {
        // ���� Hp
        hpBarF.value = enemy.hp;

        // �ֽ�ȭ ������
        while(curTime > 0)
        {
            curTime -= Time.deltaTime;
            yield return null;
        }

        // ���� Hp
        float start = hpBarB.value;
        float end = enemy.hp;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 4f;
            hpBarB.value = Mathf.Lerp(start, end, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        hpBarB.value = enemy.hp;
    }


    // ���� ���� �� UI Ȱ��ȭ & ���� ���� �� UI ��Ȱ��ȭ
    public void UI_Setting(bool isOn)
    {
        if (isOn)
        {
            nameText.text = enemy.status.EnemyName;
            hpBarF.value = enemy.status.Hp;
            hpBarB.value = enemy.status.Hp;
        }

        nameSet.SetActive(isOn);
        hpBarSet.SetActive(isOn);

        // UI ���İ� ���� ȣ��
        StartCoroutine(UIFade(isOn));
    }

    // UI ���İ� ���� ����
    private IEnumerator UIFade(bool isOn)
    {
        float timer = 0;
        if (isOn)
        {
            timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime;
                canvasGroup.alpha = timer;
                yield return null;
            }
        }
        else
        {
            timer = 1;
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                canvasGroup.alpha = timer;
                yield return null;
            }
        }
    }
}
