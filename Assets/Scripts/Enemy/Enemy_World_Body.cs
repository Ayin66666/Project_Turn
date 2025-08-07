using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_World_Body : MonoBehaviour
{
    [SerializeField] private Enemy_World enemy_World;
    [SerializeField] private TurnFight_Manager manager;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            enemy_World.Find();
            manager.TurnFight_Start();
            //Destroy(gameObject);
        }
    }
}
