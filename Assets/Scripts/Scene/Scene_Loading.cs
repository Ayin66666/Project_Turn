using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Scene_Loading : MonoBehaviour
{
    public static Scene_Loading instance;
    [SerializeField] static string nextScene;
    public static int curStage;
    private bool isLoading;

    private Coroutine tipCoroutine;

    [Header("---Loading Image---")]
    [SerializeField] private Slider progressbar;
    [SerializeField] private Text loadText;
    [SerializeField] private Text tipText;
    [SerializeField][TextArea] private string[] texts;


    [Header("=== 페이드 ===")]
    [SerializeField] private Image fadeImage;


    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        StartCoroutine(Loading());
    }

    public static void LoadScene(string sceneName)
    {
        // Next Scene Name Setting 
        curStage++;
        nextScene = sceneName;
        SceneManager.LoadScene("Loading_Scene");
    }

    public static void ReturnMain()
    {
        Time.timeScale = 1;
        curStage = 0;
        nextScene = "Start_Scene";
        SceneManager.LoadScene("Loading_Scene");
    }

    IEnumerator Tip()
    {
        // Tip Text Fade Setting
        tipText.color = new Color(tipText.color.r, tipText.color.g, tipText.color.b, 0);
        while (isLoading)
        {
            // 렌덤 텍스트 선택
            tipText.text = texts[Random.Range(0, texts.Length)];

            // 페이드 인
            float a = 0;
            while (a < 1)
            {
                a += Time.deltaTime;
                tipText.color = new Color(tipText.color.r, tipText.color.g, tipText.color.b, a);
                yield return null;
            }
            tipText.color = new Color(tipText.color.r, tipText.color.g, tipText.color.b, 1);

            // 딜레이
            yield return new WaitForSeconds(2f);

            // 페이드 아웃
            a = 1;
            while (a > 0)
            {
                a -= Time.deltaTime;
                tipText.color = new Color(tipText.color.r, tipText.color.g, tipText.color.b, a);
                yield return null;
            }
            tipText.color = new Color(tipText.color.r, tipText.color.g, tipText.color.b, 0);

            // 다음 텍스트 딜레이
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator Loading()
    {
        Cursor.lockState = CursorLockMode.None;
        isLoading = true;

        // 화면 페이드
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);
        float timer = 1;
        while(timer > 0)
        {
            timer -= Time.deltaTime * 1.25f;
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, timer);
            yield return null;
        }
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);

        // 팁 실행
        if(tipCoroutine != null)
            StopCoroutine(tipCoroutine);

        tipCoroutine = StartCoroutine(Tip());

        // 로딩
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene);
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            if (progressbar.value < 0.9f)
            {
                progressbar.value = Mathf.MoveTowards(progressbar.value, 0.9f, Time.deltaTime);
            }
            else if (operation.progress >= 0.9f)
            {
                progressbar.value = Mathf.MoveTowards(progressbar.value, 1f, Time.deltaTime);
            }

            if (progressbar.value >= 1f)
            {
                loadText.text = "Press SpaceBar to Start.";
            }

            if (Input.GetKeyDown(KeyCode.Space) && progressbar.value >= 1f && operation.progress >= 0.9f)
            {
                // Scene Move
                isLoading = false;
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
