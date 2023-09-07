using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Tooltip("奔跑力度")]
    public float speed;
    [Tooltip("奔跑最大速度")]
    public float MaxSpeed;
    [Tooltip("跳跃力度")]
    public float JumpForce;
    [Tooltip("检测玩家是否能够跳跃的overlapbox大小")]
    public Vector2 CanJumpBox;
    [Tooltip("检测玩家是否能够跳跃的overlapbox相对于中心的位置偏移")]
    public Vector2 JumpBoxOffset;
    [Tooltip("触发Wave动画的速度最小值")]
    public float WaveSpeed;

    [System.NonSerialized]
    public Rigidbody2D rb;

    private Rope rope;
    private Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rope = GetComponentInChildren<Rope>();
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        ResetRopeMass();
    }

    private void FixedUpdate()
    {
        Run();
        Jump();
    }

    //--奔跑
    void Run()
    {
        Vector2 RunDir = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        if (rb.velocity.magnitude < MaxSpeed)
        {
            rb.AddForce(RunDir * speed);
        }
    }

    void Jump()
    {
        Vector2 JumpBoxPos = new Vector2(transform.position.x + JumpBoxOffset.x, transform.position.y + JumpBoxOffset.y);
        if (Input.GetKey(KeyCode.W) && Physics2D.OverlapBox(JumpBoxPos, CanJumpBox, 0, LayerMask.GetMask("ground")))
        {
            rb.velocity += new Vector2(0, JumpForce);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Vector3 JumpBoxPos = new Vector3(transform.position.x + JumpBoxOffset.x, transform.position.y + JumpBoxOffset.y, 0);
        Vector3 CanJumpBox2 = new Vector3(CanJumpBox.x, CanJumpBox.y, 0);
        Gizmos.DrawWireCube(JumpBoxPos, CanJumpBox);
    }

    void ResetRopeMass()
    {
        Vector2 JumpBoxPos = new Vector2(transform.position.x + JumpBoxOffset.x, transform.position.y + JumpBoxOffset.y);
        if (Physics2D.OverlapBox(JumpBoxPos, CanJumpBox, 0, LayerMask.GetMask("ground")))
        {
            rope.GetComponent<Rigidbody2D>().mass = 1;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (rb.velocity.magnitude > WaveSpeed)
        {
            animator.Play("Wave", 0);
        }
    }
}
