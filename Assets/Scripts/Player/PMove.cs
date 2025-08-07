using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing.Tweening;

public class PMove : MonoBehaviour
{
    [Header("--- Setting ---")]
    [SerializeField] private Movement movement;
    [SerializeField] private Vector2[] moveSpeed;
    [SerializeField] private float[] mSpeed;
    [SerializeField] private Material[] materials;
    private Coroutine speedCoroutine;
    private enum Movement { Left, Right };


    private void FixedUpdate()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetTextureOffset("_MainTex", (movement == Movement.Left ? 
                Vector2.left : Vector2.right) * mSpeed[i] * Time.time);
        }


        if (Input.GetKeyDown(KeyCode.L))
        {
            SpeedUp();
        }
    }

    public void SpeedUp()
    {
        if(speedCoroutine != null)
            StopCoroutine(speedCoroutine);

        speedCoroutine = StartCoroutine(SpeedUpCall());
    }

    private IEnumerator SpeedUpCall()
    {
        for (int i = 0; i < moveSpeed.Length; i++)
        {
            mSpeed[i] = moveSpeed[i].y;
        }

        float timer = 0;
        while(timer < 1)
        {
            timer += Time.deltaTime * 1.5f;
            for (int i = 0; i < mSpeed.Length; i++)
            {
                mSpeed[i] = Mathf.Lerp(moveSpeed[i].y, moveSpeed[i].x, EasingFunctions.OutExpo(timer));
            }
            yield return null;
        }

        for (int i = 0; i < mSpeed.Length; i++)
        {
            mSpeed[i] = moveSpeed[i].x;
        }
    }
}
