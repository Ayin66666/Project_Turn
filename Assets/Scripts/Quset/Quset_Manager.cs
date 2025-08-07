using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quset_Manager : MonoBehaviour
{
    public static Quset_Manager instance;

    [Header("=== Setting ===")]
    [SerializeField] private List<Quset_Data> data;


    [Header("=== Quset UI ===")]
    [SerializeField] private GameObject qusetSet;
    [SerializeField] private Text qusetNameText;
    [SerializeField] private Text qusetDescriptionText;
    [SerializeField] private CanvasGroup qusetCanvasGroup;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ����Ʈ UI On/Off
    public void QusetUI_Setting(bool isOn)
    {
        StartCoroutine(isOn ? On(): Off());
    }

    private IEnumerator On()
    {
        qusetSet.SetActive(true);
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            qusetCanvasGroup.alpha = timer;
            yield return null;
        }
        qusetCanvasGroup.alpha = 1;
    }

    private IEnumerator Off()
    {
        float timer = 1;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            qusetCanvasGroup.alpha = timer;
            yield return null;
        }
        qusetCanvasGroup.alpha = 0;

        qusetSet.SetActive(false);
    }

    // ����Ʈ �ֽ�ȭ ȣ�� -> �̰� �������� ���� �ϳ��� �������� ����!
    public void Quset_Setting(int index)
    {
        StartCoroutine(On());
        qusetNameText.text = data[index].QusetName;
        qusetDescriptionText.text = data[index].QusetDescription;
    }
}
