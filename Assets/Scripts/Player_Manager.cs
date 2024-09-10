using System;
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
    public Action buffAction;

    public enum State { World, Turn }
    public enum HitType { None, Groggy }


    [Header("=== Component ===")]
    public Player_World player_World;
    public Player_Turn player_Turn;
    public Player_UI player_UI;


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

    public void Type_Setting(State state)
    {
        this.state = state;
        switch (state)
        {
            case State.World:
                // Cam Setting

                // Turn Body Off
                buffAction = null;

                // Start Animation
                break;

            case State.Turn:
                // Cam Setting

                // World Body Off
                buffAction = null;

                // Return Idle
                break;
        }
    }
}
