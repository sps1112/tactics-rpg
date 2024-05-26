using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Loads the given scene from index
    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    // Quits games
    public void QuitGame()
    {
        Application.Quit();
    }
}
