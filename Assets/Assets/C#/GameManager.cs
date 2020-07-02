using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager gameManager;
    [SerializeField]
    public int fullHP = 3;

    private float volume = 1f;
    private int HP;

    private int numberOfKeys;
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
        HP = fullHP;
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void ResetScene() {
        HP = fullHP;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        

    }
    public void ChangeVolume(float _volume) {
        volume = _volume;
    }
    public void changeHealth(int amount) {
        HP = Mathf.Clamp( HP +amount,0,fullHP);
        if (amount != 0) {
            for (int i = 0; i < fullHP; i++) {
                UI.ui.healthIcons[i].SetActive(i < HP);
            }
            if (HP == 0) {
                MainCharacterControls.mainCharacter.Dead();
                Debug.Log("DEAD");
            }
        }
    }

    public void changeKeys(int amount) {
        numberOfKeys = Mathf.Max(0, numberOfKeys + amount);
        if (amount != 0) {
            UI.ui.changeKeys(amount);
        }
    }


}
