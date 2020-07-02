using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour {
    public static UI ui;

    [SerializeField]
    GameObject healthIcon;

    [SerializeField]
    GameObject keyIcon;

    [HideInInspector]
    public GameObject[] healthIcons;
    [HideInInspector]
    List<GameObject> keyIcons = new List<GameObject>();


    const float healthScale = 120f;
    const float keyScale = 40f;
    const float borderSize = 10f;
    const float gapSize = 30f;

    int keyCount = 0;

    private void Awake() {
        if (ui != null && ui != this) {
            Destroy(this.gameObject);
        }
        else {
            ui = this;
        }
    }
    // Start is called before the first frame update
    void Start() {

        healthIcons = new GameObject[GameManager.gameManager.fullHP];
        for (int i = 0; i < GameManager.gameManager.fullHP; i++) {
            healthIcons[i] = Instantiate(healthIcon, transform);
            RectTransform rectTrans = healthIcons[i].GetComponent<RectTransform>();
            rectTrans.localScale = new Vector3(0.58591549296f, 1f) * healthScale;
            rectTrans.anchoredPosition = new Vector2(healthScale / 2 * 0.58591549296f + borderSize, -(healthScale / 2 + borderSize)) + Vector2.right * (healthScale / 2 + gapSize) * i;
            rectTrans.sizeDelta = Vector2.one;
        }
    }

    // Update is called once per frame
    void Update() {

    }


    public void changeKeys(int amount) {
        keyCount = Mathf.Max(0, keyCount + amount);
        if (amount != 0) {
            foreach (GameObject keyIconIndex in keyIcons) {
                Destroy(keyIconIndex);
            }
            keyIcons.Clear();
            if (keyCount > 0) {
                for (int i = 0; i < keyCount; i++) {
                    keyIcons.Add(Instantiate(keyIcon, transform));
                    RectTransform rectTrans = keyIcons[keyIcons.Count - 1].GetComponent<RectTransform>();
                    rectTrans.localScale = Vector2.one * keyScale;
                    rectTrans.anchoredPosition = new Vector2(healthScale / 2 * 0.58591549296f + borderSize, -(healthScale / 2 + borderSize)) + Vector2.right * (keyScale * 0.6f + gapSize) * (keyIcons.Count - 1) - Vector2.up * (healthScale / 2 + keyScale / 2 + gapSize);
                    rectTrans.sizeDelta = Vector2.one;
                }
            }
        }
    }
}
