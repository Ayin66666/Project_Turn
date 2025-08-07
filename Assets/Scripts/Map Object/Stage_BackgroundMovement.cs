using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing.Tweening;


public class Stage_BackgroundMovement : MonoBehaviour
{
    [Header("=== Movement Setting ===")]
    [SerializeField] private MoveType movement;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float delay;

    [SerializeField] private GameObject body;
    [SerializeField] private Vector3[] movePos = new Vector3[2];

    private enum MoveType { Up, Down }


    private void Start()
    {
        Setting();
        StartCoroutine(Movement());
    }


    private void Setting()
    {
        int ran = Random.Range(0, 2);
        movement = ran == 0 ? MoveType.Up : MoveType.Down;
        moveSpeed = Random.Range(0.05f, 0.15f);
        delay = Random.Range(0.25f, 1.6f);


        float ran2 = Random.Range(0.15f, 0.3f);
        movePos[0] = new Vector3(transform.position.x, transform.position.y + ran2, transform.position.z);
        movePos[1] = new Vector3(transform.position.x, transform.position.y - ran2, transform.position.z);
    }


    private IEnumerator Movement()
    {
        while(true)
        {
            // Movement
            Vector3 strtPos = movement == MoveType.Up ? movePos[1] : movePos[0];
            Vector3 endPos = movement == MoveType.Up ? movePos[0] : movePos[1];
            float timer = 0;
            while(timer < 1)
            {
                timer += Time.deltaTime;
                body.transform.position = Vector3.Lerp(strtPos, endPos, EasingFunctions.InOutCubic(timer));
                yield return null;
            }

            // MovePos Setting
            movement = movement == MoveType.Up ? MoveType.Down : MoveType.Up;

            // Delay
            yield return new WaitForSeconds(delay);
        }
    }
}
