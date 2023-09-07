using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UICanvas : MonoBehaviour
{
    [Tooltip("底板")]
    public GameObject Panel;
    [Tooltip("所有选项卡对应的内容")]
    public GameObject[] Selections;
    [System.NonSerialized]
    //按下暂停的时候，记录玩家的速度(提供给SaveManager）
    public Vector2 PlayerVel;
    [System.NonSerialized]
    //类似上面的
    public Vector2 RopeVel;
    [Tooltip("设置分辨率的下拉菜单")]
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
        //给缓冲数组初始化
        PlayerVelBuffers = new Vector2[pcs.Length];
        PlayerAngVelBuffers = new float[pcs.Length];
        RopeVelBuffers = new Vector2[ropes.Length];

        //进入游戏时先关闭暂停界面
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

            //记录玩家速度和绳头。未考虑多人模式
            PlayerVel = pcs[0].GetComponent<Rigidbody2D>().velocity;
            RopeVel = ropes[0].GetComponent<Rigidbody2D>().velocity;

            PlayerRb.constraints = RigidbodyConstraints2D.FreezeAll;
            RopeRb.constraints = RigidbodyConstraints2D.FreezeAll;

            //禁用玩家输入
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

            //恢复玩家输入
            pcs[i].enabled = true;
            ropes[i].enabled = true;
        }
    }

    //--选项卡的实现
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
                Debug.Log("分辨率 " + mode);
                break;
            case 1:
                Screen.SetResolution(1600, 900, isFullScreen);
                Debug.Log("分辨率 " + mode);
                break;
            case 2:
                Screen.SetResolution(1280, 720, isFullScreen);
                Debug.Log("分辨率 " + mode);
                break;
            default:
                Screen.SetResolution(1280, 720, isFullScreen);
                Debug.Log("分辨率默认");
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
