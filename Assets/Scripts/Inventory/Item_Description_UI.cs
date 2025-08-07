using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Description_UI : MonoBehaviour
{
    [Header("=== Data ===")]
    public Item_Base myItem;

    [Header("=== UI ===")]
    [SerializeField] private Image backGround_Image;
    [SerializeField] private Image item_Icon;
    [SerializeField] private Text item_Name_Text;
    [SerializeField] private Text item_Type_Text;
    [SerializeField] private Text item_Status_Text;
    [SerializeField] private Text item_ex_Text;
    [SerializeField] private List<string> item_Status_TextList;
    public RectTransform rect;
    [SerializeField] System.Text.StringBuilder sb = new System.Text.StringBuilder();

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        UI_Setting();
    }

    private void OnDisable()
    {
        // 내용물 비우기
        sb.Clear();
        item_Status_TextList.Clear();
    }

    /// <summary>
    /// 아이템 정보 받아오는 기능 -> 혹시 status 에서 접근해야 하는데 안한 부분 있는지 체크할 것!
    /// </summary>
    public void UI_Setting()
    {
        item_Icon.sprite = myItem.item_Image;
        item_Name_Text.text = myItem.status.Item_Name;
        item_Type_Text.text = myItem.itemType.ToString();

        switch (myItem.itemType)
        {
            case Item_Base.ItemType.Equipment:
                item_Status_TextList.Add(myItem.status.Hp.ToString());
                item_Status_TextList.Add(myItem.status.PhysicalDamage.ToString());
                item_Status_TextList.Add(myItem.status.MagicalDamage.ToString());
                item_Status_TextList.Add(myItem.status.PhysicalDefense.ToString());
                item_Status_TextList.Add(myItem.status.MagicalDefense.ToString());
                item_Status_TextList.Add(myItem.status.CriticalChance.ToString());
                item_Status_TextList.Add(myItem.status.CriticalMultiplier.ToString());

                for (int i = 0; i < item_Status_TextList.Count; i++)
                {
                    if (i == 0)
                    {
                        sb.Append("체력 : " + item_Status_TextList[i]);
                    }
                    else if (i == 1)
                    {
                        sb.Append($"물리/마법 공격력 : {item_Status_TextList[i]} / {item_Status_TextList[i + 1]}");
                    }
                    else if (i == 3)
                    {
                        sb.Append($"물리/마법 방어력 : {item_Status_TextList[i]} / {item_Status_TextList[i + 1]}");
                    }
                    else if (i == 5)
                    {
                        sb.Append("치명타 확률 : " + item_Status_TextList[i]);
                    }
                    else if (i == 6)
                    {
                        sb.Append("치명타 배율 : " + item_Status_TextList[i]);
                    }

                    // 마지막 요소가 아니라면 줄바꿈 추가
                    if (i < item_Status_TextList.Count - 1 && i != 2 && i != 4)
                    {
                        sb.Append("\n");
                    }
                }

                // 아이템 설명
                item_ex_Text.text = $"{myItem.status.Item_Description}";

                break;
            case Item_Base.ItemType.Usable:
                item_Status_TextList.Add(myItem.status.Usable_Value.ToString());
                item_ex_Text.text = $"{myItem.status.Item_Description}";
                sb.Append("체력 " + item_Status_TextList[0] + "만큼 회복");
                break;
            case Item_Base.ItemType.Others:
                item_Status_TextList.Add(myItem.status.Item_Description);

                sb.Append(item_Status_TextList[0]);
                break;
        }


        item_Status_Text.text = sb.ToString();
    }
}
