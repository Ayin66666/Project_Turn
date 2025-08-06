using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Easing.Tweening;


public class Dialog_Manager : MonoBehaviour
{
    public static Dialog_Manager instance;

    [Header("=== Dialog Setting ===")]
    [SerializeField] private List<Dialog_Data> data;
    [SerializeField] private bool isDialog;
    private Coroutine dialogCoroutine;


    [Header("=== 다이얼로그 UI ===")]
    [SerializeField] private GameObject dialogSet;
    [SerializeField] private Image dialogPortraitImage;
    [SerializeField] private Image dialogNameBorder;
    [SerializeField] private Text nameText;
    [SerializeField] private Text dialogText;
    private RectTransform rectPos;
    [SerializeField] private RectTransform[] movePos;
    [SerializeField] private Sprite[] portraitSprite;


    [Header("=== 사운드 ===")]
    private AudioSource dialog_Audio;
    [SerializeField] private AudioClip[] dialog_Sound;


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

        dialog_Audio = GetComponent<AudioSource>();
        rectPos = dialogSet.GetComponent<RectTransform>();
    }


    public void Dialog(int dialog_Index)
    {
        if (dialogCoroutine != null)
        {
            StopCoroutine(dialogCoroutine);
        }

        dialogCoroutine = StartCoroutine(DialogCall(dialog_Index));
    }

    public void DialogOff()
    {
        if (dialogCoroutine != null)
        {
            StopCoroutine(dialogCoroutine);
        }

        dialogSet.SetActive(false);
    }

    private IEnumerator DialogCall(int dialog_Index)
    {
        isDialog = true;
        bool isFrist = true;

        // 대사 수 만큼 동작
        for (int i = 0; i < data[dialog_Index].line_Data.Count; i++)
        {
            // 시작 기능 체크
            if (data[dialog_Index].line_Data[i].haveEvnet)
            {
                // 이벤트 타입 체크
                switch (data[dialog_Index].line_Data[i].evnetType)
                {
                    case Line_Data.Evnet_Type.Door:
                        Stage_Manager.instance.DoorOpen(data[dialog_Index].line_Data[i].eventIndex);
                        break;

                    case Line_Data.Evnet_Type.WayPoint:
                        Waypoint_Manager.instance.Waypoint_Setting(true, data[dialog_Index].line_Data[i].eventIndex);
                        break;

                    case Line_Data.Evnet_Type.Portal:
                        Stage_Manager.instance.NextStagePortal(true);
                        break;

                    case Line_Data.Evnet_Type.Quset:
                        Quset_Manager.instance.Quset_Setting(data[dialog_Index].line_Data[i].eventIndex);
                        break;

                    case Line_Data.Evnet_Type.Tutorial:
                        Tutorial_Manager.instance.TutorialOn(data[dialog_Index].line_Data[i].eventIndex);
                        break;
                }
            }

            // 이벤트만 있는 데이터의 경우 - 출력 X
            if (data[dialog_Index].line_Data[i].type != Line_Data.LineType.None)
            {
                dialogSet.SetActive(true);

                // UI 셋팅
                dialogPortraitImage.sprite = data[dialog_Index].line_Data[i].icon;
                nameText.text = data[dialog_Index].line_Data[i].name_Text;
                dialogText.text = data[dialog_Index].line_Data[i].dialog_Text;
                dialogText.color = new Color(dialogText.color.r, dialogText.color.g, dialogText.color.b, 1);
                dialogNameBorder.sprite = data[dialog_Index].line_Data[i].type == Line_Data.LineType.Enemy ? portraitSprite[1] : portraitSprite[0];

                // UI 동작 - 최초 1회 실행
                if(isFrist)
                {
                    // Sound
                    dialog_Audio.PlayOneShot(dialog_Sound[0]);

                    isFrist = false;
                    float timer = 0;
                    while (timer < 1)
                    {
                        timer += Time.deltaTime * 1.5f;
                        dialogSet.transform.position = Vector2.Lerp(movePos[0].position, movePos[1].position, EasingFunctions.OutExpo(timer));
                        yield return null;
                    }
                    rectPos.transform.position = movePos[1].position;
                }

                // 종료 기능 체크
                if (data[dialog_Index].line_Data[i].haveEvnet)
                {
                    // 이벤트 타입 체크
                    switch (data[dialog_Index].line_Data[i].evnetType)
                    {
                        case Line_Data.Evnet_Type.Door:
                            Stage_Manager.instance.DoorOpen(data[dialog_Index].line_Data[i].eventIndex);
                            break;

                        case Line_Data.Evnet_Type.WayPoint:
                            Waypoint_Manager.instance.Waypoint_Setting(true, data[dialog_Index].line_Data[i].eventIndex);
                            break;
                    }
                }
            }

            // 다음 텍스트 출력 딜레이
            yield return new WaitForSeconds(data[dialog_Index].line_Data[i].textDelay);

            // 대사 페이드 아웃으로 사라지기
            if (data[dialog_Index].line_Data[i].type != Line_Data.LineType.None)
            {
                float t = 1;
                while (t > 0)
                {
                    t -= Time.deltaTime * 3f;
                    dialogText.color = new Color(dialogText.color.r, dialogText.color.g, dialogText.color.b, t);
                    yield return null;
                }

                yield return new WaitForSeconds(0.15f);
            }
        }

        // Sound
        dialog_Audio.PlayOneShot(dialog_Sound[0]);

        // UI Off
        float t1 = 0;
        while (t1 < 1)
        {
            t1 += Time.deltaTime * 1.25f;
            dialogSet.transform.position = Vector2.Lerp(movePos[1].position, movePos[0].position, EasingFunctions.OutExpo(t1));
            yield return null;
        }
        rectPos.transform.position = movePos[1].position;

        dialogSet.SetActive(false);
        isDialog = false;
    }
}
