using Easing.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_World : MonoBehaviour
{
    // 걷기 달리기 대쉬 3개만 구현
    // 점프는 제외

    [Header("=== Move Status ===")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private int dashCount;
    [SerializeField] private float dashPower;
    [SerializeField] private float dashCooldown;
    private float curDashCooldown;


    [Header("=== Move Setting ===")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool canDash;
    [SerializeField] private bool isDash;
    private float yDir;
    private Vector3 moveDir;
    private Vector3 dashDir;
    public Vector3 camDir;


    [Header("---Camera Status---")]
    [SerializeField] private float turnSmoothTime;
    private float turnSmoothVelocity;


    [Header("=== Component ===")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform cam;


    private void OnEnable()
    {
        controller = GetComponent<CharacterController>();
        canDash = true;
    }

    private void Update()
    {
        if (Player_Manager.instnace.canMove)
        {
            Movement();

            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            {
                StartCoroutine(Dash());
            }
        }
    }

    private void Movement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        moveDir = new Vector3(h, 0f, v).normalized;

        float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

        camDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        dashDir = camDir;
        if (moveDir.magnitude == 0)
        {
            dashDir = transform.forward;
        }

        if (isDash || !Player_Manager.instnace.canMove || Player_Manager.instnace.isDie)
        {
            return;
        }

        //플레이어 정면 조절
        if (moveDir.magnitude != 0)
        {
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            controller.Move(moveSpeed * Time.deltaTime * camDir.normalized);
        }
    }

    private IEnumerator Dash()
    {
        Player_Manager.instnace.canMove = true;
        isDash = true;

        float timer = 0.3f;
        float dashMultiply = 25f;
        float canRotateTime = 0.01f;

        dashPower = 0f;
        curDashCooldown = 0.5f;

        StartCoroutine(Cooldown());

        while (timer > 0)
        {
            while (canRotateTime > 0)
            {
                transform.rotation = Quaternion.LookRotation(dashDir);
                canRotateTime -= Time.deltaTime;
                yield return null;
            }
            dashDir = transform.forward;
            timer -= Time.deltaTime;
            dashPower += 3f * Time.deltaTime;
            controller.Move(EasingFunctions.OutExpo(dashPower) * dashMultiply * Time.deltaTime * dashDir.normalized);
            yield return null;
        }

        isDash = false;
    }

    private IEnumerator Cooldown()
    {
        canDash = false;

        while(curDashCooldown > 0)
        {
            curDashCooldown -= Time.deltaTime;

            yield return null;
        }

        canDash = true;
    }

    private void GourndCheck()
    {

    }
}
