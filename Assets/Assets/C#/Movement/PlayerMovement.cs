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
    Vector2 moveDirection;
    Vector3 initScale;
    Transform pivot;

    bool space;
    bool shift;

    Controller2D controller;
    const float MIN_ANIM_SPEED = 0.1f;

    MainCharacterControls mainCharacter;
    Animator playerAnimator;

    internal Vector3 hittingMonster;

    float targetVelocityX;

    Vector3 frameVelocity;

    float dropTime = 0f;
    const float dropDuration = 0.25f;
    bool droppingPlatform = false;
    

    void Start() {
        mainCharacter = MainCharacterControls.mainCharacter;
        playerAnimator = GetComponentInChildren<Animator>();
        controller = GetComponent<Controller2D>();
        pivot = transform.GetChild(0);

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        rollVelocity = (2 * rollDistance) / rollTime;
        rollBreakingAcceleration = -(rollVelocity / rollTime);

        pushTime = mainCharacter.stunTime * 0.8f;

        pushVelocity = (2 * pushDistance) / pushTime;
        pushBreakingAcceleration = -(pushVelocity/pushTime);
        

        Debug.Log("Breaking: " + rollBreakingAcceleration + " ,Velocity: " + rollVelocity);
        initScale = pivot.transform.localScale;

        
        //print ("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);
    }
    private void Update() {
        //GetKeyDown doesn't work well with FixedUpdate because it get reset every normal update so sometimes it skips a click, so I made a custom one
        space |= Input.GetKeyDown("space");
        shift |= (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift));
    }

    void FixedUpdate() {
        //Debug.Log(currentMoveState);
        if (currentMoveState != MovementStates.Dead && currentMoveState != MovementStates.Push) {
            Move();
        }
        else {
            Push();
        }

        ApplyMove();
        ResetKeys();
        if (FOV != null && frameVelocity.magnitude > 0.01f) {
            FOV.DrawFieldOfView();
        }
        playerAnimator.SetBool("Running", !(frameVelocity.x > -0.01f && frameVelocity.x < 0.01f) && mainCharacter.playerCurrentState == MainCharacterControls.PlayerStates.Moving);
        playerAnimator.SetBool("IsJump", frameVelocity.y > 0.01f);
        playerAnimator.SetBool("IsFalling", frameVelocity.y < -0.01f);
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
            moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        if (shift && moveDirection.x != 0 && controller.IsGrounded() && currentMoveState != MovementStates.Roll && Time.timeSinceLevelLoad >= lastRollTime + rollCD) {
            TrySwitchState(MovementStates.Roll);
            currentRollVelocity = moveDirection.x * rollVelocity;
            controller.colliderHeight = rollColliderHeight;
        }




        if (currentMoveState == MovementStates.Normal) {
            jump = space && controller.IsGrounded() && Input.GetAxisRaw("Vertical") != -1;
            if (jump) {
                velocity.y = jumpVelocity;
            }
            targetVelocityX = moveDirection.x * moveSpeed;
            controller.colliderHeight = 1f;
        }
        else if (currentMoveState == MovementStates.Roll) {
            targetVelocityX = currentRollVelocity;
            currentRollVelocity = moveDirection.x * Mathf.Max(Mathf.Abs(currentRollVelocity) + rollBreakingAcceleration * Time.deltaTime, 0f);
            if (currentRollVelocity == 0f) {
                currentMoveState = MovementStates.Normal;
                
            }

        }else {
            controller.colliderHeight = 1f;
        }
    }

    private void ApplyMove() {
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (currentMoveState == MovementStates.Dead ? accelerationTimeDead : (controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborne)), float.MaxValue, Time.fixedDeltaTime);
        if (controller.collisions.left || controller.collisions.right) {
            velocity.x = 0;
        }
        else if (((currentMoveState != MovementStates.Dead && currentMoveState != MovementStates.Push) ? controller._velocity.x :   hittingMonster.x-transform.position.x) > 0) {
            pivot.transform.localScale = new Vector3(initScale.x, initScale.y, initScale.z);
        }
        else if (((currentMoveState != MovementStates.Dead && currentMoveState != MovementStates.Push) ? controller._velocity.x : hittingMonster.x - transform.position.x  ) < 0) {
            pivot.transform.localScale = new Vector3(-initScale.x, initScale.y, initScale.z);
        }

        velocity.y += gravity * Time.deltaTime;
        if (Input.GetAxisRaw("Vertical") == -1 && space && !droppingPlatform) {
            droppingPlatform = true;
            dropTime = Time.timeSinceLevelLoad;
        }
        if (Time.timeSinceLevelLoad >= dropTime + dropDuration) {
            droppingPlatform = false;
        }
        frameVelocity = controller.Move(velocity * Time.deltaTime, !droppingPlatform);
        Debug.Log(droppingPlatform);


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
        Debug.Log("Pushed");
        moveDirection.x = Mathf.Sign(transform.position.x - hittingMonster.x);
        targetVelocityX = currentPushVelocity;
        currentPushVelocity = moveDirection.x * Mathf.Max(Mathf.Abs(currentPushVelocity) + pushBreakingAcceleration * Time.deltaTime, 0f);
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
            if (currentMoveState == targetState) {
                switch (currentMoveState) {
                    case MovementStates.Normal:
                        break;
                    case MovementStates.Roll:
                        break;
                    case MovementStates.Push:
                        velocity.x = 0f;
                        currentPushVelocity = Mathf.Sign(transform.position.x - hittingMonster.x) * pushVelocity;
                        break;
                    case MovementStates.Dead:
                        velocity.x = 0;
                        currentPushVelocity = Mathf.Sign(transform.position.x - hittingMonster.x) * pushVelocity;
                        break;
                }
            }


        }
    }
}