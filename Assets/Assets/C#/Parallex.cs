using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallex : MonoBehaviour
{
    [SerializeField]
    float layerSpeed;
    
    Transform cam;

        
    Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector3(startPos.x + cam.position.x * layerSpeed , startPos.y + cam.position.y * layerSpeed , transform.position.z);
    }
}
