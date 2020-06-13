using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBounds : MonoBehaviour {
    Collider2D platformCollider;
    // Start is called before the first frame update
    void Start() {
        
    }

    private void OnDrawGizmos() {
        platformCollider = GetComponent<Collider2D>();
        Gizmos.DrawWireCube(platformCollider.bounds.center, platformCollider.bounds.size);
    }
}
