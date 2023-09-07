using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [Tooltip("存档的文件名")]
    public string SaveName;

    //这里没有考虑多人玩家的情况
    private PlayerController pc;
    private Rigidbody2D pcrb;
    private Rope rope;
    private Rigidbody2D RopeRb;
    private UICanvas uiCanvas;
    void Start()
    {
        pc = FindObjectOfType<PlayerController>();
        pcrb = pc.GetComponent<Rigidbody2D>();
        rope = FindObjectsOfType<Rope>()[0];
        RopeRb = rope.GetComponent<Rigidbody2D>();
        uiCanvas = FindObjectOfType<UICanvas>();
    }

    public void SaveAll()
    {
        SaveData.instance.PlayerQua = pc.transform.rotation;
        SaveData.instance.PlayerPos = pc.transform.position;
        SaveData.instance.PlayerVel = uiCanvas.PlayerVel;
        SaveData.instance.RopePos = rope.transform.position;
        SaveData.instance.RopeVel = uiCanvas.RopeVel;
        SerializationManager.Save(SaveName, SaveData.instance);

        Debug.Log("Saved at " + SerializationManager.savePath + '\\' + SaveName);
    }

    public void LoadAll()
    {
        SaveData saveData = SerializationManager.Load(SaveName);

        pc = FindObjectOfType<PlayerController>();
        pcrb = pc.GetComponent<Rigidbody2D>();
        rope = FindObjectsOfType<Rope>()[0];
        RopeRb = rope.GetComponent<Rigidbody2D>();

        pc.transform.rotation = saveData.PlayerQua;
        pc.transform.position = saveData.PlayerPos;
        pcrb.velocity = saveData.PlayerVel;
        rope.transform.position = saveData.RopePos;
        RopeRb.velocity = saveData.RopeVel;

        Debug.Log("Load Done");
    }

    public void UnPauseByReload(UICanvas uICanvas)
    {
        uICanvas.GetComponent<Canvas>().enabled = false;
        pc.enabled = true;
        rope.enabled = true;
        pcrb.constraints = RigidbodyConstraints2D.None;
        RopeRb.constraints = RigidbodyConstraints2D.None;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
