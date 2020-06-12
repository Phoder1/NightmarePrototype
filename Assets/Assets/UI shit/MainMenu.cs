using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    [SerializeField]
    GameObject background;
    Material BGmaterial;
    public float wait_Play;


    void Start()
    {
        BGmaterial = background.GetComponent<Image>().material;
        BGmaterial.SetFloat("DissolveIntensity", 0f);
    }
    public void PlayGame()
    {
        Invoke("InvokeGame", wait_Play);
    }

    public void InvokeGame()
    {
        BGmaterial.SetFloat("DissolveIntensity", 0f);
        SceneManager.LoadScene("LevelDesignPrototype");
    }

    public void QuitGame()
    {
        Debug.Log("QUIT works!!!");
        Application.Quit();
    }
}
