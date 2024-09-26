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
        // 선행 스킬 인덱스 체크
        for (int i = 0; i < node.learn_Conditions_Skill_Index.Length; i++)
        {
            if (!skill_List[node.learn_Conditions_Skill_Index[i]].isLearn)
            {
                return;
            }
        }

        // 모든 조건이 만족되었을 때 스킬 해금
        node.isLearn = true;
        node.skill_List_Object.SetActive(true);
    }
}
