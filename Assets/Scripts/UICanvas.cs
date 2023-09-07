using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UICanvas : MonoBehaviour
{
    [Tooltip("�װ�")]
    public GameObject Panel;
    [Tooltip("����ѡ���Ӧ������")]
    public GameObject[] Selections;
    [System.NonSerialized]
    //������ͣ��ʱ�򣬼�¼��ҵ��ٶ�(�ṩ��SaveManager��
    public Vector2 PlayerVel;
    [System.NonSerialized]
    //���������
    public Vector2 RopeVel;
    [Tooltip("���÷ֱ��ʵ������˵�")]
    public TMP_Dropdown ResolutionDropDown;
    public Toggle FullScreenToggle;

    private Canvas canvas;
    private Animator animator;
    private PlayerController[] pcs;
    private Rope[] ropes;
    private Vector2[] PlayerVelBuffers;
    private float[] PlayerAngVelBuffers;
    private Vector2[] RopeVelBuffers;
    private bool isFullScreen = true;
    void Start()
    {
        canvas = GetComponent<Canvas>();
        animator = Panel.GetComponent<Animator>();
        pcs = (PlayerController[])FindObjectsOfType(typeof(PlayerController));
        ropes = (Rope[])FindObjectsOfType(typeof(Rope));
        //�����������ʼ��
        PlayerVelBuffers = new Vector2[pcs.Length];
        PlayerAngVelBuffers = new float[pcs.Length];
        RopeVelBuffers = new Vector2[ropes.Length];

        //������Ϸʱ�ȹر���ͣ����
        canvas.enabled = false;
    }


    void Update()
    {
        CallCanvas();
    }

    void CallCanvas()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (canvas.enabled == true)
            {
                UnPause();
            }
            else if (canvas.enabled == false)
            {
                Pause();
            }
        }
    }

    void Pause()
    {
        canvas.enabled = true;
        animator.Play("ComeIn", 0);

        for (int i = 0; i < pcs.Length; i++)
        {
            Rigidbody2D PlayerRb = pcs[i].GetComponent<Rigidbody2D>();
            Rigidbody2D RopeRb = ropes[i].GetComponent<Rigidbody2D>();
            PlayerVelBuffers[i] = PlayerRb.velocity;
            PlayerAngVelBuffers[i] = PlayerRb.angularVelocity;
            RopeVelBuffers[i] = RopeRb.velocity;

            //��¼����ٶȺ���ͷ��δ���Ƕ���ģʽ
            PlayerVel = pcs[0].GetComponent<Rigidbody2D>().velocity;
            RopeVel = ropes[0].GetComponent<Rigidbody2D>().velocity;

            PlayerRb.constraints = RigidbodyConstraints2D.FreezeAll;
            RopeRb.constraints = RigidbodyConstraints2D.FreezeAll;

            //�����������
            pcs[i].enabled = false;
            ropes[i].enabled = false;
        }
    }

    void UnPause()
    {
        canvas.enabled = false;

        for (int i = 0; i < pcs.Length; i++)
        {
            Rigidbody2D PlayerRb = pcs[i].GetComponent<Rigidbody2D>();
            Rigidbody2D RopeRb = ropes[i].GetComponent<Rigidbody2D>();
            PlayerRb.velocity = PlayerVelBuffers[i];
            //PlayerRb.angularVelocity = PlayerAngVelBuffers[i]; Debug.Log(PlayerRb.angularVelocity + "|||" + PlayerAngVelBuffers[i]);
            var impulse = (PlayerAngVelBuffers[i] * Mathf.Deg2Rad) * PlayerRb.inertia;
            PlayerRb.AddTorque(impulse, ForceMode2D.Impulse);

            RopeRb.velocity = RopeVelBuffers[i];

            PlayerRb.constraints = RigidbodyConstraints2D.None;
            RopeRb.constraints = RigidbodyConstraints2D.None;

            //�ָ��������
            pcs[i].enabled = true;
            ropes[i].enabled = true;
        }
    }

    //--ѡ���ʵ��
    public void Select(int Selected)
    {
        for (int i = 0; i < Selections.Length; i++)
        {
            if (i == Selected)
            {
                Selections[i].SetActive(true);
            }
            else if (i != Selected)
            {
                Selections[i].SetActive(false);
            }
        }
    }

    int mode = 0;
    public void SetResolution()
    {
        mode = ResolutionDropDown.value;
        switch (mode)
        {
            case 0:
                Screen.SetResolution(1980, 1080, isFullScreen);
                Debug.Log("�ֱ��� " + mode);
                break;
            case 1:
                Screen.SetResolution(1600, 900, isFullScreen);
                Debug.Log("�ֱ��� " + mode);
                break;
            case 2:
                Screen.SetResolution(1280, 720, isFullScreen);
                Debug.Log("�ֱ��� " + mode);
                break;
            default:
                Screen.SetResolution(1280, 720, isFullScreen);
                Debug.Log("�ֱ���Ĭ��");
                break;
        }
    }

    public void SetFullScreen()
    {
        isFullScreen = FullScreenToggle.isOn;
        Screen.fullScreen = isFullScreen;
        Debug.Log("FullScreen: " + isFullScreen);
    }
}
