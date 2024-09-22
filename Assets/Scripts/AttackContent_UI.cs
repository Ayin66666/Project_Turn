using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AttackContent_UI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("=== Data ===")]
    [SerializeField] private Attack_Base myAttack;


    [Header("=== UI ===")]
    [SerializeField] private Image icon;
    [SerializeField] private Image borderImage;
    [SerializeField] private Image iconBorderImage;
    [SerializeField] private Text attackName_Text;
    [SerializeField] private Text attackCost_Text;
    [SerializeField] private Text attacDamage_Text;
    [SerializeField] private Text attackDescription_Text;


    private void Start()
    {
        if (myAttack != null)
        {
            UI_Setting();
        }
    }

    private void UI_Setting()
    {
        icon.sprite = myAttack.icon;
        attackName_Text.text = myAttack.attackName;
        attackDescription_Text.text = myAttack.attackDescription_Text;
        attackCost_Text.text = myAttack.attackCost.ToString();

        // 데미지 셋팅
        // 문자열을 누적할 StringBuilder 사용
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        // damageValue 배열의 값들을 반복하여 텍스트 형식으로 변환
        for (int i = 0; i < myAttack.damageValue.Length; i++)
        {
            sb.Append($"{myAttack.damageValue[i].x} ~ {myAttack.damageValue[i].y}");

            // 마지막 요소가 아니라면 줄바꿈 추가
            if (i < myAttack.damageValue.Length - 1)
            {
                sb.Append("\n");
            }
        }

        // 변환된 문자열을 Text 컴포넌트에 할당
        attacDamage_Text.text = sb.ToString();
    }


    #region 마우스 이벤트
    public void OnPointerEnter(PointerEventData eventData)
    {
        borderImage.color = Color.gray;
        iconBorderImage.color = Color.gray;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        borderImage.color = Color.white;
        iconBorderImage.color = Color.white;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Player_Manager.instnace.player_Turn.Slot_AttackSetting(myAttack);
        Debug.Log("Click");
    }
    #endregion
}

