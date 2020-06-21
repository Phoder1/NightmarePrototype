using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementOld : MonoBehaviour {

    [SerializeField]
    CharacterController2D controller;

    [SerializeField]
    internal Transform PlayerPivot;
    [SerializeField]
    internal Collider2D PlayerCollider;
    [SerializeField]
    float moveSpeed;
    [SerializeField]
    float JumpForce = 0f;

    [SerializeField]
    float pushForce = 6f;
    [SerializeField]
    float pushStopForce = 9f;


    //Movement variables
    float velocityX = 0f;
    internal float velocityY = 0f;


    internal float pushSpeedX = 0f;
    bool jump;
    

    MainCharacterControls.PlayerStates playerState;

    private void Start() {
    }
    private void FixedUpdate() {
        Movement();
    }


    private void Movement() {

        if(jump = Input.GetKeyDown("space")) {

        }
        playerState = MainCharacterControls.mainCharacter.playerCurrentState;
        if (playerState != MainCharacterControls.PlayerStates.Hit && playerState != MainCharacterControls.PlayerStates.Dead) {
            velocityX = moveSpeed * Input.GetAxis("Horizontal");
        }
        else if (pushSpeedX != 0f) {
            velocityX = pushSpeedX;
            pushSpeedX = Mathf.Clamp(pushSpeedX - Mathf.Sign(pushSpeedX) * pushStopForce * Time.fixedDeltaTime, Mathf.Min(0f, pushSpeedX), Mathf.Max(0f, pushSpeedX));

        }

        controller.Move(velocityX * Time.fixedDeltaTime,false,jump);
        jump = false;

    }


    public void Push(Vector2 hitPoint) {
        pushSpeedX = Mathf.Sign(transform.position.x - hitPoint.x) * pushForce;
    }
}
