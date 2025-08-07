using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Item_Base;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    [Header("---Inventory Slots---")]
    public List<Item_Slot> All_Slots;
    public List<Item_Slot> Equipment_Slots;
    public List<Item_Slot> Usable_Slots;
    public List<Item_Slot> Others_Slots;

    public enum SlotType { All, Equipment, Usable, Others }

    // 테스트용
    [Header("---Item Data---")]
    public List<Item_Base> items = new List<Item_Base>();

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        //테스트용
        
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetItem(items[0]);
            Debug.Log("포션 추가");
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetItem(items[1]);
            Debug.Log("검 추가");
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetItem(items[2]);
            Debug.Log("잡화 추가");
        }
        else if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetItem(items[3]);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha5))
        {
            SetItem(items[4]);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha6))
        {
            SetItem(items[5]);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha7))
        {
            SetItem(items[6]);
            Debug.Log("잡화 추가");
        }
        else if(Input.GetKeyDown(KeyCode.Alpha8))
        {
            SetItem(items[7]);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha9))
        {
            SetItem(items[8]);
        }
        else if(Input.GetKeyDown(KeyCode.Minus))
        {
            SetItem(items[9]);
        }
        else if (Input.GetKeyDown(KeyCode.Plus))
        {
            SetItem(items[10]);
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            SetItem(items[11]);
        }
        
    }

    /// <summary>
    /// 아이템을 인벤토리에 넣는 기능
    /// </summary>
    /// <param name="itemData">아이템 데이터</param> 
    public void SetItem(Item_Base itemData)
    {
        Debug.Log("아이템 추가");
        //아이템 스택 가능하다면
        if(itemData.canStack)
        {
            Debug.Log("스택!");
            for (int i = 0; i < All_Slots.Count; i++)
            {
                //슬롯이 안 비어있다면
                if (!All_Slots[i].isEmpty)
                {
                    //슬롯의 아이템 종류 체크
                    if (All_Slots[i].item_Base.item_Number == itemData.item_Number)
                    {
                        //아이템 스택수가 최대가 아니라면
                        if (All_Slots[i].item_Stack < All_Slots[i].item_Base.max_StackCount)
                        {
                            Debug.Log("아이템 스택");
                            All_Slots[i].item_Stack++;
                            All_Slots[i].stack_Text.text = All_Slots[i].item_Stack.ToString();
                            // 현재 스택수 확인용
                            Debug.Log(All_Slots[i].item_Stack);
                            Debug.Log(All_Slots[i]);
                            return;
                        }
                    }
                }
            }

            for (int i = 0; i < All_Slots.Count; i++)
            {
                // 슬롯이 비어있다면
                if (All_Slots[i].isEmpty)
                {
                    Debug.Log("새 칸에 아이템 추가");
                    // 데이터 입력
                    All_Slots[i].Set_Slot(itemData);
                    Debug.Log(itemData.itemType);
                    switch (itemData.itemType)
                    {
                        case Item_Base.ItemType.Equipment:
                            break;
                        case Item_Base.ItemType.Usable:
                            Usable_Slots.Add(All_Slots[i]);
                            break;
                        case Item_Base.ItemType.Others:
                            Others_Slots.Add(All_Slots[i]);
                            break;
                    }
                    All_Slots[i].item_Stack = 0;
                    All_Slots[i].item_Stack++;
                    All_Slots[i].stack_Text.text = All_Slots[i].item_Stack.ToString();
                    // 현재 스택수 확인용
                    Debug.Log(All_Slots[i].item_Stack);
                    return;
                }
            }
            Debug.Log("가득 참!");
        }
        //아이템 스택 불가하다면
        else
        {
            // 빈 슬롯 체크
            for (int i = 0; i < All_Slots.Count; i++)
            {
                // 슬롯이 비어있다면
                if (All_Slots[i].isEmpty)
                {
                    Debug.Log("아이템 입력됨");
                    // 데이터 입력
                    All_Slots[i].Set_Slot(itemData);
                    switch (itemData.itemType)
                    {
                        case Item_Base.ItemType.Equipment:
                            Equipment_Slots.Add(All_Slots[i]);
                            break;
                        case Item_Base.ItemType.Usable:
                            break;
                        case Item_Base.ItemType.Others:
                            break;
                    }
                    All_Slots[i].stack_Text.gameObject.SetActive(false);
                    return;
                }
            }
            Debug.Log("가득 참!");
        }
    }

    /// <summary>
    /// 인벤토리 타입별 슬롯 켜고 끄기 (버튼 함수)
    /// </summary>
    /// <param name="index">0 = 전체, 1 = 장비템, 2 = 사용템, 3 = 잡화</param>
    public void OnOff_Slot(int index)
    {
        SlotType slotType = (SlotType)index;

        switch (slotType)
        {
            case SlotType.All:
                for (int i = 0; i < All_Slots.Count; i++)
                {
                    All_Slots[i].canvasGroup.alpha = 1;
                    All_Slots[i].canvasGroup.blocksRaycasts = true;
                }
                break;
            case SlotType.Equipment:
                for (int i = 0; i < All_Slots.Count; i++)
                {
                    All_Slots[i].canvasGroup.alpha = 0.5f;
                    All_Slots[i].canvasGroup.blocksRaycasts = false;
                }

                for (int i = 0; i < Equipment_Slots.Count; i++)
                {
                    Equipment_Slots[i].canvasGroup.alpha = 1;
                    Equipment_Slots[i].canvasGroup.blocksRaycasts = true;
                }
                break;
            case SlotType.Usable:
                for (int i = 0; i < All_Slots.Count; i++)
                {
                    All_Slots[i].canvasGroup.alpha = 0.5f;
                    All_Slots[i].canvasGroup.blocksRaycasts = false;
                }

                for (int i = 0; i < Usable_Slots.Count; i++)
                {
                    Usable_Slots[i].canvasGroup.alpha = 1;
                    Usable_Slots[i].canvasGroup.blocksRaycasts = true;
                }
                break;
            case SlotType.Others:
                for (int i = 0; i < All_Slots.Count; i++)
                {
                    All_Slots[i].canvasGroup.alpha = 0.5f;
                    All_Slots[i].canvasGroup.blocksRaycasts = false;
                }

                for (int i = 0; i < Others_Slots.Count; i++)
                {
                    Others_Slots[i].canvasGroup.alpha = 1;
                    Others_Slots[i].canvasGroup.blocksRaycasts = true;
                }
                break;
        }
    }
}
