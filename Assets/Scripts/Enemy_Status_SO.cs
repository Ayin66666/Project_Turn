using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_Status", menuName = "Scriptalbe Ojbect / Enemy_Status", order = int.MaxValue)]
public class Enemy_Status_SO : ScriptableObject
{
    [SerializeField] private string enemy_Name;
    public string EnemyName {  get { return enemy_Name; } }



    [SerializeField] private int hp;
    public int Hp {  get { return hp; } }

    [SerializeField] private int physicalDefense;
    public int PhysicalDefense { get {  return physicalDefense; } }

    [SerializeField] private int magicalDefense;
    public int MagicalDefense { get { return magicalDefense; } }



    [SerializeField] private int physicalDamage;
    public int PhysicalDamage { get { return physicalDamage; } }

    [SerializeField] private int magcialDamage;
    public int MagcialDamage { get { return magcialDamage; } }



    [SerializeField] private float criticalChance;
    public float CriticalChance { get { return criticalChance; } }

    [SerializeField] private float criticalMultiplier;
    public float CriticalMultiplier { get { return criticalMultiplier; } }


    [SerializeField] private Vector2Int slotSpeed;
    public Vector2Int SlotSpeed { get { return slotSpeed; } }
}
