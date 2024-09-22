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
    [SerializeField] private bool canJump;
    private float yDir;
    private Vector3 moveDir;


    [Header("=== Component ===")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private TrailRenderer trail;


    private void OnEnable()
    {
        canDash = true;
        canJump = true;
    }

    private void Update()
    {
        if (Player_Manager.instnace.canMove)
        {
            Movement();
            Jump();
            Dash();
        }

        Cooldown();
    }

    private void Movement()
    {
        // Key Input
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        moveDir = new Vector3(x, 0, z);

        // Movement
        if(moveDir.magnitude > 0.1f)
        {
            controller.Move(new Vector3(moveDir.x, yDir, moveDir.z).normalized * moveSpeed * Time.deltaTime);
        }

    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            canJump = false;
            yDir = 10f;
            curDashCooldown = dashCooldown;
        }
    }

    private void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {

        }
    }

    private void Cooldown()
    {
        if(curDashCooldown > 0)
        {
            curDashCooldown -= Time.deltaTime;
        }
    }

    private void GourndCheck()
    {

    }
}
