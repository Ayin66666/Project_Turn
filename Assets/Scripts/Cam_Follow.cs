using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam_Follow : MonoBehaviour
{
    [SerializeField] private Transform followTarget;

    private void Update()
    {
        Follow();
    }

    private void Follow()
    {
        Vector3 pos = new Vector3(followTarget.position.x, transform.position.y, followTarget.position.z);
        transform.position = pos;
    }
}
