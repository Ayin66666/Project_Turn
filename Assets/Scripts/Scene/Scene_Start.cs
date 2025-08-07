using Easing.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene_Start : MonoBehaviour
{
    [Header("=== State ===")]
    [SerializeField] private Page curPage;
    private enum Page { None, Option, Exit }


    [Header("=== UI ===")]
    [SerializeField] private GameObject exitSet;
    [SerializeField] private CanvasGroup startbuttonGroup;
    private Coroutine startbuttonCoroutine;
    private Coroutine charMoveCoroutine;
    private Coroutine bMoveCoroutine;

    // 캐릭터
    [SerializeField] private float speed;
    [SerializeField] private RectTransform imageRect;
    [SerializeField] private RectTransform moveRect;

    // 배경
    [SerializeField] private float bspeed;
    [SerializeField] private RectTransform bimageRect;
    [SerializeField] private RectTransform bmoveRect;

    // 메인 제목
    [SerializeField] private CanvasGroup mainTextCanvasGroup;


    private void Start()
    {
        startbuttonCoroutine = StartCoroutine(StartButtonFade());
        StartCoroutine(MainTextFade());    

        //charMoveCoroutine = StartCoroutine(CharMove());
        //bMoveCoroutine = StartCoroutine(BMove());
    }


    #region 배경 & 캐릭터 움직임
    private IEnumerator CharMove()
    {
        while (true)
        {
            Vector2 startPos = imageRect.anchoredPosition;
            Vector2 endPos = SetRandomTargetPosition(moveRect);
            float timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime * speed;
                imageRect.anchoredPosition = Vector2.Lerp(startPos, endPos, timer);
                yield return null;
            }

            startPos = imageRect.anchoredPosition;
            endPos = SetRandomTargetPosition(moveRect);
            timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime * speed;
                imageRect.anchoredPosition = Vector2.Lerp(startPos, endPos, timer);
                yield return null;
            }
        }
    }

    private IEnumerator BMove()
    {
        while (true)
        {
            Vector2 startPos = bimageRect.anchoredPosition;
            Vector2 endPos = SetRandomTargetPosition(bmoveRect);
            float timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime * bspeed;
                bimageRect.anchoredPosition = Vector2.Lerp(startPos, endPos, timer);
                yield return null;
            }



            startPos = bimageRect.anchoredPosition;
            endPos = SetRandomTargetPosition(bmoveRect);
            timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime * bspeed;
                bimageRect.anchoredPosition = Vector2.Lerp(startPos, endPos, timer);
                yield return null;
            }
        }
    }

    Vector2 SetRandomTargetPosition(RectTransform rect)
    {
        Vector2 minAnchor = rect.rect.min + new Vector2(imageRect.rect.width / 2, imageRect.rect.height / 2);
        Vector2 maxAnchor = rect.rect.max - new Vector2(imageRect.rect.width / 2, imageRect.rect.height / 2);

        // 랜덤한 목표 위치를 설정합니다.
        return new Vector2(Random.Range(minAnchor.x, maxAnchor.x), Random.Range(minAnchor.y, maxAnchor.y));
    }
    #endregion


    private IEnumerator StartButtonFade()
    {
        while (true)
        {
            float start = 0;
            float end = 1;

            float timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime;
                startbuttonGroup.alpha = Mathf.Lerp(start, end, EasingFunctions.InOutExpo(timer));
                yield return null;
            }

            yield return new WaitForSeconds(0.15f);

            start = 1;
            end = 0;
            timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime;
                startbuttonGroup.alpha = Mathf.Lerp(start, end, EasingFunctions.InOutExpo(timer));
                yield return null;
            }

            yield return new WaitForSeconds(0.15f);
        }
    }

    private IEnumerator MainTextFade()
    {
        while (true)
        {
            int ran = 0;
            for (int i = 0; i < 3; i++)
            {
                // 암전

                mainTextCanvasGroup.alpha = 1;
                float speed = Random.Range(0.45f, 0.75f);
                float start = ran == 0 ? 0 : 1;
                float end = ran == 0 ? 1 : 0;

                float timer = 0;
                while (timer < 1)
                {
                    timer += Time.deltaTime * speed;
                    mainTextCanvasGroup.alpha = Mathf.Lerp(start, end, EasingFunctions.InOutElastic(timer));
                    yield return null;
                }

                ran = ran == 0 ? 1 : 0;
            }

            // 딜레이
            yield return new WaitForSeconds(Random.Range(3.5f, 5.5f));
        }
    }

    #region 버튼 이벤트
    public void Click_Start()
    {
        Scene_Loading.LoadScene("Tutorial_Scene");
    }

    public void Click_Option()
    {
        Option_Manager.instance.On_Menu();
        Player_Sound.instance.SFXPlay_OneShot(Player_Sound.instance.onClick);
    }

    public void Click_Exit()
    {
        exitSet.SetActive(true);
        Player_Sound.instance.SFXPlay_OneShot(Player_Sound.instance.onClick);
    }

    public void Click_ExitOff()
    {
        exitSet.SetActive(false);
        Player_Sound.instance.SFXPlay_OneShot(Player_Sound.instance.onClick);
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
}
