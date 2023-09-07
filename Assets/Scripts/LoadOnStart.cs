using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadOnStart : MonoBehaviour
{
    void Start()
    {
        FindObjectOfType<SaveManager>().LoadAll();
    }
}
