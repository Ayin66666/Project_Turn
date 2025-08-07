using System.Collections.Generic;
using UnityEngine;


public abstract class Attack_Base : MonoBehaviour
{
    [Header("=== Attack Base State ===")]
    public Type type; // 공격 타입
    public AttackOwner owner; // 해당 공격의 주인
    public string attackName; // 공격 이름
    [TextArea] public string attackDescription_Text; // 공격 설명
    [TextArea] public string summaryDescriptionText; // 합 공격 간략 설명
    public int attackCost; // 자원 소모량
    public int attackCount; // 공격 횟수
    public bool isAOE; // 다중 공격 확인
    public int aoeCount; // 다중 공격 인수
    public Vector2[] damageValue; // 각 합마다 데미지 벨류
    public DamageType[] damageType; // 각 공격마다 물리&마법 데미지 체크
    public Sprite icon;
    public int learnCost; // 배우기 위한 코스트

    public enum AttackOwner { Player, Enemy }
    public enum DamageType { physical, magicl }
    public enum Type { Attack, Defense, Buff, Complex } // 나중에 스킬을 태그 별로 나눈다면?


    [Header("---Component---")]
    [SerializeField] protected Animator anim = null;
    [SerializeField] protected GameObject ownerObject;
    [SerializeField] protected GameObject target;


    /// <summary>
    /// 공격 호출 함수 - 한명 공격
    /// </summary>
    /// <param name="attackOwner">공격의 주인</param>
    /// <param name="ownerObj">공격의 주인 오브젝트</param>
    /// <param name="targetObj">타겟의 오브젝트</param>
    /// <param name="isExchange">합 여부</param>
    /// <param name="attackCount">남은 공격 횟수</param>
    public abstract void UseAttack(AttackOwner attackOwner, GameObject ownerObj, GameObject targetObj, bool isExchange, int attackCount);

    /// <summary>
    /// 공격 호출 함수 - 다수 공격
    /// </summary>
    /// <param name="attackOwner">공격의 주인</param>
    /// <param name="ownerObj">공격의 주인 오브젝트</param>
    /// <param name="targetObj">타겟의 오브젝트</param>
    /// <param name="isExchange">합 여부</param>
    /// <param name="attackCount">남은 공격 횟수</param>
    public abstract void UseAttack(AttackOwner attackOwner, GameObject ownerObj, List<GameObject> targetObj, bool isExchange, int attackCount);
}
