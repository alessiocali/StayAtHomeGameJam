using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause_Menu : MonoBehaviour {
    
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject HUD;

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(GameIsPaused) {
                Resume();
                Debug.Log("ESC");
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
}
