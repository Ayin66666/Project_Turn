using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Item_Data", menuName = "Scriptalbe Ojbect / Item_Data", order = int.MaxValue)]

public class Item_Data_SO : ScriptableObject
{
    #region 아이템 베이스 스탯
    [Header("---Item Base Status---")]
    [TextArea]
    [SerializeField] private string item_Name;
    public string Item_Name { get { return item_Name; } }


    [SerializeField] private bool stackable;
    public bool Stackable { get { return stackable; } }


    [SerializeField] private int curStack;
    public int CurStack { get { return curStack; } }


    [SerializeField] private int maxStack;
    public int MaxStack { get { return maxStack; } }


    [SerializeField] private int item_Number;
    public int Item_Number { get {  return item_Number; } }

    [TextArea][SerializeField] private string item_Description;
    public string Item_Description { get { return item_Description; } }
    #endregion

    #region 장비템 스탯
    [Header("---Equipment Status---")]
    [SerializeField] private int hp;
    [SerializeField] private int physicalDamage;
    [SerializeField] private int magicalDamage;
    [SerializeField] private float physicalDefense;
    [SerializeField] private float magicalDefense;
    [SerializeField] private int criticalChance;
    [SerializeField] private float criticalMultiplier;

    public int Hp
    { 
        get { return hp; }
        private set { hp = value; }
    }
    public int PhysicalDamage
    { 
        get { return physicalDamage; }
        private set { physicalDamage = value; }
    }
    public int MagicalDamage
    { get { return magicalDamage; }
      private set { magicalDamage = value; }
    }
    public float PhysicalDefense
    {
        get { return physicalDefense; }
        private set { physicalDefense = value; }
    }
    public float MagicalDefense
    {
        get { return magicalDefense; }
        private set { magicalDefense = value;}
    }
    public int CriticalChance
    {
        get { return criticalChance; }
        private set { criticalChance = value; }
    }

    // int에서 float로 수정함
    public float CriticalMultiplier
    {
        get { return criticalMultiplier; }
        private set { criticalMultiplier = value; }
    }
    #endregion

    #region 사용템 스탯
    [Header("---Usable Status---")]
    [SerializeField] private int usable_value;
    public int Usable_Value { get { return usable_value; } }
    #endregion
}
