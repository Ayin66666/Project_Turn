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
    /// 지정한 씬으로 이동
    /// </summary>
    private void Portal_NextScene()
    {
        StartCoroutine(NextSceneCall());
    }

    private IEnumerator NextSceneCall()
    {
        // 이동 제한 - 해제는 스테이지 매니져에서 동작함 - 이거 나중에 위치 셋팅 기능 추가해서 바꾸는거도 좋을듯?
        Player_Manager.instnace.canMove = false;

        // 플레이어 중력 동작
        Player_Manager.instnace.player_World.useGravity = false;

        // 퀘스트 UI 종료
        Quset_Manager.instance.QusetUI_Setting(false);

        // 페이드 UI 동작
        Player_UI.instance.Stage_StartEnd(false, null);
        while(Player_UI.instance.isFade)
        {
            yield return null;
        }

        // 플레이어 위치 셋팅
        Player_Manager.instnace.World_PlayerPos_Setting(movePos.position);

        // 씬 전환
        Scene_Loading.LoadScene(stageName);
    }

    /// <summary>
    /// 지정한 위치로 순간이동
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
