using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikyBush : MonoBehaviour
{    
    MainCharacterControls mainCharacter;
    CompositeCollider2D collider;
    Collider2D playerCollider;

    private void Start() {
        collider = GetComponent<CompositeCollider2D>();
        mainCharacter = MainCharacterControls.mainCharacter;
        playerCollider = mainCharacter.GetComponent<Collider2D>();
    }

    private void OnCollisionStay2D(Collision2D collision) {
        Debug.Log("Hit");
        ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];
        collision.GetContacts(contacts);
        for (int i = 0; i < collision.contactCount; i++) {
            if (contacts[i].collider == playerCollider) {
                mainCharacter.WasHit(collider.bounds.ClosestPoint(mainCharacter.gameObject.transform.position));
            }
        }
    }

}
