using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Menu : MonoBehaviour
{
    [SerializeField] private GameObject[] tutorial;


    /// <summary>
    /// ��ư Ŭ�� ȣ�� �Լ�
    /// </summary>
    /// <param name="index">Ȱ��ȭ �� Ʃ�丮�� �ε���</param>
    public void Button_Click(int index)
    {
        for (int i = 0; i < tutorial.Length; i++)
        {
            tutorial[i].SetActive(false);
        }

        tutorial[index].SetActive(true);
    }
}
