using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack_Base : MonoBehaviour
{
    [Header("=== Attack Base State ===")]
    public Type type; // ���� Ÿ��
    public string attackName; // ���� �̸�
    [TextArea] public string attackDescription_Text; // ���� ����
    public int attackCost; // �ڿ� �Ҹ�
    public int attackCount; // ���� Ƚ��
    public Vector2[] damageValue; // �� �ո��� ������ ����
    public DamageType[] damageType; // �� ���ݸ��� ����&���� ������ üũ
    public Sprite icon;

    public enum AttackOwner { Player, Enemy }
    public enum DamageType { physical, magicl }
    public enum Type { Attack, Defense, Buff, Complex } // ���߿� ��ų�� �±� ���� �����ٸ�?


    public abstract void UseAttack(AttackOwner attackOwner, GameObject ownerObj, GameObject targetObj);
}
