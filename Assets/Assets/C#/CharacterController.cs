using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    Transform ShoulderTransform;

    [SerializeField]
    float MouseSensetivity;

    [SerializeField]
    int  ShoulderMaxDegree;
    [SerializeField]
    int ShoulderMinDegree;

    [SerializeField]
    float MoveVelocity;

    float TempVelocity;
    float Angle;
    bool flipped = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {

        Angle += Input.GetAxisRaw("Mouse Y")*MouseSensetivity*Time.deltaTime;
        Angle = Mathf.Clamp(Angle, ShoulderMinDegree, ShoulderMaxDegree);
        
        TempVelocity = MoveVelocity * Input.GetAxis("Horizontal") * Time.deltaTime;
        if (TempVelocity < 0) {
            flipped = true;
            transform.localRotation = Quaternion.Euler(transform.localRotation.x, 180, transform.localRotation.z);
        }else if (TempVelocity > 0) {
            flipped = false;
            transform.localRotation = Quaternion.Euler(transform.localRotation.x, 0, transform.localRotation.z);
        }
        transform.position += Vector3.right * TempVelocity;
        ShoulderTransform.localRotation = Quaternion.Euler(ShoulderTransform.localRotation.eulerAngles.x, ShoulderTransform.localRotation.eulerAngles.y, Angle);


    }
}
