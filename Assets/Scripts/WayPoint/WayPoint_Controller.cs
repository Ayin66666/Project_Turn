using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WayPoint_Controller : MonoBehaviour
{
    public WayPoint_Base wayPoint_Base;

    public GameObject previous_Parent;
    public GameObject wayPoint_UI;
    [SerializeField] private GameObject icon_Obj;
    [SerializeField] private GameObject wayPoint_Canvas;
    [SerializeField] private GameObject worldWaypoints;

    private void OnEnable()
    {
        previous_Parent = this.gameObject;
        WayPoint_Setting();
    }

    void Update()
    {
        // ��ȣ�ۿ� �Ÿ�üũ
        if(wayPoint_Base.Get_Distance(transform.position, wayPoint_Base.target.transform.position) < wayPoint_Base.interact_Distance)
        {
            wayPoint_Base.Enable_WayPoint(false);
        }
        else
        {
            wayPoint_Base.Enable_WayPoint(true);
        }
    }

    /// <summary>
    /// ��������Ʈ ������ ����
    /// </summary>
    void WayPoint_Setting()
    {
        wayPoint_Base.image = wayPoint_UI.GetComponentInChildren<Image>();
        wayPoint_Base.text = wayPoint_UI.GetComponentInChildren<Text>();

        // ��������Ʈ�� �÷��̾� ��ġ ����
        wayPoint_Base.Set_Target(GameObject.FindGameObjectWithTag("Player"));
        wayPoint_Base.Set_Transform(transform);

        // ��������Ʈ UI ĵ���� ����
        icon_Obj.transform.SetParent(wayPoint_Canvas.transform);

        // ��������Ʈ ������Ʈ ����
        wayPoint_Base.wayPoint_UI = wayPoint_UI;
        transform.SetParent(worldWaypoints.transform);
    }
}
