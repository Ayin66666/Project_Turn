using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drag_Obj : MonoBehaviour // 드래그 할때만 쓰는 드래그 전용 오브젝트
{
    public static Drag_Obj instance;

    public Item_Slot drag_Slot;
    [SerializeField] private Image drag_Image;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void Set_Drag_Image(Image item_Image)
    {
        drag_Image.sprite = item_Image.sprite;
        Set_Color(0.6f);
    }

    public void Set_Color(float alpha)
    {
        Color color = drag_Image.color;
        color.a = alpha;
        drag_Image.color = color;
    }
}
