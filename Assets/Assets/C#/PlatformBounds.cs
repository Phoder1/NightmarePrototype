using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBounds : MonoBehaviour {
    Collider2D collider;
    // Start is called before the first frame update
    void Start() {
        collider = GetComponent<Collider2D>();
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
    }
}
