using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraController : MonoBehaviour
{

    [SerializeField]
    float maxlevelX = Mathf.Infinity;
    [SerializeField]
    float maxlevelY = Mathf.Infinity;
    [SerializeField]
    float cameraSpeed;
    Transform playerTransform;

    Vector3 movement;
    // Start is called before the first frame update
    void Start()
    {

        playerTransform = MainCharacterControls.mainCharacter.transform;

    }

    // Update is called once per frame
    void Update() {
        float posX = Mathf.Clamp(playerTransform.position.x, 0, maxlevelX);
        float posY = Mathf.Clamp(playerTransform.position.y, 0, maxlevelY);
        
        movement = (Vector3.MoveTowards(transform.position, new Vector3(posX, posY, transform.position.z), 1f) - transform.position) * cameraSpeed * Time.deltaTime;
        transform.position += movement;
    }
}
