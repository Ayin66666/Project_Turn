using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Equipment_Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("---참조 스크립트---")]
    public Item_Base item_Base;


    [Header("---변수---")]
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
    /// 장비템 벗는 기능
    /// </summary>
    public void Remove_Slot()
    {
        // 착용된게 없다면
        if (item_Base == null)
        {
            return;
        }

        // 장비템 스탯만큼 다시 빼기
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
            // 인벤토리 슬롯이 비어있다면
            if (inventory_slots[i].isEmpty)
            {
                Debug.Log("새 칸에 아이템 추가");
                // 데이터 입력
                inventory_slots[i].Set_Slot(item_Base);
                inventory_slots[i].stack_Text.gameObject.SetActive(false);
                Inventory.instance.Equipment_Slots.Add(inventory_slots[i]);

                isEmpty = true;
                slot_Image.raycastTarget = true;
                item_Base = null;
                icon.sprite = backGround_Image;

                // 스테이터스 세팅
                Player_UI.instance.Set_Inventory_Status_Text();

                // 사운드
                Player_Sound.instance.SFXPlay_OneShot(Player_Sound.instance.item_Equip_Sound);
                return;
            }
            else
            {
                Debug.Log("가득 참");
            }
        }
    }

    #region 포인터 함수
    // 장비칸을 우클릭 했을때
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isEmpty && eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("테스트");

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
    #endregion
}
