using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player_Skill_Node : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("=== Skill Status Setting ===")]
    public SkillType type;
    public string skill_Name;
    public int skill_cost;
    public bool isLearn;

    public enum SkillType { physical, magicl, Compound }


    [Header("=== Skill UI Setting ===")]
    [SerializeField] private Attack_Base myAttack;
    public GameObject skill_List_Object;
    public int[] learn_Conditions_Skill_Index;


    #region 마우스 이벤트

    public void Click_Button()
    {
        if (!isLearn)
        {
            Player_Skill_Manager.instance.Skill_Learn_Check(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Player_UI.instance.Skill_DescriptionUI(true, myAttack, this.GetComponent<RectTransform>());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Player_UI.instance.Skill_DescriptionUI(false, myAttack, this.GetComponent<RectTransform>());
    }

    #endregion
}
