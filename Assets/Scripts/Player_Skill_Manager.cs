using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player_Skill_Manager : MonoBehaviour
{
    public static Player_Skill_Manager instance;

    [Header("=== Skill Data ===")]
    public List<Player_Skill_Slot> skill_List;


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

    public void Learn_Skill(Player_Skill_Slot slot)
    {

    }
}
