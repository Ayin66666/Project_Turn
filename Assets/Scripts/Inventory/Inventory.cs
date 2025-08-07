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

    // �׽�Ʈ��
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
        //�׽�Ʈ��
        
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetItem(items[0]);
            Debug.Log("���� �߰�");
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetItem(items[1]);
            Debug.Log("�� �߰�");
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetItem(items[2]);
            Debug.Log("��ȭ �߰�");
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
            Debug.Log("��ȭ �߰�");
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
    /// �������� �κ��丮�� �ִ� ���
    /// </summary>
    /// <param name="itemData">������ ������</param> 
    public void SetItem(Item_Base itemData)
    {
        Debug.Log("������ �߰�");
        //������ ���� �����ϴٸ�
        if(itemData.canStack)
        {
            Debug.Log("����!");
            for (int i = 0; i < All_Slots.Count; i++)
            {
                //������ �� ����ִٸ�
                if (!All_Slots[i].isEmpty)
                {
                    //������ ������ ���� üũ
                    if (All_Slots[i].item_Base.item_Number == itemData.item_Number)
                    {
                        //������ ���ü��� �ִ밡 �ƴ϶��
                        if (All_Slots[i].item_Stack < All_Slots[i].item_Base.max_StackCount)
                        {
                            Debug.Log("������ ����");
                            All_Slots[i].item_Stack++;
                            All_Slots[i].stack_Text.text = All_Slots[i].item_Stack.ToString();
                            // ���� ���ü� Ȯ�ο�
                            Debug.Log(All_Slots[i].item_Stack);
                            Debug.Log(All_Slots[i]);
                            return;
                        }
                    }
                }
            }

            for (int i = 0; i < All_Slots.Count; i++)
            {
                // ������ ����ִٸ�
                if (All_Slots[i].isEmpty)
                {
                    Debug.Log("�� ĭ�� ������ �߰�");
                    // ������ �Է�
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
                    // ���� ���ü� Ȯ�ο�
                    Debug.Log(All_Slots[i].item_Stack);
                    return;
                }
            }
            Debug.Log("���� ��!");
        }
        //������ ���� �Ұ��ϴٸ�
        else
        {
            // �� ���� üũ
            for (int i = 0; i < All_Slots.Count; i++)
            {
                // ������ ����ִٸ�
                if (All_Slots[i].isEmpty)
                {
                    Debug.Log("������ �Էµ�");
                    // ������ �Է�
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
            Debug.Log("���� ��!");
        }
    }

    /// <summary>
    /// �κ��丮 Ÿ�Ժ� ���� �Ѱ� ���� (��ư �Լ�)
    /// </summary>
    /// <param name="index">0 = ��ü, 1 = �����, 2 = �����, 3 = ��ȭ</param>
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
