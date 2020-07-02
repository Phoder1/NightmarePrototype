using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    [SerializeField]
    int fullHP = 3;
    [SerializeField]
    string shaderPropertyName = "DissolveIntensity";

    float effectTime = 0f;

    Renderer wallRenderer;
    Material material;
    int hp;

    float hitTime = 0f;
    const float hitDelay = 0.5f;
    bool isStillBeingHit = false;
    bool lostHP = true;
    // Start is called before the first frame update
    void Start()
    {
        wallRenderer = GetComponent<Renderer>();
        material = wallRenderer.material;
        material.SetFloat(shaderPropertyName, effectTime);
        hp = fullHP;
    }

    // Update is called once per frame
    void Update()
    {
        if (!lostHP) {
            lostHP = true;
            DealDamage();
        }
    }

    private void DealDamage() {
        
        hp--;
        effectTime = Mathf.Clamp(effectTime + Mathf.Ceil((1f / (fullHP+1.5f)) / 0.1f)*0.1f,0f,1f);
        Debug.Log(Mathf.Ceil((1f / fullHP) / 0.1f) * 0.1f + " , " + effectTime);
        if(hp == 0) {
            Destroy(gameObject);
        }
        else {
            material.SetFloat(shaderPropertyName, effectTime);
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];
        collision.GetContacts(contacts);
        for (int i = 0; i < collision.contactCount; i++) {
            if (contacts[i].collider.tag == "Spoon") {
                if(!isStillBeingHit && Time.timeSinceLevelLoad >= hitTime + hitDelay) {
                    lostHP = false;
                    hitTime = Time.timeSinceLevelLoad;
                }
                
                isStillBeingHit = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        lostHP = true;
        isStillBeingHit = false;
    }
}
