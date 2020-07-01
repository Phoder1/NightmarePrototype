using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikyBush : MonoBehaviour
{
    [SerializeField]
    float hitRange = 1f;


    Transform playerTransform;
    MainCharacterControls mainCharacter;
    // Start is called before the first frame update
    void Start()
    {
        mainCharacter = MainCharacterControls.mainCharacter;
        playerTransform = MainCharacterControls.mainCharacter.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(Vector3.Scale(transform.position,new Vector3(1f,1f,0f)), playerTransform.position) <= hitRange) {
            mainCharacter.WasHit(gameObject);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitRange);
    }
}
