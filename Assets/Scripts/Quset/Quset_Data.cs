using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Quset Data", menuName = "Scriptalbe Object / Quset_Data", order = int.MaxValue)]
public class Quset_Data : ScriptableObject
{

    [SerializeField] private string qusetName;
    public string QusetName { get { return qusetName; } }


    [SerializeField][TextArea] private string qusetDescription;
    public string QusetDescription { get { return qusetDescription; } }
}
