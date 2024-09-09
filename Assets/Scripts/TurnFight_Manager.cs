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


    // 1. ���̵� ���� ȣ��

    // 2. ���� �ʵ�� �÷��̾� & ���� Ȱ��ȭ

    // 3. ���̵� ���� ���� �� �÷��̾� ���� ���

    // 4. �÷��̾� ��ų ����

    // 5. ���� UI ����

    // 6. �÷��̾� - ���� �̵�

    // 7. �÷��̾� - ���� ��

    // 8. �÷��̾� - ���� ����ġ

    // ���� 4 ~ 8 �ݺ�


    public void Turn_Start()
    {
        StartCoroutine(Turn_StartCall());
    }

    private IEnumerator Turn_StartCall() // 1 2 3 ����
    {
        curTurn = Turn.Start;

        // 1. ���̵� ���� ȣ��
        Player_UI.instance.TurnFight_Fade();


        // ���̵� �� �ɶ����� ���
        yield return new WaitForSeconds(1f);


        // 2. ���� �ʵ�� �÷��̾� & ���� �̵�
        Player_Manager.instnace.player_Turn.gameObject.transform.position = playerPos.position;
        for (int i = 0; i < enemys.Count; i++)
        {
            enemys[i].gameObject.SetActive(true);
        }


        // 3. ���̵� ���� ���� �� �÷��̾� ���� ���
        while (Player_UI.instance.isFade)
        {
            yield return null;
        }
        Player_Manager.instnace.player_Turn.Turn_StartAnim();
        while(Player_Manager.instnace.player_Turn.isSpawnAnim)
        {
            yield return null;
        }

        // ���̵� �ƿ� �� �Ͻ� ���
        yield return new WaitForSeconds(1f);

        StartCoroutine(Turn_Select());
    }

    private IEnumerator Turn_Select()
    {
        curTurn = Turn.Select;

        // 4. �÷��̾� ��ų ����
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

        // 5. ���� UI ����
        Player_UI.instance.TurnFight_Select(false);

        // 6. �÷��̾� - ���� �̵�
        Player_Manager.instnace.player_Turn.Turn_EngageMove();


        // 7. �÷��̾� - ���� ��


        // 8. �÷��̾� - ���� ����ġ

    }
}
