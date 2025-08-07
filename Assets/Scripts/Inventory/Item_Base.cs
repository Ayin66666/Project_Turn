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
    public int item_Number; // 0~100 장비템 101~200 소모품 201~300 잡화
    public Sprite item_Image;

    [Header("---Item Status---")]
    public Item_Data_SO status;
    public int hp;
    public float physicalDefense;
    public float magicalDefense;

    public int physicalDamage;
    public int magcialDamage;

    public int criticalChance;
    // int에서 float로 수정함
    public float criticalMultiplier;

    public int usable_Value;

    public string item_Description;

    [Header("---Stack check---")]
    public int max_StackCount;
    public bool canStack;


    public abstract void Status_Setting(); // 아이템 스테이터스 설정

    public abstract void Use();  // 아이템 사용

    private void Awake()
    {
        Status_Setting();
    }
}
