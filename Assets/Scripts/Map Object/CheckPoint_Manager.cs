using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint_Manager : MonoBehaviour
{
    public static CheckPoint_Manager instance;

    // �÷��̾� �������ͽ�
    [Header("=== Player Data ===")]
    [SerializeField] private int maxHp;
    [SerializeField] private int curHp;
    [SerializeField] private float physicalDefense;
    [SerializeField] private float magicalDefense;
    [SerializeField] private int physicalDamage;
    [SerializeField] private int magcialDamage;
    [SerializeField] private int criticalChance;
    [SerializeField] private float criticalMultiplier;
    [SerializeField] private int curUndersideGauge;
    [SerializeField] private int maxUndersideGauge;
    [SerializeField] private int maxAttackPoint;
    [SerializeField] private Vector2Int slotSpeed;


    // �������� ����
    [Header("=== Stage Data ===")]
    [SerializeField] private bool isDoor1;
    [SerializeField] private bool isDoor2;
    [SerializeField] private bool isDoor3;
    [SerializeField] private bool isMonster1;
    [SerializeField] private bool isMonster2;
    [SerializeField] private bool isMonster3;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// üũ����Ʈ���� ��ȣ�ۿ��ϸ� ���� �÷��̾��� �����͸� �Ŵ��� ��ũ��Ʈ�� �����ϴ� ��� / ȣ�� �� �ڵ� ����
    /// </summary>
    public void Player_DataUpdate()
    {

    }


    /// <summary>
    /// üũ����Ʈ���� ��ȣ�ۿ��ϸ� ���� ���������� �����͸� �Ŵ��� ��ũ��Ʈ�� �����ϴ� ��� / ȣ�� �� �ڵ� ����
    /// </summary>
    public void Stage_DataUpdate()
    {

    }


    /// <summary>
    /// �÷��̾� ��� �� ȣ�� �� �÷��̾ ���� ���̺�����Ʈ���� ��Ȱ / ȣ�� �� �ڵ� ����
    /// </summary>
    public void Respawn()
    {

    }
}
