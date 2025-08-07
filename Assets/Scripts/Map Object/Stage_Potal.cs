using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_Potal : MonoBehaviour
{
    [Header("=== Setting ===")]
    [SerializeField] private Type type;
    [SerializeField] private string stageName;
    [SerializeField] private Transform movePos;
    private bool isUsed;
    private enum Type { NextScene, JustMove }


    /// <summary>
    /// ������ ������ �̵�
    /// </summary>
    private void Portal_NextScene()
    {
        StartCoroutine(NextSceneCall());
    }

    private IEnumerator NextSceneCall()
    {
        // �̵� ���� - ������ �������� �Ŵ������� ������ - �̰� ���߿� ��ġ ���� ��� �߰��ؼ� �ٲٴ°ŵ� ������?
        Player_Manager.instnace.canMove = false;

        // �÷��̾� �߷� ����
        Player_Manager.instnace.player_World.useGravity = false;

        // ����Ʈ UI ����
        Quset_Manager.instance.QusetUI_Setting(false);

        // ���̵� UI ����
        Player_UI.instance.Stage_StartEnd(false, null);
        while(Player_UI.instance.isFade)
        {
            yield return null;
        }

        // �÷��̾� ��ġ ����
        Player_Manager.instnace.World_PlayerPos_Setting(movePos.position);

        // �� ��ȯ
        Scene_Loading.LoadScene(stageName);
    }

    /// <summary>
    /// ������ ��ġ�� �����̵�
    /// </summary>
    private void Portal_JustMove()
    {
        Player_Manager.instnace.player_World.transform.position = movePos.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !isUsed)
        {
            isUsed = true;

            switch (type)
            {
                case Type.NextScene:
                    Portal_NextScene();
                    break;

                case Type.JustMove:
                    Portal_JustMove();
                    break;
            }
        }
    }
}
