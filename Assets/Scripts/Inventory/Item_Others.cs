using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Others : Item_Base
{
    public override void Use()
    {

    }

    public override void Status_Setting()
    {
        item_Name = status.Item_Name;

        canStack = status.Stackable;
        item_Number = status.Item_Number;

        max_StackCount = status.MaxStack;

        item_Description = status.Item_Description;
    }
}
