using System;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class PlayerMovement : MonoBehaviour {

    [Header("Refrences")]
    [SerializeField]
    Animator animator;
    [SerializeField]
    FieldOfView FOV;

    [Header("Move")]
    [SerializeField]
    float moveSpeed = 6;
    [SerializeField]
    float accelerationTimeAirborne = 0.2f;
    [SerializeField]
    float accelerationTimeGrounded = 0.1f;
    [SerializeField]
    float accelerationTimeDead = 0.6f;

    [Header("Jump")]
    [SerializeField]
    float jumpHeight = 4;
    [SerializeField]
    float timeToJumpApex = 0.4f;

    [Header("Roll")]
    [SerializeField]
    float rollDistance = 3;
    [SerializeField]
    float rollTime = 0.3f;
    [SerializeField]
    float rollCD = 1f;
    [Tooltip("The ratio of the height if the collider, relative to it's usual height")]
    [SerializeField]
    float rollColliderHeight = 0.5f;

    [Header("Push")]
    [SerializeField]
    float pushDistance = 3;
    [SerializeField]
    float pushTime = 0.3f;





    internal enum MovementStates { Normal, Roll, Push, Dead };
    internal MovementStates currentMoveState = MovementStates.Normal;

    bool controlsEnabled = true;

    float gravity;
    float jumpVelocity;

    float rollBreakingAcceleration;
    float rollVelocity;
    float currentRollVelocity;
    float lastRollTime = 0f;

    float pushBreakingAcceleration;
    float pushVelocity;
    float currentPushVelocity;
    float lastPushTime = 0f;

    bool jump = false;

    Vector3 velocity;
    float velocityXSmoothing;
    //Controls
    Vector2 input;
    Vector3 initScale;
    Transform pivot;

    bool space;
    bool shift;

    Controller2D controller;
    const float MIN_ANIM_SPEED = 0.1f;

    MainCharacterControls mainCharacter;

    internal Vector3 hittingMonster;

    float targetVelocityX;

    Vector3 frameVelocity;

    void Start() {
        controller = GetComponent<Controller2D>();
        pivot = transform.GetChild(0);

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        rollBreakingAcceleration = (2 * rollDistance) / Mathf.Pow(rollTime, 2);
        rollVelocity = rollBreakingAcceleration * rollTime;

        pushBreakingAcceleration = (2 * pushDistance) / Mathf.Pow(pushTime, 2);
        pushVelocity = pushBreakingAcceleration * pushTime;

        Debug.Log("Breaking: " + rollBreakingAcceleration + " ,Velocity: " + rollVelocity);
        initScale = pivot.transform.localScale;

        mainCharacter = MainCharacterControls.mainCharacter;
        //print ("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);
    }
    private void Update() {
        //GetKeyDown doesn't work well with FixedUpdate because it get reset every normal update so sometimes it skips a click, so I made a custom one
        space |= Input.GetKeyDown("space");
        shift |= (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift));
    }

    void FixedUpdate() {
        Debug.Log(currentMoveState);
        if (currentMoveState != MovementStates.Dead && currentMoveState != MovementStates.Push) {
            Move();
        }
        else if (currentMoveState == MovementStates.Push) {
            Push();
        }

        ApplyMove();
        ResetKeys();
        if (FOV != null && frameVelocity.magnitude > 0.01f) {
            Debug.Log("FOV Update: " + frameVelocity +", " + frameVelocity.magnitude);
            FOV.DrawFieldOfView();
        }
        targetVelocityX = 0f;
    }

    private void ResetKeys() {
        //Reset the Custom GetKeyDowm
        space = false;
        shift = false;
    }

    public void Move() {
        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }
        if (currentMoveState == MovementStates.Normal) {
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        if (shift && input.x != 0 && controller.IsGrounded() && currentMoveState != MovementStates.Roll && Time.timeSinceLevelLoad >= lastRollTime + rollCD) {
            TrySwitchState(MovementStates.Roll);
            currentRollVelocity = input.x * rollVelocity;
            controller.colliderHeight = rollColliderHeight;
        }



        
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
                controller.colliderHeight = 1f;
            }

        }
    }

    private void ApplyMove() {
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (currentMoveState == MovementStates.Dead? accelerationTimeDead :(controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne)), float.MaxValue, Time.fixedDeltaTime);
        if (controller.collisions.left || controller.collisions.right) {
            velocity.x = 0;
        }
        else if (controller._velocity.x > 0) {
            pivot.transform.localScale = new Vector3(initScale.x, initScale.y, initScale.z);
        }
        else if (controller._velocity.x < 0) {
            pivot.transform.localScale = new Vector3(-initScale.x, initScale.y, initScale.z);
        }
        velocity.y += gravity * Time.deltaTime;
        frameVelocity = controller.Move(velocity * Time.deltaTime);
        

        if (animator != null) {
            if (controller.IsGrounded() && currentMoveState != MovementStates.Dead) {
                animator.speed = Mathf.Max(Mathf.Abs(velocity.x) / moveSpeed, MIN_ANIM_SPEED);
            }
            else {
                animator.speed = 1;
            }
        }
    }

    internal void Push() {
        targetVelocityX = currentPushVelocity;
        currentPushVelocity = input.x * Mathf.Max(Mathf.Abs(currentPushVelocity) - pushBreakingAcceleration * Time.deltaTime, 0f);
        if (currentPushVelocity == 0f) {
            currentMoveState = MovementStates.Normal;
        }
    }

    internal void TrySwitchState(MovementStates targetState) {
        if (targetState != currentMoveState) {
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

            //code to run when switching
            if(currentMoveState == targetState) {
                switch (currentMoveState) {
                    case MovementStates.Normal:
                        break;
                    case MovementStates.Roll:
                        break;
                    case MovementStates.Push:
                        currentPushVelocity = Mathf.Sign(transform.position.x - hittingMonster.x) * pushVelocity;
                        break;
                    case MovementStates.Dead:
                        break;
                }
            }


        }
    }
}