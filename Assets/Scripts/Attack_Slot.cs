using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Slot : MonoBehaviour
{
    [Header("=== Slot Setting ===")]
    public SlotType slotType;
    public Attack_Base myAttack;
    public Attack_Slot targetSlot;
    public bool haveAttack;
    public int slotSpeed;
    public int curAttackCount;
    public enum SlotType { Player, Enemy }
    
    public int DamageCal(int count)
    {
        // ���⿡ ������ ���� ������ ���� ���� �ʿ���!
        // ������ �⺻���� �鰡�� ����!

        // �⺻ ������
        int minDamage = myAttack.damageValue[count].x;
        int maxDamage = myAttack.damageValue[count].y;

        // ���� ���
        

        // ���� ������ ����
        int damage = Random.Range(minDamage, maxDamage);
        return damage;
    }

    public void TargetSetting(Attack_Slot target)
    {
        targetSlot = target;
    }

    public void ResetSlot()
    {
        myAttack = null;
        curAttackCount = 0;
        haveAttack = false;
    }
}
