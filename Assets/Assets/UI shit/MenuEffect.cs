using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuEffect : MonoBehaviour
{

    CanvasRenderer Renderer;

    private float nextActionTime = 1f;
    private float blinkTimeStart;
    [SerializeField]
    private float Duration = 0.2f;
    [SerializeField]
    private float period = 2.5f;
    private int IsBlink = 0;
    [SerializeField]
    int BlinksCount = 2;
    Color color1;
    Color color2;

    private void Start()
    {
        Renderer = GetComponent<CanvasRenderer>();
        color1 = new Color(1f, 1f, 1f, 0f);
        color2 = new Color(1f, 1f, 1f, 1f);
    }

    void Update()
    {
        if (IsBlink == 0)
        {
            if (Time.time > nextActionTime)
            {

                IsBlink++;
                blinkTimeStart = Time.time;
                Renderer.SetColor(color1);
            }
        }
        else
        {
            if(Time.time>blinkTimeStart+Duration)
            {
                IsBlink++;
                if(IsBlink >= BlinksCount)
                {
                    IsBlink = 0;
                }
                nextActionTime = Time.time + period;
                Renderer.SetColor(color2);
            }
        }
    }
}
