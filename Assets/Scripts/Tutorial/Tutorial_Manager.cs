using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TutorialData
{
    [SerializeField] private string tutorialName;
    public GameObject[] tutorial;
    public string[] inputKeycode;
}


public class Tutorial_Manager : MonoBehaviour
{
    public static Tutorial_Manager instance;

    [Header("=== Setting ===")]
    [SerializeField] private TutorialData[] data;
    [SerializeField] private TutorialData[] fightData;
    [SerializeField] private Button fightButton;
    [SerializeField] public bool isWaiting;
    private Coroutine tutocoroutine;

    private void Start()
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

    /// <summary>
    /// Ʃ�丮�� ȣ��
    /// </summary>
    /// <param name="index"></param>
    public void TutorialOn(int index)
    {
        if(tutocoroutine != null)
            Tutorial_Reset();

        Player_Manager.instnace.Cursor_Setting(false);

        tutocoroutine = StartCoroutine(TutorialCall(data[index]));
    }

    /// <summary>
    /// �������̴� Ʃ�丮�� ����
    /// </summary>
    public void Tutorial_Reset()
    {
        if(tutocoroutine != null)
            StopCoroutine(tutocoroutine);

        for (int i = 0; i < data.Length; i++)
        {
            for (int i1 = 0; i1 < data[i].tutorial.Length; i1++)
            {
                data[i].tutorial[i1].SetActive(false);
            }
        }

        Time.timeScale = 1;
    }

    /// <summary>
    /// Ʃ�丮�� ����
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private IEnumerator TutorialCall(TutorialData data)
    {
        Time.timeScale = 0;

        for (int i = 0; i < data.tutorial.Length; i++)
        {
            // Ʃ�丮�� ON
            data.tutorial[i].SetActive(true);

            // Ʃ�� ���� ��� -> �̰� �Ϲ����� �ϸ� �����µ�
            yield return new WaitForSecondsRealtime(0.5f);

            // �Է� ���
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

            Player_Manager.instnace.Cursor_Setting(true);

            // Ʃ�丮�� Off
            data.tutorial[i].SetActive(false);
        }

        Time.timeScale = 1;
    }

    public void Tutorial_Fight()
    {
        StartCoroutine(TutorialFightCall());
    }

    private IEnumerator TutorialFightCall()
    {

        // 1�� Ʃ�� - ��ų ���� ����
        isWaiting = true;
        Player_UI.instance.attackContent[1].click_Action += Click;
        fightData[0].tutorial[0].SetActive(true);
        while (isWaiting)
        {
            yield return null;
        }
        fightData[0].tutorial[0].SetActive(false);


        // 2�� Ʃ�� - ��ų ����Ʈ ����
        isWaiting = true;
        fightData[1].tutorial[0].SetActive(true);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        fightData[1].tutorial[0].SetActive(false);

        // ���� ��ư Ȱ��ȭ ���
        yield return new WaitUntil(() => fightButton.interactable);

        // 3�� Ʃ�� - ���� ��ư Ŭ�� ����
        fightData[2].tutorial[0].SetActive(true);
        fightButton.onClick.AddListener(Fight_Button);
        isWaiting = true;
        while(isWaiting)
        {
            yield return null;
        }
        fightData[2].tutorial[0].SetActive(false);
    }

    public void Click()
    {
        isWaiting = false;
        Player_UI.instance.attackContent[0].click_Action -= Click;
    }

    public void Fight_Button()
    {
        isWaiting = false;
        fightButton.onClick.RemoveListener(Fight_Button);
    }
}
