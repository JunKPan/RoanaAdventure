using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [Tooltip("发射速度")]
    public float LaunchVel;
    [Tooltip("绳索最大长度")]
    public float MaxLength;
    [Tooltip("绳索收回速度")]
    public float TakeBackSpeed;
    [Tooltip("判断绳索收回的，绳末与玩家的距离")]
    public float TakeBackDis;
    [Tooltip("在空中，每丢一次绳子就减少一点绳头的质量")]//重置绳头质量的部分代码在PlayerController.cs。//如果玩家的绳头吸附到了物体，也重置其质量
    public float DeltaMass;
    [Tooltip("绳头的最小质量")]
    public float MinMass;
    [Tooltip("判断玩家自动飞升的高度阈值")]
    public float DeltaHeight;

    private PlayerController pc;
    private LineRenderer lineRenderer;
    private SpringJoint2D joint;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private bool isUnused = true;
    void Start()
    {
        pc = GetComponentInParent<PlayerController>();
        lineRenderer = GetComponent<LineRenderer>();
        joint = GetComponent<SpringJoint2D>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        joint.distance = MaxLength;
    }

    void Update()
    {
        if (isUnused)
        {
            Unused();
            Launch();
        }
        else if (!isUnused)
        {
            Launching();
        }
    }

    //--绳子处于闲置状态时
    void Unused()
    {
        //让joint黏在玩家身上
        joint.transform.position = pc.transform.position;
        //不显示绳子
        lineRenderer.enabled = false;
        spriteRenderer.enabled = false;
    }

    //--绳子正在被发射出去
    //设计倒计时，是为了不要刚发射绳子的一瞬间就判定绳子已被收回
    private float ResetTime = 0.1f;
    private bool haveAttached = false;//绳头是否吸附到 过 物体
    private float PlayerTakeBackHeight;
    void Launching()
    {
        if (ResetTime > 0)
        {
            ResetTime -= Time.deltaTime;
        }
        else if (ResetTime <= 0)
        {
            if (Vector3.Distance(joint.transform.position, pc.transform.position) < TakeBackDis && !Input.GetKey(KeyCode.Mouse0))
            {
                isUnused = true;
                ResetTime = 0.1f;

                //记录玩家收回绳子时，玩家的高度
                PlayerTakeBackHeight = pc.transform.position.y;
                //在收回绳子的时候检测玩家是否在自动飞升
                ReduceRopeMass();

                //这个布尔值的重置一定要放在ReduceRopeMass()的后面
                haveAttached = false;
            }
        }

        //以下是不希望受到倒计时限制的内容      
        if (!Input.GetKey(KeyCode.Mouse0))
        {
            rb.constraints = RigidbodyConstraints2D.None;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        if (Input.GetKey(KeyCode.Mouse1))
        {
            joint.distance -= Time.deltaTime * TakeBackSpeed;
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("ground"), LayerMask.NameToLayer("rope"), true);
        }
        else
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("ground"), LayerMask.NameToLayer("rope"), false);
            if (joint.distance <= MaxLength)
            {
                joint.distance = Vector2.Distance(transform.position, pc.transform.position);
            }
            else if (joint.distance > MaxLength)
            {
                joint.distance = MaxLength;
            }
        }

        //让线渲染器的端点始终附着在玩家和绳末上
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, pc.transform.position);
    }

    //--当碰到墙壁时，就吸附上去
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (ResetTime <= 0)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("ground") && Input.GetKey(KeyCode.Mouse0) && !isUnused)
            {
                rb.constraints = RigidbodyConstraints2D.FreezePosition;
                haveAttached = true;
                //如果玩家的绳头吸附到了物体，也重置其质量
                rb.mass = 1;
            }
        }
    }

    private float PlayerLaunchHeight;
    void Launch()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            isUnused = false;

            Vector2 LaunchDir = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position).normalized;
            rb.velocity = LaunchDir * LaunchVel;

            spriteRenderer.enabled = true;

            //记录玩家丢出绳子的高度
            PlayerLaunchHeight = pc.transform.position.y;
        }
    }

    void ReduceRopeMass()
    {
        //只有玩家上升了一定的高度，并且绳子没有吸附到过物体，才会尝试减少质量
        if (PlayerTakeBackHeight - PlayerLaunchHeight > DeltaHeight && !haveAttached)
        {
            //减少绳头质量
            if (rb.mass >= MinMass)
            {
                rb.mass -= DeltaMass;
                rb.mass = Mathf.Clamp(rb.mass, MinMass, 1);
            }
        }
    }
}
