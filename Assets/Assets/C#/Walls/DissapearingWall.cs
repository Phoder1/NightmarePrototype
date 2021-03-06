﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class DissapearingWall : MonoBehaviour {
    [SerializeField]
    Renderer wallRenderer;
    [SerializeField]
    string shaderPropertyName = "EffectTime";
    [SerializeField]
    float timeToLight = 2f;
    [SerializeField]
    [Range(0f, 1f)]
    float animationDisappearTimeRatio = 1f;

    Material material;
    Collider2D lightMaskCollider;
    bool isBeingLit = false;
    float EffectTime = 0f;
    // Start is called before the first frame update
    void Start() {
        lightMaskCollider = GameObject.FindGameObjectWithTag("LightconeMask").GetComponent<Collider2D>();
        material = wallRenderer.material;
        material.SetFloat(shaderPropertyName, EffectTime);
    }

    // Update is called once per frame
    void Update() {
        //Debug.Log(isBeingLit);
        if (isBeingLit) {
            Debug.Log("beingLit");
            EffectTime = Mathf.Min(EffectTime + Time.deltaTime / (timeToLight/animationDisappearTimeRatio), 1f);
            material.SetFloat(shaderPropertyName, EffectTime);
        }
        if (EffectTime >= animationDisappearTimeRatio) {
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
