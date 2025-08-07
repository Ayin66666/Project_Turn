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
    /// 튜토리얼 호출
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
    /// 실행중이던 튜토리얼 리셋
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
    /// 튜토리얼 동작
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private IEnumerator TutorialCall(TutorialData data)
    {
        Time.timeScale = 0;

        for (int i = 0; i < data.tutorial.Length; i++)
        {
            // 튜토리얼 ON
            data.tutorial[i].SetActive(true);

            // 튜토 씹힘 대기 -> 이거 일반으로 하면 씹히는듯
            yield return new WaitForSecondsRealtime(0.5f);

            // 입력 대기
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

            Player_Manager.instnace.Cursor_Setting(true);

            // 튜토리얼 Off
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

        // 1번 튜토 - 스킬 선택 지시
        isWaiting = true;
        Player_UI.instance.attackContent[1].click_Action += Click;
        fightData[0].tutorial[0].SetActive(true);
        while (isWaiting)
        {
            yield return null;
        }
        fightData[0].tutorial[0].SetActive(false);


        // 2번 튜토 - 스킬 포인트 설명
        isWaiting = true;
        fightData[1].tutorial[0].SetActive(true);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        fightData[1].tutorial[0].SetActive(false);

        // 공격 버튼 활성화 대기
        yield return new WaitUntil(() => fightButton.interactable);

        // 3번 튜토 - 공격 버튼 클릭 지시
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
