using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Easing.Tweening;

public class Player_ItemUI : MonoBehaviour
{
    [Header("=== UI ===")]
    [SerializeField] private Image icon;
    [SerializeField] private Text nameText;
    [SerializeField] private Text countText;
    [SerializeField] private RectTransform movePos;
    private CanvasGroup canvasGroup;
    private RectTransform rectPos;


    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectPos = GetComponent<RectTransform>();
    }

    /// <summary>
    /// UI 이동 위치, 아이템 데이터, 아이템 갯수 데이터 넣어야 함
    /// </summary>
    /// <param name="movePos">UI 이동위치</param>
    /// <param name="item">아이템 데이터</param>
    /// <param name="count">아이템 갯수</param>
    public void UISetting(Item_Base item, RectTransform movePos, int count)
    {
        // UI 셋팅
        this.movePos = movePos;
        icon.sprite = item.item_Image;
        nameText.text = item.item_Name;
        countText.text = count.ToString();

        StartCoroutine(Movement());
    }

    private IEnumerator Movement()
    {
        // UI 이동
        canvasGroup.alpha = 1;
        Vector2 startPos = rectPos.position;
        Vector2 endPos = movePos.position;
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * 1f;
            rectPos.position = Vector2.Lerp(startPos, endPos, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        rectPos.position = endPos;

        // 일시 대기
        yield return new WaitForSeconds(0.25f);

        // 페이드 효과
        timer = 1;
        while(timer > 0)
        {
            timer -= Time.deltaTime * 0.75f;
            canvasGroup.alpha = timer;
            yield return null;
        }
        canvasGroup.alpha = 0;

        // 오브젝트 파괴
        Destroy(gameObject);
    }
}
