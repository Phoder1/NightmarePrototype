using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager gameManager;


    private float volume = 1f;
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
        Debug.Log(volume);
    }


    public void ChangeVolume(float _volume) {
        volume = _volume;
    }
}
