using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    float maxlevelX = Mathf.Infinity;
    [SerializeField]
    float maxlevelY = Mathf.Infinity;
    Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = MainCharacterControls.mainCharacter.transform;
    }

    // Update is called once per frame
    void Update()
    {
        float posX = Mathf.Clamp(playerTransform.position.x, 0, maxlevelX);
        float posY = Mathf.Clamp(playerTransform.position.y, 0, maxlevelY);
        transform.position = new Vector3 ( posX, posY, transform.position.z);
    }
}
