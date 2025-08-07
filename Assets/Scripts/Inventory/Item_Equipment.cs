using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Equipment : Item_Base
{
    public override void Use()
    {
        switch (equipmentType)
        {
            case EquipmentType.Weapon:
            case EquipmentType.Head:
            case EquipmentType.Top:
            case EquipmentType.Bottom:
            case EquipmentType.Shoes:
            case EquipmentType.Accessory:
                Player_Manager.instnace.Status_Set(Player_Manager.Status.MaxHp, hp);
                if (Player_Manager.instnace.curHp > Player_Manager.instnace.maxHp)
                {
                    Player_Manager.instnace.curHp = Player_Manager.instnace.maxHp;
                }
                //Player_Manager.instnace.Status_Set(Player_Manager.Status.CurHp, hp);
                Player_Manager.instnace.Status_Set(Player_Manager.Status.PhysicalDamage, physicalDamage);
                Player_Manager.instnace.Status_Set(Player_Manager.Status.MagcialDamage, magcialDamage);
                Player_Manager.instnace.Status_Set(Player_Manager.Status.PhysicalDefense, physicalDefense);
                Player_Manager.instnace.Status_Set(Player_Manager.Status.MagicDefense, magicalDefense);
                Player_Manager.instnace.Status_Set(Player_Manager.Status.CriticalChance, criticalChance);
                Player_Manager.instnace.Status_Set(Player_Manager.Status.CriticalMultiplier, criticalMultiplier);
                break;
        }
    }

    public override void Status_Setting()
    {
        item_Name = status.Item_Name;

        hp = status.Hp;

        physicalDefense = status.PhysicalDefense;
        magicalDefense = status.MagicalDefense;

        physicalDamage = status.PhysicalDamage;
        magcialDamage = status.MagicalDamage;

        criticalChance = status.CriticalChance;
        criticalMultiplier = status.CriticalMultiplier;

        item_Number = status.Item_Number;

        item_Description = status.Item_Description;
    }
}
