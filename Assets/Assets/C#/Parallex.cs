using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallex : MonoBehaviour
{
    [SerializeField]
    float layerSpeed;
    [SerializeField]
    Transform cam;


    Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(startPos.x + cam.position.x * layerSpeed , startPos.y + cam.position.y * layerSpeed , transform.position.z);
    }
}
