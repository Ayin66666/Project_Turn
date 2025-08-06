using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easing.Tweening;

public class Stage_Drone : MonoBehaviour
{
    [Header("=== Setting ===")]
    [SerializeField] private Type moveType;
    [SerializeField] private UpDown upDown;
    [SerializeField] private Rotate rotate;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveDelay;
    private enum Type { None, Movement, Rotate }
    private enum UpDown { Up, Down };
    private enum Rotate { Left, Right };

    [Header("=== Pos & Body ===")]
    [SerializeField] private GameObject body;
    [SerializeField] private List<Transform> movePos;
    [SerializeField] private List<Transform> updownPos;


    [Header("=== Rotate ===")]
    [SerializeField] private Transform[] rot;
    // [SerializeField] private Quaternion[] rot;


    [Header("=== Other ===")]
    private Coroutine moveCoroutine;
    private Coroutine upDownCoroutine;
    private Coroutine rotateCoroutine;
    private bool isLook;
    private int count = 0;


    private void Start()
    {
        Setting();
    }

    private void Setting()
    {
        upDownCoroutine = StartCoroutine(UpDownMovement());

        // ���� Ÿ�� üũ
        switch (moveType)
        {
            case Type.None:
                break;

            case Type.Movement:
                moveCoroutine = StartCoroutine(Movement());
                break;

            case Type.Rotate:
                moveCoroutine = StartCoroutine(RotateMovement());
                break;
        }
    }

    private IEnumerator Movement()
    {
        while(true)
        {
            for (int i = 0; i < movePos.Count; i++)
            {
                // ����
                Vector3 startPos = transform.position;
                Vector3 endPos = movePos[count < movePos.Count ? i : 0].position;

                // ȸ��
                if(rotateCoroutine != null)
                    StopCoroutine(rotateCoroutine);

                rotateCoroutine = StartCoroutine(LookAt(endPos));
                while (isLook)
                {
                    yield return null;
                }

                // �̵�
                float timer = 0;
                while(timer < 1)
                {
                    timer += Time.deltaTime * moveSpeed;
                    transform.position = Vector3.Lerp(startPos, endPos, EasingFunctions.InOutCubic(timer));
                    yield return null;
                }

                // ���
                yield return new WaitForSeconds(moveDelay);
            }
        }
    }

    private IEnumerator RotateMovement()
    {
        while(true)
        {
            // ����
            Quaternion start = transform.rotation;
            Vector3 rotDir = (rot[rotate == Rotate.Left ? 0 : 1].transform.position - transform.position);
            Quaternion end = Quaternion.LookRotation(rotDir);

            // ȸ��
            float timer = 0;
            while(timer < 1)
            {
                timer += Time.deltaTime * moveSpeed;
                transform.rotation = Quaternion.Lerp(start, end, EasingFunctions.InOutCubic(timer));
                yield return null;
            }

            // ����
            rotate = rotate == Rotate.Left ? Rotate.Right : Rotate.Left;

            // ���� ������
            yield return new WaitForSeconds(moveDelay);
        }
    }

    private IEnumerator LookAt(Vector3 lookPos)
    {
        isLook = true;

        Vector3 lookDir = (lookPos - transform.position).normalized;
        lookDir.y = 0;

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(lookDir);
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 0.65f;
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, EasingFunctions.InOutCubic(timer));
            yield return null;
        }

        isLook = false;
    }

    private IEnumerator UpDownMovement()
    {
        while(true)
        {
            // �̵� ����
            Vector3 startPos = upDown == UpDown.Up ? updownPos[0].position : updownPos[1].position;
            Vector2 endPos = upDown == UpDown.Up ? updownPos[1].position : updownPos[0].position;
            float timer = 0;
            
            // �̵�
            while (timer < 1)
            {
                timer += Time.deltaTime;
                body.transform.position = new Vector3(transform.position.x, Mathf.Lerp(startPos.y, endPos.y, timer), transform.position.z);
                yield return null;
            }

            upDown = upDown == UpDown.Up ? UpDown.Down : UpDown.Up;

            // ������
            yield return new WaitForSeconds(0.1f);
        }
    }
}
