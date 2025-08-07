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

            // �ݶ��̴� ��Ȱ��ȭ
            checkCollider.enabled = false;

            // ������ ��Ȱ��ȭ
            if (iconCoroutine != null)
                StopCoroutine(iconCoroutine);

            iconCoroutine = StartCoroutine(IconOff());

            // ���̾�α� ȣ��
            if(haveDialog)
                Dialog_Manager.instance.Dialog(dialog_Index);

            // ��� ����
            Use();
        }
    }

    /// <summary>
    /// �÷��̾ ���� ���� ���� ȣ���ϸ� ������ �Լ�
    /// </summary>
    public abstract void Use();

    /// <summary>
    /// IsIcon �� ���� ������ Ȱ��ȭ + �÷��̾� �ٶ󺸱�
    /// </summary>
    /// <returns></returns>
    private IEnumerator IconOn()
    {
        // ������ Ȱ��ȭ
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
    /// �÷��̾ ���� ������ ������ ������ ��Ȱ��ȭ + �÷��̾� �ٶ󺸱�
    /// </summary>
    /// <returns></returns>
    private IEnumerator IconOff()
    {
        // ������ ��Ȱ��ȭ
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

            // ������ Ȱ��ȭ
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

            // ������ ��Ȱ��ȭ
            if (iconCoroutine != null)
                StopCoroutine(iconCoroutine);

            iconCoroutine = StartCoroutine(IconOff());
        }
    }
}
