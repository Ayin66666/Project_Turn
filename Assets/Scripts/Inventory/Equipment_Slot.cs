using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Equipment_Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("---���� ��ũ��Ʈ---")]
    public Item_Base item_Base;


    [Header("---����---")]
    public bool isEmpty = true;


    [Header("---Inventory Slots---")]
    public List<Item_Slot> inventory_slots;


    [Header("---Components---")]
    public Image icon;
    [SerializeField] private Image slot_Image;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Sprite backGround_Image;

    private void Awake()
    {
        slot_Image = GetComponent<Image>();
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        for (int i = 0; i < Inventory.instance.All_Slots.Count; i++)
        {
            inventory_slots.Add(Inventory.instance.All_Slots[i]);
        }

        icon.sprite = backGround_Image;
    }

    private void Update()
    {

    }

    public void Set_Slot(Item_Base set_Item)
    {
        isEmpty = false;
        slot_Image.raycastTarget = false;
        item_Base = set_Item;
        icon.sprite = item_Base.item_Image;
    }

    /// <summary>
    /// ����� ���� ���
    /// </summary>
    public void Remove_Slot()
    {
        // ����Ȱ� ���ٸ�
        if (item_Base == null)
        {
            return;
        }

        // ����� ���ȸ�ŭ �ٽ� ����
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

        for (int i = 0; i < inventory_slots.Count; i++)
        {
            // �κ��丮 ������ ����ִٸ�
            if (inventory_slots[i].isEmpty)
            {
                Debug.Log("�� ĭ�� ������ �߰�");
                // ������ �Է�
                inventory_slots[i].Set_Slot(item_Base);
                inventory_slots[i].stack_Text.gameObject.SetActive(false);
                Inventory.instance.Equipment_Slots.Add(inventory_slots[i]);

                isEmpty = true;
                slot_Image.raycastTarget = true;
                item_Base = null;
                icon.sprite = backGround_Image;

                // �������ͽ� ����
                Player_UI.instance.Set_Inventory_Status_Text();

                // ����
                Player_Sound.instance.SFXPlay_OneShot(Player_Sound.instance.item_Equip_Sound);
                return;
            }
            else
            {
                Debug.Log("���� ��");
            }
        }
    }

    #region ������ �Լ�
    // ���ĭ�� ��Ŭ�� ������
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isEmpty && eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("�׽�Ʈ");

            switch (item_Base.itemType)
            {
                case Item_Base.ItemType.Equipment:
                    Remove_Slot();
                    Player_UI.instance.item_Description_UI.SetActive(false);
                    break;
                case Item_Base.ItemType.Usable:
                    break;
                case Item_Base.ItemType.Others:
                    break;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isEmpty)
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
    #endregion
}
