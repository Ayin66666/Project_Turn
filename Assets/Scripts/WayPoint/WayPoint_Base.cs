using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class WayPoint_Base
{
    public GameObject target;
    public Transform transform;
    public Image image;
    public Text text;
    public GameObject wayPoint_UI;
    public float height_OffSet;
    public float interact_Distance;

    public void Set_Target(GameObject get_Target)
    {
        target = get_Target;
    }

    public void Set_Transform(Transform get_Transform)
    {
        transform = get_Transform;
    }

    public float Get_Distance(Vector3 startPosition, Vector3 endPosition) => Vector3.Distance(startPosition, endPosition);

    public void Enable_WayPoint(bool value)
    {
        image.enabled = value;
        text.enabled = value;
    }
}
