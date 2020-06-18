using UnityEngine;
using UnityEngine.UI;

public class TextEffect : MonoBehaviour {
    [SerializeField]
    private float effectSpeed = 6f;

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
