using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack_Base : MonoBehaviour
{
    [Header("=== Base State ===")]
    public Type type;
    public enum Type { Attack, Buff }
}
