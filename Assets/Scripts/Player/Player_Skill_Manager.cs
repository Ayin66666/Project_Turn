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
    /// 레벨업 or 스킬포인트 아이템을 획득했을 때 호출
    /// </summary>
    /// <param name="point">추가할 포인트</param>
    public void Add_SkillPoint(int point)
    {
        skillPoint += point;

        // UI
        Player_UI.instance.SkillPointUI(point);
    }


    /// <summary>
    /// 선행 스킬이 있을 경우 해당 조건이 만족되었는지 체크
    /// </summary>
    /// <param name="node_Index">선행 학습이 필요한 스킬의 인덱스 리스트</param>
    /// <returns></returns>
    public bool Skill_Check(Attack_Base attackData, List<int> node_Index)
    {
        // 선행 스킬 체크
        for (int i = 0; i < node_Index.Count; i++)
        {
            // 조건 체크
            if (!nodeList[i].Learn)
            {
                // 조건 불만족 표기 UI -> PlayerUI에서 호출
                Debug.Log("아직 선행스킬 " + nodeList[i].AttackData.attackName + " 을 배우지 않았습니다!");
                return false;
            }
        }
        return true;
    }


    /// <summary>
    /// 스킬 포인트 충족 체크
    /// </summary>
    /// <param name="attackData">배우려고 하는 스킬의 데이터</param>
    /// <returns></returns>
    public bool Skill_PointCheck(Attack_Base attackData)
    {
        // 스킬 포인트 체크
        if (skillPoint >= attackData.learnCost)
        {
            skillPoint -= attackData.learnCost;
            return true;
        }
        else
        {
            Debug.Log("스킬 포인트 부족!");
            return false;
        }
    }
}
