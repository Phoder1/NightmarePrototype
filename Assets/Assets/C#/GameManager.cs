using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager gameManager;


    private float volume = 1f;
    private int HP = 3;
    private void Awake() {
        if (gameManager != null && gameManager != this) {
            Destroy(this.gameObject);
        }
        else {
            gameManager = this;
        }

    }
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        
    }

    public void ResetScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }
    public void ChangeVolume(float _volume) {
        volume = _volume;
    }
    public void LoseLife() {
        HP--;
        UI.ui.healthIcons[HP].SetActive(false);
        if(HP <= 0) {
            MainCharacterControls.mainCharacter.Dead();
            Debug.Log("DEAD");
        }
    }
}
