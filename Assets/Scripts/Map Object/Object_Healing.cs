using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Healing : Object_Base
{
    [Header("===Healing Status===")]
    [SerializeField] private int healingValue;
    [SerializeField] private GameObject healingVFX;
    [SerializeField] private GameObject healingAuraVFX;


    public override void Use()
    {
        // �ƿ�� ����
        healingAuraVFX.SetActive(false);

        // �÷��̾� ȸ��
        Player_Manager.instnace.curHp += healingValue;
        Instantiate(healingVFX, Player_Manager.instnace.player_World.transform.position, Quaternion.identity);
    }
}
