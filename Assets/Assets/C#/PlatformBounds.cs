using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBounds : MonoBehaviour {
    Collider2D collider;
    // Start is called before the first frame update
    void Start() {
        
    }

    private void OnDrawGizmos() {
        collider = GetComponent<Collider2D>();
        Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
    }
}
