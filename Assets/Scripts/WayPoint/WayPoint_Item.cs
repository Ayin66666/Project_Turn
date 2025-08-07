using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct WayPoint_Item
{
    public Sprite icon;
    public float height_OffSet;
    [HideInInspector] public Image image;
    [HideInInspector] public Text text;
    [HideInInspector] public GameObject effect;
    [HideInInspector] public GameObject wayPoint_UI;
    [HideInInspector] public GameObject target;
    [HideInInspector] public Transform transform;
}
