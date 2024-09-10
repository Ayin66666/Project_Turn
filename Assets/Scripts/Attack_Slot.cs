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
        // 여기에 버프에 따른 데미지 증감 구현 필요함!
        // 지금은 기본값만 들가게 만듬!

        // 기본 데미지
        int minDamage = myAttack.damageValue[count].x;
        int maxDamage = myAttack.damageValue[count].y;

        // 버프 계산
        

        // 최종 데미지 결정
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
