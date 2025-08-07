using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Checkpoint : Object_Base
{
    [Header("=== CheckPoint Status ===")]
    [SerializeField] private GameObject saveVFX;


    public override void Use()
    {
        CheckPoint_Manager.instance.Player_DataUpdate();
        CheckPoint_Manager.instance.Stage_DataUpdate();
        saveVFX.SetActive(true);
    }
}
