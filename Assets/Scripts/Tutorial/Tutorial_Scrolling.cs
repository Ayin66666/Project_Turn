using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class TutoData
{
    public Sprite tutorial_Image;
    [TextArea] public string tutorial_Text;
}


public class Tutorial_Scrolling : MonoBehaviour
{
    [Header("=== Setting ===")]
    [SerializeField] private TutoData[] data;
    private int tutorial_Index = 1;
    [SerializeField] private Vector2Int minmax;

    [Header("=== Component ===")]
    [SerializeField] private Image image;
    [SerializeField] private Text text;
    [SerializeField] private Text count;


    public void Click_Next()
    {
        if(tutorial_Index < minmax.y)
        {
            tutorial_Index++;
            image.sprite = data[tutorial_Index-1].tutorial_Image;
            text.text = data[tutorial_Index-1].tutorial_Text;
            count.text = $"< {tutorial_Index} / {minmax.y} >";
        }
    }

    public void Click_Previous()
    {
        if(tutorial_Index > minmax.x)
        {
            tutorial_Index--;
            image.sprite = data[tutorial_Index-1].tutorial_Image;
            text.text = data[tutorial_Index-1].tutorial_Text;
            count.text = $"< {tutorial_Index} / {minmax.y} >";
        }
    }
}
