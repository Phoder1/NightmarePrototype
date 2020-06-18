using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuEffect : MonoBehaviour
{

    CanvasRenderer Renderer;

    private float nextActionTime = 1f;
    private float blinkTimeStart;
    [SerializeField]
    private float Duration = 6f;
    [SerializeField]
    private float period = 2.5f;
    private int IsBlink = 0;
    [SerializeField]
    int BlinksCount = 2;
    Color color1;
    Color color2;
    bool blinkToggle = false;
    float animationTime;
    Material material;
    

    private void Start()
    {
        Renderer = GetComponent<CanvasRenderer>();
        color1 = new Color(1f, 1f, 1f, 0f);
        color2 = new Color(1f, 1f, 1f, 1f);
        material = gameObject.GetComponent<Image>().material;
        material.SetFloat("Respawn", 1f);
        material.SetFloat("DissolveIntensity", 0f);
    }

    void Update()
    {
        if (IsBlink == 0)
        {
            if (Time.time > nextActionTime)
            {
                IsBlink++;
                blinkTimeStart = Time.time + Random.Range(0f, 1f); ;
                
                //Renderer.SetColor(color1);
            }
        }
        else
        {
            Debug.Log(Time.time - blinkTimeStart + Duration <= Duration / 2);
            if (Time.time - blinkTimeStart <= Duration / 2) {
                animationTime = Mathf.Clamp(animationTime + (Time.deltaTime / (Duration / 2)), 0f, 1f);
            }
            else {
                animationTime = Mathf.Clamp(animationTime - (Time.deltaTime / (Duration / 2)), 0f, 1f);
            }
            material.SetFloat("DissolveIntensity", animationTime);
            if (Time.time>=blinkTimeStart+Duration)
            {
                IsBlink = 0;
                nextActionTime = Time.time + period + Random.Range(0f,1.5f);
                material.SetFloat("DissolveIntensity", 0f);


                //Renderer.SetColor(color2);
            }
        }
    }
}
