using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    [SerializeField] private GameObject lookTarget;



    private void OnEnable()
    {
        if(lookTarget == null)
        {
            lookTarget = GameObject.Find("Main Camera");
        }
    }

    private void FixedUpdate()
    {
        Vector3 lookDir = (lookTarget.transform.position - transform.position).normalized;
        lookDir.y = 0;

        transform.rotation = Quaternion.LookRotation(-lookDir);
    }
}
