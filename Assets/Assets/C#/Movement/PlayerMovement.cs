using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class PlayerMovement : MonoBehaviour {

    public float jumpHeight = 4;
    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    public float moveSpeed = 6;

    float gravity;
    float jumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;
    bool jump;
    Controller2D controller;

    void Start() {
        controller = GetComponent<Controller2D>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        //print ("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);
    }
    private void Update() {
        jump |= Input.GetKeyDown("space") && controller.IsGrounded();
    }

    void FixedUpdate() {

        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (jump) {
            velocity.y = jumpVelocity;
        }
        jump = false;

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        if (controller._velocity.x > 0) {
            transform.rotation = Quaternion.Euler(Vector3.up * 0f);
        }else if(controller._velocity.x < 0) {
            transform.rotation = Quaternion.Euler(Vector3.up * -180f);
        }
    }

    internal void Push(Vector3 monsterPos) {

    }
}
