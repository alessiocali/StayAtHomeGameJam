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

    public void EnableCharacterSelection ()
    {
        GameObject.Find("Title_text").SetActive(false);
        GameObject.Find("Button_Stelect").SetActive(false);
        GameObject.Find("Button_Credits").SetActive(false);
        GameObject.Find("Button_Quit").SetActive(false);

        GameObject CanvasCharacterSelection = GameObject.Find("CharacterSelection");
        CanvasCharacterSelection.GetComponent<Canvas>().enabled = true;
    }
}
