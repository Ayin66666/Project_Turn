using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_EventPoint : MonoBehaviour
{
    [Header("=== ����Ʈ ���� ===")]
    [SerializeField] private bool isUsed;
    [SerializeField] private Collider checkCollier;


    [Header("=== ��������Ʈ ===")]
    [SerializeField] private bool haveWayPoint;
    [SerializeField] private int wayPointIndex;


    [Header("=== ���̾�α� ===")]
    [SerializeField] private bool haveDialog;
    [SerializeField] private int dialogIndex;


    [Header("=== Ʃ�丮�� ===")]
    [SerializeField] private bool haveTutorial;
    [SerializeField] private int tutorialIndex;
    private void Used()
    {
        isUsed = true;
        checkCollier.enabled = false;

        // ���̾�α�
        if(haveDialog)
        {
            Dialog_Manager.instance.Dialog(dialogIndex);
        }

        // ��������Ʈ
        if (haveWayPoint)
        {
            Waypoint_Manager.instance.Waypoint_Setting(true, wayPointIndex);
        }

        // Ʃ�丮��
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
