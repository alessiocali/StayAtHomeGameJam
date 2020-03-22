using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start_Menù : MonoBehaviour
{
    public void PlayGame() {
        //SceneManager.LoadScene("Game_Manager");
        Debug.Log("START");
    }

    public void QuitGame() {
        Debug.Log("QUIT");
        Application.Quit();
    }
}
