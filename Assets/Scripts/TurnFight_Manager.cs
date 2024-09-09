using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnFight_Manager : MonoBehaviour
{
    [Header("=== State ===")]
    [SerializeField] private Turn curTurn;
    private enum Turn { Start, Select, Fight, End }


    [Header("=== Pos ===")]
    [SerializeField] private Transform[] enemyPos;
    [SerializeField] private Transform playerPos;


    [Header("=== Enemy ===")]
    [SerializeField] private List<Enemy_Base> enemys;
    [SerializeField] private int enemyCount;


    // 1. 페이드 연출 호출

    // 2. 전투 필드로 플레이어 & 몬스터 활성화

    // 3. 페이드 연출 종료 후 플레이어 시작 모션

    // 4. 플레이어 스킬 선택

    // 5. 선택 UI 종료

    // 6. 플레이어 - 몬스터 이동

    // 7. 플레이어 - 몬스터 합

    // 8. 플레이어 - 몬스터 원위치

    // 이하 4 ~ 8 반복


    public void Turn_Start()
    {
        StartCoroutine(Turn_StartCall());
    }

    private IEnumerator Turn_StartCall() // 1 2 3 동작
    {
        curTurn = Turn.Start;

        // 1. 페이드 연출 호출
        Player_UI.instance.TurnFight_Fade();


        // 페이드 인 될때까지 대기
        yield return new WaitForSeconds(1f);


        // 2. 전투 필드로 플레이어 & 몬스터 이동
        Player_Manager.instnace.player_Turn.gameObject.transform.position = playerPos.position;
        for (int i = 0; i < enemys.Count; i++)
        {
            enemys[i].gameObject.SetActive(true);
        }


        // 3. 페이드 연출 종료 후 플레이어 시작 모션
        while (Player_UI.instance.isFade)
        {
            yield return null;
        }
        Player_Manager.instnace.player_Turn.Turn_StartAnim();
        while(Player_Manager.instnace.player_Turn.isSpawnAnim)
        {
            yield return null;
        }

        // 페이드 아웃 후 일시 대기
        yield return new WaitForSeconds(1f);

        StartCoroutine(Turn_Select());
    }

    private IEnumerator Turn_Select()
    {
        curTurn = Turn.Select;

        // 4. 플레이어 스킬 선택
        Player_Manager.instnace.player_Turn.Turn_Select();
        while(Player_Manager.instnace.player_Turn.isSelect)
        {
            yield return null;
        }
    }

    private IEnumerator Turn_Fight()
    {
        curTurn = Turn.Fight;
        yield return null;

        // 5. 선택 UI 종료
        Player_UI.instance.TurnFight_Select(false);

        // 6. 플레이어 - 몬스터 이동
        Player_Manager.instnace.player_Turn.Turn_EngageMove();


        // 7. 플레이어 - 몬스터 합


        // 8. 플레이어 - 몬스터 원위치

    }
}
