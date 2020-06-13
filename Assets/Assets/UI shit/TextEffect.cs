using UnityEngine;
using UnityEngine.UI;

public class TextEffect : MonoBehaviour {

    private float nextActionTime = 1f;
    private float blinkTimeStart;
    [SerializeField]
    private float effectSpeed = 6f;
    [SerializeField]
    private float period = 2.5f;
    private int IsBlink = 0;

    float effectTime;
    Material material;


    private void Start() {
        material = gameObject.GetComponent<Image>().material;
        material.SetFloat("EffectTime", 0.5f);
    }

    void Update() {
        if (Time.time % effectSpeed <= effectSpeed / 2) {
            effectTime = Mathf.Clamp(effectTime + (Time.deltaTime / (effectSpeed / 2)), 0f, 1f);
        }
        else {
            effectTime = Mathf.Clamp(effectTime - (Time.deltaTime / (effectSpeed / 2)), 0f, 1f);
        }
        material.SetFloat("EffectTime", effectTime);
    }
}
