using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using static Item_Equipment;

public class Item_Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("---���� ��ũ��Ʈ---")]
    public Item_Base item_Base;

    [Header("---����---")]
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
                // ���� �ؽ�Ʈ ����
                stack_Text.gameObject.SetActive(false);
                break;
            case Item_Base.ItemType.Usable:
            case Item_Base.ItemType.Others:
                // ���� �ؽ�Ʈ �ѱ�
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
    /// �κ��丮���� ���Գ��� �ٲٴ� ���
    /// </summary>
    public void ChangeSlot_Inventory()
    {
        // �� ������ ������ ������ ���
        Item_Base now_Item = item_Base;
        // �ٲٴ� ĭ�� ������ ��������
        Set_Slot(Drag_Obj.instance.drag_Slot.item_Base);

        // �� ������ �� ����ִٸ�
        if (now_Item != null)
        {
            // �ٲٴ� ĭ�� �������� �� ������ ���������� ����
            Drag_Obj.instance.drag_Slot.Set_Slot(now_Item);
        }
        // �� ������ ����ִٸ�
        else
        {
            // �ٲٴ� ĭ�� ���� ����
            Drag_Obj.instance.drag_Slot.Remove_Slot();
        }
    }

    /// <summary>
    /// ��� �����ϴ� ���
    /// </summary>
    /// <param name="equipmentType">��� Ÿ��</param>
    public void ChangeSlot_Equipment(Item_Base.EquipmentType equipmentType)
    {
        int equipment_slot_number = 0;

        // ��� ������ ���� ��ȣ �ű��
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
                // �� �׼����� ĭ üũ
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

                // �� �׼����� ĭ�� ���ٸ�
                if (!equipment_slots[equipment_slot_number].isEmpty)
                {
                    Debug.Log("��ű� �� ��");
                    return;
                }
                break;
        }
        // �� ������ ������ ������ ���
        Item_Base now_Item = item_Base;
        Debug.Log("���");

        // �̹� ����ϰ� �ִ� �� �ִٸ�
        if (equipment_slots[equipment_slot_number].item_Base != null)
        {
            Debug.Log("���");
            // ����ϰ� �ִ� ������ ��������
            Set_Slot(equipment_slots[equipment_slot_number].item_Base);
            // �� ������ ������ ����
            equipment_slots[equipment_slot_number].Set_Slot(now_Item);

            Debug.Log("���� ����");
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

            // �������ͽ� ����
            Player_UI.instance.Set_Inventory_Status_Text();

            // ����
            Player_Sound.instance.SFXPlay_OneShot(Player_Sound.instance.item_Equip_Sound);
            return;
        }

        // �̹� ����ϰ� �ִ°� ����
        // �� ������ �� ����ִٸ�
        if (now_Item != null)
        {
            Debug.Log("���");
            // �� ������ ������ ����
            equipment_slots[equipment_slot_number].Set_Slot(now_Item);
            now_Item.Use();
            Inventory.instance.Equipment_Slots.Remove(this);
            Remove_Slot();
        }

        // �������ͽ� ����
        Player_UI.instance.Set_Inventory_Status_Text();

        // ����
        Player_Sound.instance.SFXPlay_OneShot(Player_Sound.instance.item_Equip_Sound);
    }

    #region ������ �Լ�
    /// <summary>
    /// �κ��丮���� ��Ŭ�� ���� ��
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isEmpty && eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("�׽�Ʈ");

            switch (item_Base.itemType)
            {
                // ������̶�� ����
                case Item_Base.ItemType.Equipment:
                    Debug.Log("��� �׽�Ʈ");
                    ChangeSlot_Equipment(item_Base.equipmentType);
                    Player_UI.instance.item_Description_UI.SetActive(false);
                    break;
                // ������̶�� ���
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
            // ���� ������ ������ ����
            Debug.Log("�׽�Ʈ1");
            Player_UI.instance.item_Description.myItem = item_Base;

            // ����â ����
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
            // ���� ������ ������ ����
            Debug.Log("�׽�Ʈ2");
            Player_UI.instance.item_Description.myItem = null;

            // ����â ����
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
        Debug.Log("����ȭ");
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
