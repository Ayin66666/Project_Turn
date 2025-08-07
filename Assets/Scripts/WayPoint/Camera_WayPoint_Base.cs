using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Camera_WayPoint_Base : MonoBehaviour
{
    [System.Serializable]
    public struct WayPoint_Data
    {
        public GameObject worldWayPoints;
        public List<WayPoint_Controller> wayPoints;
        [HideInInspector] public int screen_Width;
        [HideInInspector] public int screen_Height;
        [HideInInspector] public Camera cam;
    }

    [SerializeField] private GameObject player;

    public WayPoint_Data data;

    public void Start()
    {
        CamSet();
    }

    void CamSet()
    {
        data.cam = Camera.main;
        data.screen_Width = Screen.width;
        data.screen_Height = Screen.height;
    }

    public Vector2 UI_Image_Position(WayPoint_Base item)
    {
        float item_Image_Width = item.image.GetPixelAdjustedRect().width / 2;
        float item_Image_Height = item.image.GetPixelAdjustedRect().height / 2;

        Vector2 screenPosition = GetItemScreenPosition(item);
        screenPosition.x = ScreenClamp(screenPosition.x, item_Image_Width, data.screen_Width);
        screenPosition.y = ScreenClamp(screenPosition.y, item_Image_Height, data.screen_Height);


        return screenPosition;
    }

    public Vector3 UI_Image_Scale(WayPoint_Base item)
    {
        Vector3 screenScale = new Vector3(item.image.transform.localScale.x, item.image.transform.localScale.y, item.image.transform.localScale.z );

        return screenScale;
    }

    public float WayPointDistance(WayPoint_Base item)
    {
        return Mathf.Round(Vector3.Distance(item.transform.position, player.transform.position));
    }

    public float ScreenClamp(float ScreenPosition, float item_Image_Width, int screen_Width)
    {
        return Mathf.Clamp(ScreenPosition, item_Image_Width, screen_Width - item_Image_Width);
    }

    public Vector2 GetItemScreenPosition(WayPoint_Base item)
    {
        float x = item.transform.position.x;
        float y = item.transform.position.y + item.height_OffSet;
        float z = item.transform.position.z;
        Vector2 screenPosition = data.cam.WorldToScreenPoint(new Vector3(x, y, z));

        if(Vector3.Dot((item.transform.position - transform.position), transform.forward) < 0)
        {
            if(screenPosition.x < Screen.width / 2)
            {
                screenPosition.x = Screen.width - item.image.GetPixelAdjustedRect().width / 2;
            }
            else
            {
                screenPosition.x = item.image.GetPixelAdjustedRect().width / 2;
            }

            if(screenPosition.y < Screen.height / 2)
            {
                screenPosition.y = Screen.height - item.image.GetPixelAdjustedRect().height / 2;
            }
            else
            {
                screenPosition.y = item.image.GetPixelAdjustedRect().height / 2;
            }
        }
        return screenPosition;
    }
}
