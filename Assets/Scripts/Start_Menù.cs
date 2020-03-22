using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start_Menù : MonoBehaviour
{
    public void PlayGame() {
        Debug.Log("START");
        SceneManager.LoadScene("MainScene");
    }

    public void QuitGame() {
        Debug.Log("QUIT");
        Application.Quit();
    }
}
