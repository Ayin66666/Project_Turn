using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class Player_Skill_Node : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("=== Node Setting ===")]
    [SerializeField] private Attack_Base attackData; // �ش� ��尡 ������ �ִ� ��ų ������
    [SerializeField] private GameObject skillObject; // ���� Ȱ��ȭ �� UI
    public bool isLearn; // ������� üũ
    [SerializeField] private bool havePriorSkill; // ���� ��ų ���� üũ
    [SerializeField] private List<Player_Skill_Node> priorSkill;


    [Header("=== Node UI Setting ===")]
    [SerializeField] private Image icon_Image;
    [SerializeField] private Text cost_Text;
    [SerializeField] private RectTransform rectPos;


    [Header("=== Line UI Setting ===")]
    [SerializeField] private Sprite lineSprtie;
    [SerializeField] private Image line;
    
    #region ������Ƽ
    public Attack_Base AttackData { get { return attackData; }}
    public bool Learn { get { return isLearn; }}
    #endregion

    private void Start()
    {
        // UI ����
        icon_Image.sprite = attackData.icon;
        cost_Text.text = attackData.learnCost.ToString();
        rectPos = GetComponent<RectTransform>();
    }


    #region ���콺 �̺�Ʈ
    public void OnPointerEnter(PointerEventData eventData)
    {
        // ��ų ���� ���
        Player_UI.instance.Skill_DescriptionUI(true, attackData, rectPos);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Check Node");

        // �̹� ������� üũ
        if(isLearn)
        {
            Player_UI.instance.Skill_LearnResultUI(null, 3);
            return;
        }

        // ���� ��ų üũ
        if (havePriorSkill)
        {
            for (int i = 0; i < priorSkill.Count; i++)
            {
                if(!priorSkill[i].isLearn)
                {
                    // ���� �Ҹ�����
                    Player_UI.instance.Skill_LearnResultUI(null, 2);
                    return;
                }
            }
        }

        // ��ų ����Ʈ üũ
        if(Player_Skill_Manager.instance.Skill_PointCheck(attackData))
        {
            // ���� ������
            isLearn = true;
            skillObject.SetActive(true);
            Player_UI.instance.Skill_LearnResultUI(attackData, 0);

            if(line != null)
            {
                line.sprite = lineSprtie;
            }

            // UI �ֽ�ȭ
            Player_UI.instance.World_SkillPoint();
            return;
        }
        else
        {
            // ���� �Ҹ�����
            Player_UI.instance.Skill_LearnResultUI(null, 1);
            return;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // ��ų ���� ����
        Player_UI.instance.Skill_DescriptionUI(false, attackData, rectPos);
    }
    #endregion
}
