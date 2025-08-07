using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using static Item_Equipment;

public class Item_Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("---참조 스크립트---")]
    public Item_Base item_Base;

    [Header("---변수---")]
    public bool isEmpty = true;
    public int item_Stack;

    [Header("---Equipment Slots---")]
    public List<Equipment_Slot> equipment_slots;

    [Header("---Components---")]
    public Image icon;
    public Text stack_Text;
    [SerializeField] private Image slot_Image;
    public CanvasGroup canvasGroup;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Sprite backGround_Image;


    private void Awake()
    {
        // icon.sprite = backGround_Image;
        stack_Text.gameObject.SetActive(false);

        if (item_Base != null && item_Base.canStack)
        {
            stack_Text.gameObject.SetActive(true);
        }
    }

    public void Set_Slot(Item_Base set_Item)
    {
        isEmpty = false;
        //slot_Image.raycastTarget = false;
        item_Base = set_Item;
        icon.sprite = item_Base.item_Image;

        switch (item_Base.itemType)
        {
            case Item_Base.ItemType.Equipment:
                // 스택 텍스트 끄기
                stack_Text.gameObject.SetActive(false);
                break;
            case Item_Base.ItemType.Usable:
            case Item_Base.ItemType.Others:
                // 스택 텍스트 켜기
                stack_Text.gameObject.SetActive(true);
                break;
        }
    }

    public void Remove_Slot()
    {
        if (item_Base == null)
        {
            return;
        }

        isEmpty = true;
        //slot_Image.raycastTarget = true;
        item_Base = null;
        icon.sprite = backGround_Image;
        stack_Text.gameObject.SetActive(false);
    }

    /// <summary>
    /// 인벤토리에서 슬롯끼리 바꾸는 기능
    /// </summary>
    public void ChangeSlot_Inventory()
    {
        // 이 슬롯의 아이템 데이터 담기
        Item_Base now_Item = item_Base;
        // 바꾸는 칸의 아이템 가져오기
        Set_Slot(Drag_Obj.instance.drag_Slot.item_Base);

        // 이 슬롯이 안 비어있다면
        if (now_Item != null)
        {
            // 바꾸는 칸의 아이템을 이 슬롯의 아이템으로 설정
            Drag_Obj.instance.drag_Slot.Set_Slot(now_Item);
        }
        // 이 슬롯이 비어있다면
        else
        {
            // 바꾸는 칸의 슬롯 비우기
            Drag_Obj.instance.drag_Slot.Remove_Slot();
        }
    }

    /// <summary>
    /// 장비 착용하는 기능
    /// </summary>
    /// <param name="equipmentType">장비 타입</param>
    public void ChangeSlot_Equipment(Item_Base.EquipmentType equipmentType)
    {
        int equipment_slot_number = 0;

        // 장비 종류별 슬롯 번호 매기기
        switch (equipmentType)
        {
            case Item_Base.EquipmentType.Weapon:
                equipment_slot_number = 0;
                break;
            case Item_Base.EquipmentType.Head:
                equipment_slot_number = 1;
                break;
            case Item_Base.EquipmentType.Top:
                equipment_slot_number = 2;
                break;
            case Item_Base.EquipmentType.Bottom:
                equipment_slot_number = 3;
                break;
            case Item_Base.EquipmentType.Shoes:
                equipment_slot_number = 4;
                break;
            case Item_Base.EquipmentType.Accessory:
                // 빈 액세서리 칸 체크
                for (int i = 5; i < 8; i++)
                {
                    if(equipment_slots[i].isEmpty)
                    {
                        equipment_slot_number = i;
                        Debug.Log(equipment_slot_number);
                        break;
                    }

                    equipment_slot_number = i;
                    Debug.Log(equipment_slot_number);
                }

                // 빈 액세서리 칸이 없다면
                if (!equipment_slots[equipment_slot_number].isEmpty)
                {
                    Debug.Log("장신구 다 참");
                    return;
                }
                break;
        }
        // 이 슬롯의 아이템 데이터 담기
        Item_Base now_Item = item_Base;
        Debug.Log("장비");

        // 이미 장비하고 있는 게 있다면
        if (equipment_slots[equipment_slot_number].item_Base != null)
        {
            Debug.Log("장비");
            // 장비하고 있던 아이템 가져오기
            Set_Slot(equipment_slots[equipment_slot_number].item_Base);
            // 이 슬롯의 아이템 장착
            equipment_slots[equipment_slot_number].Set_Slot(now_Item);

            Debug.Log("스탯 감소");
            //Player_Manager.instnace.Status_Set(Player_Manager.Status.CurHp, -item_Base.hp);
            Player_Manager.instnace.Status_Set(Player_Manager.Status.MaxHp, -item_Base.hp);
            if (Player_Manager.instnace.curHp > Player_Manager.instnace.maxHp)
            {
                Player_Manager.instnace.curHp = Player_Manager.instnace.maxHp;
            }
            Player_Manager.instnace.Status_Set(Player_Manager.Status.PhysicalDamage, -item_Base.physicalDamage);
            Player_Manager.instnace.Status_Set(Player_Manager.Status.MagcialDamage, -item_Base.magcialDamage);
            Player_Manager.instnace.Status_Set(Player_Manager.Status.PhysicalDefense, -item_Base.physicalDefense);
            Player_Manager.instnace.Status_Set(Player_Manager.Status.MagicDefense, -item_Base.magicalDefense);
            Player_Manager.instnace.Status_Set(Player_Manager.Status.CriticalChance, -item_Base.criticalChance);
            Player_Manager.instnace.Status_Set(Player_Manager.Status.CriticalMultiplier, -item_Base.criticalMultiplier);

            now_Item.Use();

            // 스테이터스 세팅
            Player_UI.instance.Set_Inventory_Status_Text();

            // 사운드
            Player_Sound.instance.SFXPlay_OneShot(Player_Sound.instance.item_Equip_Sound);
            return;
        }

        // 이미 장비하고 있는게 없고
        // 이 슬롯이 안 비어있다면
        if (now_Item != null)
        {
            Debug.Log("장비");
            // 이 슬롯의 아이템 장착
            equipment_slots[equipment_slot_number].Set_Slot(now_Item);
            now_Item.Use();
            Inventory.instance.Equipment_Slots.Remove(this);
            Remove_Slot();
        }

        // 스테이터스 세팅
        Player_UI.instance.Set_Inventory_Status_Text();

        // 사운드
        Player_Sound.instance.SFXPlay_OneShot(Player_Sound.instance.item_Equip_Sound);
    }

    #region 포인터 함수
    /// <summary>
    /// 인벤토리에서 우클릭 했을 때
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isEmpty && eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("테스트");

            switch (item_Base.itemType)
            {
                // 장비템이라면 착용
                case Item_Base.ItemType.Equipment:
                    Debug.Log("장비 테스트");
                    ChangeSlot_Equipment(item_Base.equipmentType);
                    Player_UI.instance.item_Description_UI.SetActive(false);
                    break;
                // 사용템이라면 사용
                case Item_Base.ItemType.Usable:
                    item_Base.Use();

                    if (item_Base.canStack)
                    {
                        item_Stack--;
                        stack_Text.text = item_Stack.ToString();
                        if (item_Stack <= 0)
                        {
                            Remove_Slot();
                            Player_UI.instance.item_Description_UI.SetActive(false);
                        }
                    }
                    else
                    {
                        Remove_Slot();
                        Player_UI.instance.item_Description_UI.SetActive(false);
                    }
                    break;
                case Item_Base.ItemType.Others:
                    break;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!isEmpty)
        {
            // 현재 아이템 데이터 설정
            Debug.Log("테스트1");
            Player_UI.instance.item_Description.myItem = item_Base;

            // 설명창 띄우기
            Player_UI.instance.item_Description_UI.SetActive(true);
            Player_UI.instance.item_Description.rect.position = transform.position;

            icon.color = Color.white;
        }
        else
        {
            icon.color = Color.yellow;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isEmpty)
        {
            // 현재 아이템 데이터 제거
            Debug.Log("테스트2");
            Player_UI.instance.item_Description.myItem = null;

            // 설명창 끄기
            Player_UI.instance.item_Description_UI.SetActive(false);

            icon.color = Color.white;
        }
        else
        {
            icon.color = Color.white;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isEmpty && eventData.button == PointerEventData.InputButton.Left)
        {
            Drag_Obj.instance.drag_Slot = this;
            Drag_Obj.instance.Set_Drag_Image(this.icon);
            Drag_Obj.instance.transform.position = eventData.position;

            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
            scrollRect.vertical = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Drag_Obj.instance.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        scrollRect.vertical = true;

        Drag_Obj.instance.Set_Color(0);
        Drag_Obj.instance.drag_Slot = null;
        Debug.Log("투명화");
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (Drag_Obj.instance.drag_Slot != null)
        {
            ChangeSlot_Inventory();
        }

        icon.color = Color.white;
    }
    #endregion
}
