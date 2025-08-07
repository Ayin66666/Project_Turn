using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class Player_Skill_Node : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("=== Node Setting ===")]
    [SerializeField] private Attack_Base attackData; // 해당 노드가 가지고 있는 스킬 데이터
    [SerializeField] private GameObject skillObject; // 배우면 활성화 될 UI
    public bool isLearn; // 배웠는지 체크
    [SerializeField] private bool havePriorSkill; // 선행 스킬 유무 체크
    [SerializeField] private List<Player_Skill_Node> priorSkill;


    [Header("=== Node UI Setting ===")]
    [SerializeField] private Image icon_Image;
    [SerializeField] private Text cost_Text;
    [SerializeField] private RectTransform rectPos;


    [Header("=== Line UI Setting ===")]
    [SerializeField] private Sprite lineSprtie;
    [SerializeField] private Image line;
    
    #region 프로퍼티
    public Attack_Base AttackData { get { return attackData; }}
    public bool Learn { get { return isLearn; }}
    #endregion

    private void Start()
    {
        // UI 셋팅
        icon_Image.sprite = attackData.icon;
        cost_Text.text = attackData.learnCost.ToString();
        rectPos = GetComponent<RectTransform>();
    }


    #region 마우스 이벤트
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 스킬 설명 출력
        Player_UI.instance.Skill_DescriptionUI(true, attackData, rectPos);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Check Node");

        // 이미 배웠는지 체크
        if(isLearn)
        {
            Player_UI.instance.Skill_LearnResultUI(null, 3);
            return;
        }

        // 선행 스킬 체크
        if (havePriorSkill)
        {
            for (int i = 0; i < priorSkill.Count; i++)
            {
                if(!priorSkill[i].isLearn)
                {
                    // 조건 불만족시
                    Player_UI.instance.Skill_LearnResultUI(null, 2);
                    return;
                }
            }
        }

        // 스킬 포인트 체크
        if(Player_Skill_Manager.instance.Skill_PointCheck(attackData))
        {
            // 조건 만족시
            isLearn = true;
            skillObject.SetActive(true);
            Player_UI.instance.Skill_LearnResultUI(attackData, 0);

            if(line != null)
            {
                line.sprite = lineSprtie;
            }

            // UI 최신화
            Player_UI.instance.World_SkillPoint();
            return;
        }
        else
        {
            // 조건 불만족시
            Player_UI.instance.Skill_LearnResultUI(null, 1);
            return;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 스킬 설명 종료
        Player_UI.instance.Skill_DescriptionUI(false, attackData, rectPos);
    }
    #endregion
}
