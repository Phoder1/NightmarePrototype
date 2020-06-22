using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpoonEffect : MonoBehaviour
{

    [SerializeField]
    float effectTime;

    SpriteRenderer spoonRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spoonRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        spoonRenderer.material.SetFloat("SpoonEffectTime", effectTime);
    }
}
