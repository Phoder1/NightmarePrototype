using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraController : MonoBehaviour {
    [SerializeField]
    GameObject minPoint;
    [SerializeField]
    GameObject maxPoint;
    [SerializeField]
    float cameraSpeed;

    Transform playerTransform;

    Camera cam;
    Vector3 boundsMin;
    Vector3 boundsMax;

    Vector3 camExtents;
    Vector3 movement;
    // Start is called before the first frame update
    const float SCREEN_EDGE = 0.1f;
    void Start() {
        cam = GetComponent<Camera>();
        camExtents = cam.ScreenToWorldPoint(cam.pixelRect.max) - transform.position;

        
        if (minPoint.GetComponent<SpriteRenderer>() != null) {
            boundsMin = minPoint.GetComponent<SpriteRenderer>().bounds.min;
        }
        else if (minPoint.GetComponent<Collider2D>() != null) {
            boundsMin = minPoint.GetComponent<Collider2D>().bounds.min;
        }
        else {
            boundsMin = minPoint.transform.position;
        }

        if (maxPoint.GetComponent<SpriteRenderer>() != null) {
            boundsMax = maxPoint.GetComponent<SpriteRenderer>().bounds.max;
        }
        else if (maxPoint.GetComponent<Collider2D>() != null) {
            boundsMax = maxPoint.GetComponent<Collider2D>().bounds.max;
        }
        else {
            boundsMax = maxPoint.transform.position;
        }


        playerTransform = MainCharacterControls.mainCharacter.transform;

    }

    // Update is called once per frame
    void FixedUpdate() {
        float posX = Mathf.Clamp(playerTransform.position.x, boundsMin.x+camExtents.x+SCREEN_EDGE, boundsMax.x- camExtents.x-SCREEN_EDGE);
        float posY = Mathf.Clamp(playerTransform.position.y, boundsMin.y+ camExtents.y+SCREEN_EDGE, boundsMax.y- camExtents.y-SCREEN_EDGE);

        movement = (Vector3.MoveTowards(transform.position, new Vector3(posX, posY, transform.position.z), 1f) - transform.position) * cameraSpeed * Time.deltaTime;
        transform.position += movement;
    }
}
