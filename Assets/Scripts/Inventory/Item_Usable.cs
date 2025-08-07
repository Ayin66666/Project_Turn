using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Usable : Item_Base
{
    public override void Use()
    {
        switch (usableType)
        {
            case UsableType.Hp_Potion:
                Player_Manager.instnace.Status_Set(Player_Manager.Status.CurHp, usable_Value);
                break;
            case UsableType.AtkPoint_Potion:
                Player_Manager.instnace.Status_Set(Player_Manager.Status.AttackPoint, usable_Value);
                break;
            case UsableType.Buff_Potion:

                break;
        }

        Player_UI.instance.Set_Inventory_Status_Text();
    }

    public override void Status_Setting()
    {
        item_Name = status.Item_Name;
        canStack = status.Stackable;
        max_StackCount = status.MaxStack;
        usable_Value = status.Usable_Value;
        item_Number = status.Item_Number;
    }
}
