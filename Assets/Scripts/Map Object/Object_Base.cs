using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Object_Base : MonoBehaviour
{
    [Header("=== Base Setting ===")]
    [SerializeField] protected ObjectType objectType;
    [SerializeField] private int dialog_Index;
    [SerializeField] private bool haveDialog;
    [SerializeField] private Collider checkCollider;
    protected bool isUsed;
    protected bool isIcon;
    protected bool isPlayerIn;
    private Coroutine iconCoroutine;
    protected enum ObjectType { Door, Healing, CheckPoint }

    // Icon
    private GameObject cam;
    private Vector3 lookDir;


    [Header("=== Canvas ===")]
    [SerializeField] private GameObject iconSet;
    [SerializeField] private Canvas canvas;
    [SerializeField] private CanvasGroup IconCanvasGroup;


    private void OnEnable()
    {
        cam = GameObject.Find("Main Camera");
        canvas.worldCamera = cam.GetComponent<Camera>();
    }

    private void Update()
    {
        if (isPlayerIn && !isUsed && Input.GetKeyDown(KeyCode.F))
        {
            isUsed = true;
            isIcon = false;

            // 콜라이더 비활성화
            checkCollider.enabled = false;

            // 아이콘 비활성화
            if (iconCoroutine != null)
                StopCoroutine(iconCoroutine);

            iconCoroutine = StartCoroutine(IconOff());

            // 다이얼로그 호출
            if(haveDialog)
                Dialog_Manager.instance.Dialog(dialog_Index);

            // 기능 동작
            Use();
        }
    }

    /// <summary>
    /// 플레이어가 범위 내로 들어가서 호출하면 동작할 함수
    /// </summary>
    public abstract void Use();

    /// <summary>
    /// IsIcon 인 동안 아이콘 활성화 + 플레이어 바라보기
    /// </summary>
    /// <returns></returns>
    private IEnumerator IconOn()
    {
        // 아이콘 활성화
        IconCanvasGroup.alpha = 1;
        while(isIcon)
        {
            lookDir = transform.position - cam.transform.position;
            lookDir.y = 0;

            // Lookat
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            iconSet.transform.rotation = targetRotation;

            yield return null;
        }
    }

    /// <summary>
    /// 플레이어가 범위 밖으로 나가면 아이콘 비활성화 + 플레이어 바라보기
    /// </summary>
    /// <returns></returns>
    private IEnumerator IconOff()
    {
        // 아이콘 비활성화
        float start = IconCanvasGroup.alpha;
        float end = 0;
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            IconCanvasGroup.alpha = Mathf.Lerp(start, end, timer);

            // Lookat
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            iconSet.transform.rotation = targetRotation;

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isUsed)
        {
            return;
        }

        if (other.CompareTag("Player") && !isIcon)
        {
            isIcon = true;
            isPlayerIn = true;

            // 아이콘 활성화
            if (iconCoroutine != null)
                StopCoroutine(iconCoroutine);

            iconCoroutine = StartCoroutine(IconOn());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isUsed)
        {
            return;
        }

        if (other.CompareTag("Player") && isIcon)
        {
            isIcon = false;
            isPlayerIn = false;

            // 아이콘 비활성화
            if (iconCoroutine != null)
                StopCoroutine(iconCoroutine);

            iconCoroutine = StartCoroutine(IconOff());
        }
    }
}
