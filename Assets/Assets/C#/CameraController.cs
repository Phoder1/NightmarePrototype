using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Transform target;
    // Start is called before the first frame update
    void Start()
    {
        target = MainCharacterControls.mainCharacter.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3 ( target.position.x, transform.position.y, transform.position.z);
    }
}
