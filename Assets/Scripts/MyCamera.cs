using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCamera : MonoBehaviour
{
    public PlayerController pc1;

    [Tooltip("基本相机size")]
    public float MinSize;
    [Tooltip("最大相机size")]
    public float MaxSize;
    [Tooltip("相机size的变化率")]
    public float ScaleK;
    [Tooltip("相机size的变化速度")]
    public float ScaleSpeed;

    private Rigidbody2D pcrb;

    private void Start()
    {
        pcrb = pc1.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Follow();
        Scale();
    }

    void Follow()
    {
        transform.position = new Vector3(pc1.transform.position.x, pc1.transform.position.y, transform.position.z);
    }

    void Scale()
    {
        float TargetSize = MinSize + Mathf.Pow(pcrb.velocity.magnitude * ScaleK, 3);
        Mathf.Clamp(TargetSize, MinSize, MaxSize);
        Camera.main.orthographicSize += Time.deltaTime * ScaleSpeed * Mathf.Sign(TargetSize - Camera.main.orthographicSize);
        if (Camera.main.orthographicSize >= MaxSize)
        {
            Camera.main.orthographicSize = MaxSize;
        }
    }
}
