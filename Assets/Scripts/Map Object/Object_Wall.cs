using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing.Tweening;


public class Object_Wall : Object_Base
{
    [Header("=== Wall Setting  (Movemsnet) ===")]
    [SerializeField] private float wallMoveSpeed;
    [SerializeField] private GameObject wallObject;
    [SerializeField] private Transform[] wallMovePos;


    [Header("=== Wall Setting (Fade) ===")]
    [SerializeField] private ForceFieldController controller;
    [SerializeField] private BoxCollider wallcollider;
    [SerializeField] private float fadeSpeed;

    public override void Use()
    {
        //StartCoroutine(WallMove());
        StartCoroutine(WallFade());
    }

    private IEnumerator WallMove()
    {
        // 플레이어 이동 제한?

        // 문 열림 -> 위 아래 이동
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * wallMoveSpeed;
            wallObject.transform.position = Vector3.Lerp(wallMovePos[0].position, wallMovePos[1].position, EasingFunctions.OutExpo(timer));
            yield return null;
        }
        wallObject.transform.position = wallMovePos[1].position;

        // 플레이어 이동 제한 해제?
    }

    private IEnumerator WallFade()
    {
        wallcollider.enabled = false;
        controller.openAutoAnimation = false;
        float start = controller.openCloseProgress;
        float end = -2;
        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * fadeSpeed;
            controller.openCloseProgress = Mathf.Lerp(start, end, EasingFunctions.OutExpo(timer));
            yield return null;
        }
    }
}
