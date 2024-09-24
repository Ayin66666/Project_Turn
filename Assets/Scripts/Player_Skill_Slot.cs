using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Player_Skill_Slot : MonoBehaviour
{
    [Header("=== Skill Setting ===")]
    public SkillType type;
    public string skill_Name;
    public int skill_cost;
    public bool isLearn;
    public int[] learn_conditions;
    public GameObject skill_List_Object;

    public enum SkillType { physical, magicl, Compound }


    public bool Learn_Check(List<Player_Skill_Slot> list)
    {
        // 선행 스킬 인덱스 체크
        for (int i = 0; i < learn_conditions.Length; i++)
        {
            if (!list[learn_conditions[i]].isLearn)
            {
                return false;
            }
        }

        return true;
    }
}
