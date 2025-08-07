using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialog Data", menuName = "Scriptalbe Object / Dialog_Data", order = int.MaxValue)]
public class Dialog_Data : ScriptableObject
{
    public List<Line_Data> line_Data;
}

[System.Serializable]
public class Line_Data
{
    [Header("=== Setting ===")]
    public LineType type;
    public Evnet_Type evnetType;
    public Event_StartPos eventStartPos;
    public bool haveEvnet;
    public int eventIndex;
    public float textDelay;


    [Header("=== UI ===")]
    public Sprite icon;
    public string name_Text;
    [TextArea] public string dialog_Text;


    public enum LineType { Player, Enemy, Systeam, NPC, None }
    public enum Evnet_Type { None, Door, WayPoint, Portal, Quset, Tutorial }
    public enum Event_StartPos { None, TextStart, TextEnd }
}
