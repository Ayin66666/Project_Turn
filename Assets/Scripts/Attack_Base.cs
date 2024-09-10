using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack_Base : MonoBehaviour
{
    [Header("=== Base State ===")]
    public Type type;
    public int attackCount;
    public Vector2Int[] damageValue;

    public enum Type { Attack, Buff }


    public abstract void AttackSetting();

}
