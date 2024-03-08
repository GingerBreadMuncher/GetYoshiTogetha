using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadGameScene() { SceneManager.LoadScene("Game"); }

    public void LoadMainMenu() { SceneManager.LoadScene("MainMenu"); }

    public void ExitGame() { Application.Quit(); }
}
