using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [Tooltip("�����ٶ�")]
    public float LaunchVel;
    [Tooltip("������󳤶�")]
    public float MaxLength;
    [Tooltip("�����ջ��ٶ�")]
    public float TakeBackSpeed;
    [Tooltip("�ж������ջصģ���ĩ����ҵľ���")]
    public float TakeBackDis;
    [Tooltip("�ڿ��У�ÿ��һ�����Ӿͼ���һ����ͷ������")]//������ͷ�����Ĳ��ִ�����PlayerController.cs��//�����ҵ���ͷ�����������壬Ҳ����������
    public float DeltaMass;
    [Tooltip("��ͷ����С����")]
    public float MinMass;
    [Tooltip("�ж�����Զ������ĸ߶���ֵ")]
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

    //--���Ӵ�������״̬ʱ
    void Unused()
    {
        //��joint����������
        joint.transform.position = pc.transform.position;
        //����ʾ����
        lineRenderer.enabled = false;
        spriteRenderer.enabled = false;
    }

    //--�������ڱ������ȥ
    //��Ƶ���ʱ����Ϊ�˲�Ҫ�շ������ӵ�һ˲����ж������ѱ��ջ�
    private float ResetTime = 0.1f;
    private bool haveAttached = false;//��ͷ�Ƿ������� �� ����
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

                //��¼����ջ�����ʱ����ҵĸ߶�
                PlayerTakeBackHeight = pc.transform.position.y;
                //���ջ����ӵ�ʱ��������Ƿ����Զ�����
                ReduceRopeMass();

                //�������ֵ������һ��Ҫ����ReduceRopeMass()�ĺ���
                haveAttached = false;
            }
        }

        //�����ǲ�ϣ���ܵ�����ʱ���Ƶ�����      
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

        //������Ⱦ���Ķ˵�ʼ�ո�������Һ���ĩ��
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, pc.transform.position);
    }

    //--������ǽ��ʱ����������ȥ
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (ResetTime <= 0)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("ground") && Input.GetKey(KeyCode.Mouse0) && !isUnused)
            {
                rb.constraints = RigidbodyConstraints2D.FreezePosition;
                haveAttached = true;
                //�����ҵ���ͷ�����������壬Ҳ����������
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

            //��¼��Ҷ������ӵĸ߶�
            PlayerLaunchHeight = pc.transform.position.y;
        }
    }

    void ReduceRopeMass()
    {
        //ֻ�����������һ���ĸ߶ȣ���������û�������������壬�Ż᳢�Լ�������
        if (PlayerTakeBackHeight - PlayerLaunchHeight > DeltaHeight && !haveAttached)
        {
            //������ͷ����
            if (rb.mass >= MinMass)
            {
                rb.mass -= DeltaMass;
                rb.mass = Mathf.Clamp(rb.mass, MinMass, 1);
            }
        }
    }
}
