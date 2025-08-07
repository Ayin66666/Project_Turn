using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Enemy_MapChange : MonoBehaviour
{
    [Header("=== Setting ===")]
    [SerializeField] private Enemy_Base enemy;
    [SerializeField] private MeshRenderer groundRenderer;
    [SerializeField] private Material groundMaterial;
    [SerializeField] private Material skyBox;
    [SerializeField] private int checkDelay;
    private Coroutine checkCoroutine;


    private void Start()
    {
        checkCoroutine = StartCoroutine(Check());
    }

    private IEnumerator Check()
    {
        // 3�ʸ��� üũ
        while(enemy.isPhase2)
        {
            yield return new WaitForSeconds(3);
        }

        // �� ����
        Map_Change();
    }

    public void Map_Change()
    {
        groundRenderer.material = groundMaterial;
        RenderSettings.skybox = skyBox;
    }
}
