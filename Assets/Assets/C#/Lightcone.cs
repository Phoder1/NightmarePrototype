using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightcone : MonoBehaviour {

    [SerializeField]
    SpriteMask coneMask;
    [SerializeField]
    Transform flashlightTransform;
    SpriteRenderer coneSprite;
    [SerializeField]
    LayerMask terrainLayer;
    RaycastHit2D rayHit;
    // Start is called before the first frame update
    void Start() {
        coneSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
        if (rayHit = Physics2D.Raycast(flashlightTransform.position, transform.right, Mathf.Infinity, terrainLayer)) {
            Debug.Log("Found Terrain");
            coneMask.alphaCutoff = coneMask.bounds.size.x / rayHit.distance;
        }
        else {
            coneMask.alphaCutoff = 0f;
        }
    }

    private void OnDrawGizmos() {
        if(rayHit) {
            
        }
        Gizmos.DrawLine(flashlightTransform.position, transform.right * transform.localScale.x);
    }
}
