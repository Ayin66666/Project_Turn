using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private TurnFight_Manager min;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            min.TurnFight_Start();
        }
    }
}
