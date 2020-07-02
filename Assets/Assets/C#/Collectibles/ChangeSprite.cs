using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collectibles))]
[ExecuteInEditMode]
public class ChangeSprite : MonoBehaviour
{
    Collectibles collectibles;

    [SerializeField]
    internal Sprite batterySprite;
    [SerializeField]
    internal Sprite keySprite;
    [SerializeField]
    internal Sprite healthSprite;

    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        collectibles = GetComponent<Collectibles>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("working");
        switch (collectibles.collectibleType) {
            case Collectibles.CollectibleTypes.Battery:
                spriteRenderer.sprite = batterySprite;
                break;
            case Collectibles.CollectibleTypes.Key:
                spriteRenderer.sprite = keySprite;
                break;
            case Collectibles.CollectibleTypes.Health:
                spriteRenderer.sprite = healthSprite;
                break;
            default:
                break;
        }
    }
}
