using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDoor : MonoBehaviour
{
    [SerializeField]
    float transitionTime = 1.5f;
    [SerializeField]
    string shaderPropertyName = "DissolveIntensity";

    float effectTime = 0f;

    Collider2D playerCollider;
    Collider2D wallCollider;
    Renderer wallRenderer;
    Material material;

    bool open = false;
    // Start is called before the first frame update
    void Start()
    {
        playerCollider = MainCharacterControls.mainCharacter.gameObject.GetComponent<Collider2D>();
        wallCollider = GetComponent<Collider2D>();
        wallRenderer = GetComponent<Renderer>();
        material = wallRenderer.material;
        material.SetFloat(shaderPropertyName, effectTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (open) {
            Debug.Log("Transition");
            effectTime = Mathf.Clamp(effectTime + Time.deltaTime / transitionTime, 0f, 1f);
            material.SetFloat(shaderPropertyName, effectTime);
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        Debug.Log("Collide!");
        ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];
        collision.GetContacts(contacts);
        for (int i = 0; i < collision.contactCount; i++) {
            if (contacts[i].collider == playerCollider && GameManager.gameManager.numberOfKeys > 0 && !open) {
                Debug.Log("With Player!");
                Unlock();
            }
        }
    }

    private void Unlock() {
        open = true;
        GameManager.gameManager.changeKeys(-1);
        wallCollider.enabled = false;
    }
}
