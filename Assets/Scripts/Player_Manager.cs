using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Manager : MonoBehaviour
{
    public static Player_Manager instnace;

    // �׼����� ����
    // ���� ���� �÷��̾� �Ŵ����� ����
    // ���� �� + �������� �� ������� ���ӽð� ����
    // �տ������� ���� ����
    public System.Action buffAction;

    public enum State { World, Turn }
    public enum DamageType { physical, magical }
    public enum HitEffect { None, Groggy }

    [Header("=== Component ===")]
    public Player_World player_World;
    public Player_Turn player_Turn;
    public Player_UI player_UI;
    public TurnFight_Manager turnManger = null;

    [Header("=== State ===")]
    public State state;
    public bool isAttack;
    public bool canMove;
    public bool isDie;


    [Header("=== Status ===")]
    public int maxHp;
    public int curHp;
    [SerializeField] private int physicalDefense;
    [SerializeField] private int magicalDefense;

    [SerializeField] private int physicalDamage;
    [SerializeField] private int magcialDamage;

    [SerializeField] private int criticalChance;
    [SerializeField] private int criticalMultiplier;

    [SerializeField] private int attackPoint;
    [SerializeField] private Vector2Int slotSpeed;
  
    
    #region Property
    public int PhysicalDefense
    {
        get { return physicalDefense; }
        private set { physicalDefense = value; }
    }
    public int MagicDefense
    {
        get { return magicalDefense; }
        private set { magicalDefense = value; }
    }

    public int PhysicalDamage
    {
        get { return physicalDamage; }
        private set { physicalDamage = value; }
    }
    public int MagcialDamage
    {
        get { return magcialDamage; }
        private set { magcialDamage = value; }
    }

    public int CriticalChance
    {
        get { return criticalChance; }
        private set { criticalChance = value; }
    }
    public int CriticalMultiplier
    {
        get { return criticalMultiplier; }
        private set { criticalMultiplier = value; }
    }
    public int AttackPoint
    {
        get{ return attackPoint; }
        private set { attackPoint = value; }
    }
    public Vector2Int SlotSpeed
    {
        get { return slotSpeed; }
        private set { slotSpeed = value; }
    }

    #endregion


    public void Awake()
    {
        if(instnace != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instnace = this;
        }
    }


    // ������ ��� -> Ʃ�� ��� (1�� �̻��� ���� ��ȯ�� �� / C# 7.0 �̻���� ����!)
    public (int, bool) DamageCal(Attack_Base attack, int count)
    {
        // ������ ���� (���� ������ * ����1 * ����2 ... ) * ���� ���� * ġ��Ÿ ����

        // �� & �� ������ ��ǲ
        int baseDamage = attack.damageType[count] == Attack_Base.DamageType.physical ? physicalDamage : magcialDamage;

        // ���� ���� ���
        float valueDamage = baseDamage * Random.Range(attack.damageValue[count].x, baseDamage * attack.damageValue[count].y);

        // ũ��Ƽ�� ������ ���
        int ran = Random.Range(0, 100);
        valueDamage = ran <= criticalChance ? valueDamage *= criticalChance : valueDamage;

        // ������ & ũ��Ƽ�� ���� ��ȯ
        return ((int)valueDamage, ran <= criticalChance ? true : false);
    }


    public void Take_Damage(int damage, DamageType type)
    {
        if(isDie)
        {
            return;
        }

        switch (type)
        {
            case DamageType.physical:
                curHp -= damage * physicalDefense;
                break;

            case DamageType.magical:
                curHp -= damage * magicalDefense;
                break;
        }

        if(curHp <= 0)
        {
            
        }
    }

    public void Turn_Fight_Start(Transform movePos, TurnFight_Manager manager)
    {
        turnManger = manager;
        player_Turn.gameObject.transform.position = movePos.position;
        player_World.gameObject.SetActive(false);
        player_Turn.gameObject.SetActive(true);
    }

    public void Turn_Fight_End()
    {

    }
}
