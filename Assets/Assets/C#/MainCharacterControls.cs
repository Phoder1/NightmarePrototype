using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterControls : MonoBehaviour {
    [SerializeField]
    Transform ShoulderTransform;
    [SerializeField]
    Transform GroundcheckTransform;
    [SerializeField]
    float GroundDistance;
    [SerializeField]
    LayerMask GroundMask;



    CharacterController CharController;

    [SerializeField]
    float MouseSensetivity;

    [SerializeField]
    int ShoulderMaxDegree;
    [SerializeField]
    int ShoulderMinDegree;

    [SerializeField]
    float MoveVelocity;

    [SerializeField]
    float Gravity;
    [SerializeField]
    float JumpForce = 0f;

    float velocityY = 0f;
    float TempVelocity;
    float Angle;

    // Start is called before the first frame update
    void Start() {
        CharController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update() {

        Angle += Input.GetAxisRaw("Mouse Y") * MouseSensetivity * Time.deltaTime;
        Angle = Mathf.Clamp(Angle, ShoulderMinDegree, ShoulderMaxDegree);

        TempVelocity = MoveVelocity * Input.GetAxis("Horizontal");
        if (TempVelocity < 0) {
            transform.localRotation = Quaternion.Euler(transform.localRotation.x, 180, transform.localRotation.z);
        }
        else if (TempVelocity > 0) {
            transform.localRotation = Quaternion.Euler(transform.localRotation.x, 0, transform.localRotation.z);
        }
        //Physics.CheckSphere(GroundcheckTransform.position, GroundDistance, GroundMask)
        velocityY -= Gravity * Time.deltaTime;
        if (CharController.isGrounded) {
            if (Input.GetKeyUp("space")) {
                velocityY += JumpForce;
            }
            else {
                velocityY = -0.1f * Gravity;
            }
            Debug.Log("Grounded!, Velocity: "+ velocityY);
        }
        CharController.Move(Vector3.right * TempVelocity *Time.deltaTime);
        CharController.Move(Vector3.up * velocityY *Time.deltaTime);
        ShoulderTransform.localRotation = Quaternion.Euler(ShoulderTransform.localRotation.eulerAngles.x, ShoulderTransform.localRotation.eulerAngles.y, Angle);


    }
}
