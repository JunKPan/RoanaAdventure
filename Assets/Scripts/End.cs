using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class End : MonoBehaviour
{
    public GameObject WinPanel;
    public AudioSource Salvo1;
    public AudioSource Salvo2;

    AudioSource MyAudioSource;

    private bool haveEntered = false;

    private void Start()
    {
        MyAudioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null && !haveEntered)
        {
            haveEntered = true;

            StartCoroutine(EndShow());
        }
    }

    IEnumerator EndShow()
    {
        MyAudioSource.Play();
        yield return new WaitWhile(() => MyAudioSource.isPlaying);
        Salvo1.Play();
        yield return new WaitForSeconds(0.2f);
        Salvo2.Play();
        yield return new WaitForSeconds(0.8f);
        WinPanel.SetActive(true);
        WinPanel.GetComponent<Animator>().Play("Win", 0);
    }

    public void Reload()
    {
        SaveData saveData = new SaveData();
        saveData.PlayerQua = Quaternion.Euler(0, 0, 0);
        saveData.PlayerPos = new Vector3(0, 0, 0);
        saveData.PlayerVel = new Vector2(0, 0);
        saveData.RopePos = new Vector3(0, 0, 0);
        saveData.RopeVel = new Vector2(0, 0);
        SaveData.instance = saveData;
        SerializationManager.Save(FindObjectOfType<SaveManager>().SaveName, SaveData.instance);

        SceneManager.LoadScene(0);
    }
}
