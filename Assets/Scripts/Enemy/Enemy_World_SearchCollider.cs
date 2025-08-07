using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_World_SearchCollider : MonoBehaviour
{
    [Header("=== Setting ===")]
    [SerializeField] private Enemy_World enemy_World;


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            enemy_World.Chase(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemy_World.Return();
        }

    }
}
