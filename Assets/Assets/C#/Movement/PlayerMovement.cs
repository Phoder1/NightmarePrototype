using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class PlayerMovement : MonoBehaviour {
    [SerializeField]
    Animator animator;
    [SerializeField]
    float jumpHeight = 4;
    [SerializeField]
    float timeToJumpApex = 0.4f;
    [SerializeField]
    float rollDistance = 3;
    [SerializeField]
    float rollTime = 0.3f;
    [SerializeField]
    float rollCD = 1f;
    [SerializeField]
    float moveSpeed = 6;
    [SerializeField]
    float accelerationTimeAirborne = 0.2f;
    [SerializeField]
    float accelerationTimeGrounded = 0.1f;
    [SerializeField]
    FieldOfView FOV;

    enum MovementStates { Normal, Roll, Push, Dead };
    MovementStates currentMoveState = MovementStates.Normal;

    bool controlsEnabled = true;

    float gravity;
    float jumpVelocity;

    float rollBreakingAcceleration;
    float rollVelocity;
    float currentRollVelocity;
    float lastRollTime = 0f;

    bool jump = false;

    Vector3 velocity;
    float velocityXSmoothing;
    //Controls
    Vector2 input;

    bool _space;
    bool space {
        get {
            bool temp = _space;
            _space = false;
            return temp;
        }
        set {
            _space = value;
        }
    }
    bool _shift;
    bool shift {
        get {
            bool temp = _shift;
            _shift = false;
            return temp;
        }
        set {
            _shift = value;
        }
    }


    Controller2D controller;
    const float MIN_ANIM_SPEED = 0.1f;


    void Start() {
        controller = GetComponent<Controller2D>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        rollBreakingAcceleration = (2 * rollDistance) / Mathf.Pow(rollTime, 2);
        rollVelocity = rollBreakingAcceleration * rollTime;

        Debug.Log("Breaking: " + rollBreakingAcceleration + " ,Velocity: " + rollVelocity);

        //print ("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);
    }
    private void Update() {
        space |= Input.GetKeyDown("space");
        shift |= (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift));
    }

    void FixedUpdate() {

        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }
        if (currentMoveState == MovementStates.Normal) {
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        if (input.x != 0 && shift && controller.IsGrounded() && currentMoveState != MovementStates.Roll && Time.timeSinceLevelLoad >= lastRollTime + rollCD) {
            TrySwitchState(MovementStates.Roll);
            currentRollVelocity = input.x * rollVelocity;
        }



        float targetVelocityX = 0f;
        if (currentMoveState == MovementStates.Normal) {
            jump = space && controller.IsGrounded();
            if (jump) {
                velocity.y = jumpVelocity;
            }
            targetVelocityX = input.x * moveSpeed;
        }
        else if (currentMoveState == MovementStates.Roll) {
            targetVelocityX = currentRollVelocity;
            currentRollVelocity = input.x * Mathf.Max(Mathf.Abs(currentRollVelocity) - rollBreakingAcceleration * Time.deltaTime, 0f);
            if (currentRollVelocity == 0f) {
                currentMoveState = MovementStates.Normal;
            }

        }

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne), float.MaxValue, Time.fixedDeltaTime);
        if (controller.collisions.left || controller.collisions.right) {
            velocity.x = 0;
        }
        else if (controller._velocity.x > 0) {
            transform.rotation = Quaternion.Euler(Vector3.up * 0f);
        }
        else if (controller._velocity.x < 0) {
            transform.rotation = Quaternion.Euler(Vector3.up * -180f);
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (animator != null) {
            if (controller.IsGrounded()) {
                animator.speed = Mathf.Max(Mathf.Abs(velocity.x) / moveSpeed, MIN_ANIM_SPEED);
            }
            else {
                animator.speed = 1;
            }
        }


        if (FOV != null) {
            FOV.DrawFieldOfView();
        }
    }

    internal void Push(Vector3 monsterPos) {

    }

    void TrySwitchState(MovementStates targetState) {
        switch (currentMoveState) {
            case MovementStates.Normal:
                switch (targetState) {
                    case MovementStates.Normal:
                        break;
                    case MovementStates.Roll:
                        currentMoveState = MovementStates.Roll;
                        lastRollTime = Time.timeSinceLevelLoad;
                        break;
                    case MovementStates.Push:
                        currentMoveState = MovementStates.Push;
                        break;
                    case MovementStates.Dead:
                        currentMoveState = MovementStates.Dead;
                        break;
                }
                break;
            case MovementStates.Roll:
                switch (targetState) {
                    case MovementStates.Normal:
                        break;
                    case MovementStates.Roll:
                        break;
                    case MovementStates.Push:
                        break;
                    case MovementStates.Dead:
                        currentMoveState = MovementStates.Dead;
                        break;
                }
                break;
            case MovementStates.Push:
                switch (targetState) {
                    case MovementStates.Normal:
                        break;
                    case MovementStates.Roll:
                        break;
                    case MovementStates.Push:
                        break;
                    case MovementStates.Dead:
                        currentMoveState = MovementStates.Dead;
                        break;
                }
                break;
            case MovementStates.Dead:
                break;
        }
    }
}
