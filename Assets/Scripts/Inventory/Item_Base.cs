using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public abstract class Item_Base : MonoBehaviour
{
    public enum ItemType { Equipment, Usable, Others }
    public ItemType itemType;

    public enum UsableType { Hp_Potion, AtkPoint_Potion, Buff_Potion }
    public UsableType usableType;

    public enum EquipmentType { Weapon, Head, Top, Bottom, Shoes, Accessory }
    public EquipmentType equipmentType;

    [Header("---Item Data---")]
    public string item_Name;
    public int item_Number; // 0~100 ����� 101~200 �Ҹ�ǰ 201~300 ��ȭ
    public Sprite item_Image;

    [Header("---Item Status---")]
    public Item_Data_SO status;
    public int hp;
    public float physicalDefense;
    public float magicalDefense;

    public int physicalDamage;
    public int magcialDamage;

    public int criticalChance;
    // int���� float�� ������
    public float criticalMultiplier;

    public int usable_Value;

    public string item_Description;

    [Header("---Stack check---")]
    public int max_StackCount;
    public bool canStack;


    public abstract void Status_Setting(); // ������ �������ͽ� ����

    public abstract void Use();  // ������ ���

    private void Awake()
    {
        Status_Setting();
    }
}
