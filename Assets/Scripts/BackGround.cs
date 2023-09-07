using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    public PlayerController pc1;

    private RectTransform rectTransform;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x,
                                                      Mathf.Lerp(479, -14, Mathf.Clamp01((pc1.transform.position.y + 5) / 135)));
    }
}
