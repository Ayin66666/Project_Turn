using System.Collections.Generic;
using UnityEngine;


public abstract class Attack_Base : MonoBehaviour
{
    [Header("=== Attack Base State ===")]
    public Type type; // ���� Ÿ��
    public AttackOwner owner; // �ش� ������ ����
    public string attackName; // ���� �̸�
    [TextArea] public string attackDescription_Text; // ���� ����
    [TextArea] public string summaryDescriptionText; // �� ���� ���� ����
    public int attackCost; // �ڿ� �Ҹ�
    public int attackCount; // ���� Ƚ��
    public bool isAOE; // ���� ���� Ȯ��
    public int aoeCount; // ���� ���� �μ�
    public Vector2[] damageValue; // �� �ո��� ������ ����
    public DamageType[] damageType; // �� ���ݸ��� ����&���� ������ üũ
    public Sprite icon;
    public int learnCost; // ���� ���� �ڽ�Ʈ

    public enum AttackOwner { Player, Enemy }
    public enum DamageType { physical, magicl }
    public enum Type { Attack, Defense, Buff, Complex } // ���߿� ��ų�� �±� ���� �����ٸ�?


    [Header("---Component---")]
    [SerializeField] protected Animator anim = null;
    [SerializeField] protected GameObject ownerObject;
    [SerializeField] protected GameObject target;


    /// <summary>
    /// ���� ȣ�� �Լ� - �Ѹ� ����
    /// </summary>
    /// <param name="attackOwner">������ ����</param>
    /// <param name="ownerObj">������ ���� ������Ʈ</param>
    /// <param name="targetObj">Ÿ���� ������Ʈ</param>
    /// <param name="isExchange">�� ����</param>
    /// <param name="attackCount">���� ���� Ƚ��</param>
    public abstract void UseAttack(AttackOwner attackOwner, GameObject ownerObj, GameObject targetObj, bool isExchange, int attackCount);

    /// <summary>
    /// ���� ȣ�� �Լ� - �ټ� ����
    /// </summary>
    /// <param name="attackOwner">������ ����</param>
    /// <param name="ownerObj">������ ���� ������Ʈ</param>
    /// <param name="targetObj">Ÿ���� ������Ʈ</param>
    /// <param name="isExchange">�� ����</param>
    /// <param name="attackCount">���� ���� Ƚ��</param>
    public abstract void UseAttack(AttackOwner attackOwner, GameObject ownerObj, List<GameObject> targetObj, bool isExchange, int attackCount);
}
