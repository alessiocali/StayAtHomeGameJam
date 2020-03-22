using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause_Menu : MonoBehaviour {
    
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject HUD;
    public GameObject CharacterSelection;

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(GameIsPaused) {
                Resume();
            } else {
                Pause();
            }
        }
    }

    public void Resume() {
        pauseMenuUI.SetActive(false);
        GameIsPaused = false;
        HUD.SetActive(true);
    }

    void Pause() {
        pauseMenuUI.SetActive(true);
        GameIsPaused = true;
        HUD.SetActive(false);
    }

    public void quitGame() {
        Application.Quit();
    }

    public void ShowCharacterSelection ()
    {
        CharacterSelection.GetComponent<Canvas>().enabled = true;
        GameObject.Find("WIN").GetComponent<Canvas>().enabled = false;
        GameObject.Find("LOSE").GetComponent<Canvas>().enabled = false;
    }

    public void Reload()
    {

        SceneManager.LoadScene("MainScene");
    }
}
