using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack_Base : MonoBehaviour
{
    [Header("=== Attack Base State ===")]
    public Type type;
    public string attackName;
    [TextArea] public string attackDescription_Text;
    public int attackCost;
    public int attackCount;
    public Vector2Int[] damageValue;
    public Sprite icon;

    public enum Type { Attack, Buff }


    public abstract void AttackSetting();

}
