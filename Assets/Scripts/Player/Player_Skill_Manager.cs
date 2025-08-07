using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player_Skill_Manager : MonoBehaviour
{
    public static Player_Skill_Manager instance;

    [Header("=== Skill Manager Setting ===")]
    [SerializeField] private List<Player_Skill_Node> nodeList;
    [SerializeField] private int skillPoint;

    public int SkillPoint
    {
        get { return skillPoint; }
        private set { skillPoint = value; }
    }


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    /// <summary>
    /// ������ or ��ų����Ʈ �������� ȹ������ �� ȣ��
    /// </summary>
    /// <param name="point">�߰��� ����Ʈ</param>
    public void Add_SkillPoint(int point)
    {
        skillPoint += point;

        // UI
        Player_UI.instance.SkillPointUI(point);
    }


    /// <summary>
    /// ���� ��ų�� ���� ��� �ش� ������ �����Ǿ����� üũ
    /// </summary>
    /// <param name="node_Index">���� �н��� �ʿ��� ��ų�� �ε��� ����Ʈ</param>
    /// <returns></returns>
    public bool Skill_Check(Attack_Base attackData, List<int> node_Index)
    {
        // ���� ��ų üũ
        for (int i = 0; i < node_Index.Count; i++)
        {
            // ���� üũ
            if (!nodeList[i].Learn)
            {
                // ���� �Ҹ��� ǥ�� UI -> PlayerUI���� ȣ��
                Debug.Log("���� ���ེų " + nodeList[i].AttackData.attackName + " �� ����� �ʾҽ��ϴ�!");
                return false;
            }
        }
        return true;
    }


    /// <summary>
    /// ��ų ����Ʈ ���� üũ
    /// </summary>
    /// <param name="attackData">������ �ϴ� ��ų�� ������</param>
    /// <returns></returns>
    public bool Skill_PointCheck(Attack_Base attackData)
    {
        // ��ų ����Ʈ üũ
        if (skillPoint >= attackData.learnCost)
        {
            skillPoint -= attackData.learnCost;
            return true;
        }
        else
        {
            Debug.Log("��ų ����Ʈ ����!");
            return false;
        }
    }
}
