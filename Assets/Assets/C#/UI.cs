using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI ui;

    GameObject[] healthIcons;

    private void Awake() {
        if (ui != null && ui != this) {
            Destroy(this.gameObject);
        }
        else {
            ui = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {

        healthIcons = new GameObject[transform.childCount];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
