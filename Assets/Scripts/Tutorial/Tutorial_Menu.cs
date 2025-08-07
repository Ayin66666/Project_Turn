using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Menu : MonoBehaviour
{
    [SerializeField] private GameObject[] tutorial;


    /// <summary>
    /// 버튼 클릭 호출 함수
    /// </summary>
    /// <param name="index">활성화 할 튜토리얼 인덱스</param>
    public void Button_Click(int index)
    {
        for (int i = 0; i < tutorial.Length; i++)
        {
            tutorial[i].SetActive(false);
        }

        tutorial[index].SetActive(true);
    }
}
