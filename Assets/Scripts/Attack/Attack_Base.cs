using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack_Base : MonoBehaviour
{
    [Header("=== Attack Base State ===")]
    public Type type; // 공격 타입
    public string attackName; // 공격 이름
    [TextArea] public string attackDescription_Text; // 공격 설명
    public int attackCost; // 자원 소모량
    public int attackCount; // 공격 횟수
    public Vector2[] damageValue; // 각 합마다 데미지 벨류
    public DamageType[] damageType; // 각 공격마다 물리&마법 데미지 체크
    public Sprite icon;

    public enum AttackOwner { Player, Enemy }
    public enum DamageType { physical, magicl }
    public enum Type { Attack, Defense, Buff, Complex } // 나중에 스킬을 태그 별로 나눈다면?


    public abstract void UseAttack(AttackOwner attackOwner, GameObject ownerObj, GameObject targetObj);
}
