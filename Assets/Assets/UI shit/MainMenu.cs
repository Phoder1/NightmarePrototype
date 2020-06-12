using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public float wait_Play;

    public void PlayGame()
    {
        Invoke("InvokeGame", wait_Play);
    }

    public void InvokeGame()
    {
        SceneManager.LoadScene("LevelDesignPrototype");
    }

    public void QuitGame()
    {
        Debug.Log("QUIT works!!!");
        Application.Quit();
    }
}
