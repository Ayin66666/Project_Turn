using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_EventPoint : MonoBehaviour
{
    [Header("=== 포인트 셋팅 ===")]
    [SerializeField] private bool isUsed;
    [SerializeField] private Collider checkCollier;


    [Header("=== 웨이포인트 ===")]
    [SerializeField] private bool haveWayPoint;
    [SerializeField] private int wayPointIndex;


    [Header("=== 다이얼로그 ===")]
    [SerializeField] private bool haveDialog;
    [SerializeField] private int dialogIndex;


    [Header("=== 튜토리얼 ===")]
    [SerializeField] private bool haveTutorial;
    [SerializeField] private int tutorialIndex;
    private void Used()
    {
        isUsed = true;
        checkCollier.enabled = false;

        // 다이얼로그
        if(haveDialog)
        {
            Dialog_Manager.instance.Dialog(dialogIndex);
        }

        // 웨이포인트
        if (haveWayPoint)
        {
            Waypoint_Manager.instance.Waypoint_Setting(true, wayPointIndex);
        }

        // 튜토리얼
        if(haveTutorial)
        {
            Tutorial_Manager.instance.TutorialOn(tutorialIndex);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !isUsed)
        {
            Used();
            gameObject.SetActive(false);
        }
    }

}
