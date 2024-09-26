using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player_Skill_Manager : MonoBehaviour
{
    public static Player_Skill_Manager instance;

    [Header("=== Skill Data ===")]
    public List<Player_Skill_Node> skill_List;


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

    public void Skill_Learn_Check(Player_Skill_Node node)
    {
        // ���� ��ų �ε��� üũ
        for (int i = 0; i < node.learn_Conditions_Skill_Index.Length; i++)
        {
            if (!skill_List[node.learn_Conditions_Skill_Index[i]].isLearn)
            {
                return;
            }
        }

        // ��� ������ �����Ǿ��� �� ��ų �ر�
        node.isLearn = true;
        node.skill_List_Object.SetActive(true);
    }
}
