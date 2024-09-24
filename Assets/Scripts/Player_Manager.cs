using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Manager : MonoBehaviour
{
    public static Player_Manager instnace;

    // 액션으로 제어
    // 턴제 턴을 플레이어 매니저가 관리
    // 현제 턴 + 버프지속 턴 계산으로 지속시간 관리
    // 합연산으로 버프 관리
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


    // 데미지 계산 -> 튜플 사용 (1개 이상의 값을 반환할 때 / C# 7.0 이상부터 가능!)
    public (int, bool) DamageCal(Attack_Base attack, int count)
    {
        // 데미지 공식 (기초 데미지 * 버프1 * 버프2 ... ) * 공격 배율 * 치명타 배율

        // 물 & 마 데미지 인풋
        int baseDamage = attack.damageType[count] == Attack_Base.DamageType.physical ? physicalDamage : magcialDamage;

        // 공격 배율 계산
        float valueDamage = baseDamage * Random.Range(attack.damageValue[count].x, baseDamage * attack.damageValue[count].y);

        // 크리티컬 데미지 계산
        int ran = Random.Range(0, 100);
        valueDamage = ran <= criticalChance ? valueDamage *= criticalChance : valueDamage;

        // 데미지 & 크리티컬 여부 반환
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
