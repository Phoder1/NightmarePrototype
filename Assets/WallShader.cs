using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallShader : MonoBehaviour
{
    [SerializeField]
    float timeToLight = 2f;
    Material material;
    Collider2D lightMaskCollider;
    bool isBeingLit = false;
    float EffectTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        lightMaskCollider = GameObject.FindGameObjectWithTag("LightconeMask").GetComponent<Collider2D>();
        material = GetComponentInChildren<SpriteRenderer>().material;
        material.SetFloat("EffectTime", EffectTime);
    }

    // Update is called once per frame
    void Update() {
        
        if (isBeingLit) {
            Debug.Log("beingLit");
            EffectTime = Mathf.Min(EffectTime + Time.deltaTime / timeToLight,1f);
            material.SetFloat("EffectTime", EffectTime);
        }
        if(EffectTime == 1f) {
            gameObject.SetActive(false);
        }
    }
    private void OnCollisionStay2D(Collision2D collision) {
        ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];
        collision.GetContacts(contacts);
        for (int i = 0; i < collision.contactCount; i++) {
            if (contacts[i].collider == lightMaskCollider) {
                isBeingLit = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        isBeingLit = false;
    }
}
