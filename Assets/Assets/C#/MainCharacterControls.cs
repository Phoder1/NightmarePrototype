using System;
using UnityEngine;

public class MainCharacterControls : MonoBehaviour {



    //General Serialized Variables

    //refrences:
    [Serializable]
    class Refrences {
        [SerializeField]
        internal Transform ShoulderTransform;
        [SerializeField]
        internal Animator ShoulderAnimator;
        [SerializeField]
        internal GameObject Flashlight;
        [SerializeField]
        internal GameObject lightCone;
        [SerializeField]
        internal SpriteRenderer lightconeSprite;
        [SerializeField]
        internal SpriteRenderer SpoonRenderer;
        [SerializeField]
        internal LayerMask GroundLayerMask;

    }
    [SerializeField]
    Refrences refrences;



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
    float groundDetectionDistance;
    [SerializeField]
    float JumpForce = 0f;

    [SerializeField]
    float minFlashlightOnTime = 1f;
    [SerializeField]
    float maxFlashlightChargeTime = 5f;

    //General Variables
    Animator playerAnimator;
    SpriteRenderer playerRenderer;
    Collider2D playerCollider;
    public static MainCharacterControls mainCharacter;

    //Movement variables
    float velocityY = 0f;
    float velocityX = 0f;
    float shoulderDegree;
    GameObject[] Platforms;
    GameObject[] Obstacles;


    //State machine
    enum PlayerStates { Idle, Moving, Jump, Falling, Flashlight, None };
    PlayerStates playerCurrentState = PlayerStates.Idle;
    PlayerStates playerLastState = PlayerStates.None;


    enum HandStates { Spoon, Flashlight, Transition, None };
    HandStates lastAttackState = HandStates.None;
    HandStates currentAttackState = HandStates.Flashlight;
    HandStates nextAttackState = HandStates.None;
    bool flashlightOn;
    float timeSinceFlashlightOn = 0f;
    float currentFlashlightChargeTime;
    float spoonEffectTime;

    const float spoonTransitionTime = 0.5f;
    const float SHOULDER_RETURN_SPEED = 5f;



    private void Awake() {
        if (mainCharacter != null && mainCharacter != this) {
            Destroy(this.gameObject);
        }
        else {
            mainCharacter = this;
        }

    }


    // Start is called before the first frame update
    void Start() {
        playerAnimator = GetComponentInChildren<Animator>();
        Platforms = GameObject.FindGameObjectsWithTag("Platforms");
        Obstacles = GameObject.FindGameObjectsWithTag("Obstacles");
        playerRenderer = GetComponentInChildren<SpriteRenderer>();
        playerCollider = GetComponentInChildren<Collider2D>();
        Cursor.lockState = CursorLockMode.Locked;
        currentFlashlightChargeTime = maxFlashlightChargeTime;
    }

    private void FixedUpdate() {
        Movement();
    }
    // Update is called once per frame
    void Update() {
        UpdateVariables();
        Statemachine();
        AttackCheck();

        UpdateAnimator();
        Debug.Log("Player state: " + playerCurrentState + " ,Attack state: " + currentAttackState);

    }

    private void AttackCheck() {
        switch (currentAttackState) {
            case HandStates.Spoon:
                if (currentAttackState != lastAttackState) {
                    lastAttackState = currentAttackState;
                }
                if (Input.GetMouseButtonDown(0)) {
                    refrences.ShoulderAnimator.SetTrigger("Attacking");
                }   




                if (Input.mouseScrollDelta.y != 0f && !refrences.ShoulderAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")) {
                    currentAttackState = HandStates.Transition;
                    nextAttackState = HandStates.Flashlight;
                }

                break;
            case HandStates.Flashlight:
                if (currentAttackState != lastAttackState) {
                    refrences.Flashlight.SetActive(true);


                    lastAttackState = currentAttackState;
                }
                if (currentFlashlightChargeTime > 0f) {
                    if (Input.GetMouseButton(0) && !flashlightOn) {
                        flashlightOn = true;
                        timeSinceFlashlightOn = Time.timeSinceLevelLoad;

                    }
                    else if (Input.GetMouseButtonDown(0)) {
                        flashlightOn = true;
                        timeSinceFlashlightOn = Mathf.Min(timeSinceFlashlightOn + minFlashlightOnTime * 0.3f, Time.timeSinceLevelLoad);
                    }
                    else if (Input.GetMouseButton(0)) {
                        flashlightOn = true;
                    }
                    else if (flashlightOn && Time.timeSinceLevelLoad >= timeSinceFlashlightOn + minFlashlightOnTime) {
                        flashlightOn = false;
                    }
                }
                else {
                    flashlightOn = false;
                }
                if (flashlightOn) {
                    currentFlashlightChargeTime = Mathf.Max(currentFlashlightChargeTime - Time.deltaTime, 0f);
                }
                refrences.lightconeSprite.material.SetFloat("BatteryLife", currentFlashlightChargeTime / maxFlashlightChargeTime);
                refrences.lightCone.SetActive(flashlightOn);
                shoulderDegree += Input.GetAxisRaw("Mouse Y") * MouseSensetivity * Time.deltaTime;
                shoulderDegree = Mathf.Clamp(shoulderDegree, ShoulderMinDegree, ShoulderMaxDegree);
                refrences.ShoulderTransform.localRotation = Quaternion.Euler(refrences.ShoulderTransform.localRotation.eulerAngles.x, refrences.ShoulderTransform.localRotation.eulerAngles.y, shoulderDegree);


                if (Input.mouseScrollDelta.y != 0f) {
                    currentAttackState = HandStates.Transition;
                    nextAttackState = HandStates.Spoon;
                    flashlightOn = false;
                }
                break;
            case HandStates.Transition:
                if (currentAttackState != lastAttackState) {
                    if (lastAttackState == HandStates.Flashlight) {
                        refrences.Flashlight.SetActive(false);
                        refrences.ShoulderTransform.rotation = Quaternion.Euler(0f, 0f, 14.114f);
                    }

                    lastAttackState = currentAttackState;
                }
                spoonEffectTime = Mathf.Clamp(spoonEffectTime + (nextAttackState == HandStates.Spoon ? Time.deltaTime/spoonTransitionTime : -Time.deltaTime/spoonTransitionTime), 0f, 1f);
                refrences.SpoonRenderer.material.SetFloat("SpoonEffectTime", spoonEffectTime);

                if (spoonEffectTime == 1f || spoonEffectTime == 0f) {
                    currentAttackState = nextAttackState;
                }
                break;
            case HandStates.None:
                break;
        }
    }



    private void Statemachine() {
        switch (playerCurrentState) {

            /////////////////////////////////////////////////////////////
            case PlayerStates.Idle:
                //State enter action
                if (playerCurrentState != playerLastState) {

                    playerLastState = playerCurrentState;
                }

                //State end condition
                if (Isgrounded()) {
                    if (Input.GetKeyDown("space") && Input.GetAxis("Vertical") != -1) {
                        playerCurrentState = PlayerStates.Jump;
                    }
                    else if (Input.GetAxis("Horizontal") != 0) {
                        playerCurrentState = PlayerStates.Moving;
                    }
                }
                else {
                    playerCurrentState = PlayerStates.Falling;
                }


                //State update


                break;

            /////////////////////////////////////////////////////////////
            case PlayerStates.Moving:
                //State enter action
                if (playerCurrentState != playerLastState) {

                    playerLastState = playerCurrentState;
                }

                //State end condition
                if (Isgrounded()) {
                    if (Input.GetKeyDown("space") && Input.GetAxis("Vertical") != -1) {
                        playerCurrentState = PlayerStates.Jump;
                    }
                    else if (Input.GetAxis("Horizontal") == 0) {
                        playerCurrentState = PlayerStates.Idle;
                    }
                }
                else {
                    playerCurrentState = PlayerStates.Falling;
                }

                //State update



                break;

            /////////////////////////////////////////////////////////////
            case PlayerStates.Jump:
                //State enter action
                if (playerCurrentState != playerLastState) {
                    velocityY += JumpForce;
                    playerLastState = playerCurrentState;
                }

                //State end condition

                if (Isgrounded() && velocityY <= 0f) {
                    playerCurrentState = PlayerStates.Idle;
                    //Debug.Log("Landed");
                }
                else if (velocityY < 0) {
                    playerCurrentState = PlayerStates.Falling;
                }

                //State update




                break;

            /////////////////////////////////////////////////////////////
            case PlayerStates.Falling:
                //State enter action
                if (playerCurrentState != playerLastState) {

                    playerLastState = playerCurrentState;
                }

                //State end condition
                if (Isgrounded()) {
                    playerCurrentState = PlayerStates.Idle;
                }

                //State update



                break;

        }
    }

    private void Movement() {

        velocityX = MoveVelocity * Input.GetAxis("Horizontal");
        if (velocityX < 0) {
            transform.localRotation = Quaternion.Euler(transform.localRotation.x, 180, transform.localRotation.z);
        }
        else if (velocityX > 0) {
            transform.localRotation = Quaternion.Euler(transform.localRotation.x, 0, transform.localRotation.z);
        }
        if (!Isgrounded()) {
            velocityY -= Gravity * Time.deltaTime;
        }
        Vector3 movement = (Vector3.up * velocityY * Time.deltaTime) + (Vector3.right * velocityX * Time.deltaTime);
        Vector2 playerMaxCorner = playerCollider.bounds.max;
        Vector2 playerMinCorner = playerCollider.bounds.min;
        Debug.DrawRay(playerMinCorner, Vector3.right * playerCollider.bounds.size.x, Color.red, Time.deltaTime);
        //Debug.Log(playerRenderer.bounds.extents.x);

        for (int i = 0; i < Platforms.Length; i++) {
            Collider2D platformCollider = Platforms[i].GetComponent<Collider2D>();
            //Debug.Log("Player min: " + playerMinCorner.y + " , Next pos: " + playerMinCorner.y + movement.y + " ,Platform height: " + platformCollider.bounds.max.y);
            if (velocityY <= 0f && (Mathf.Approximately(playerMinCorner.y, platformCollider.bounds.max.y)
                || playerMinCorner.y >= platformCollider.bounds.max.y)
                && playerMinCorner.y + movement.y <= platformCollider.bounds.max.y
                && playerMinCorner.x < platformCollider.bounds.max.x
                && playerMaxCorner.x > platformCollider.bounds.min.x) {

                movement.y = platformCollider.bounds.max.y - playerMinCorner.y;
                velocityY = 0f;
            }
        }
        for (int i = 0; i < Obstacles.Length; i++) {
            Collider2D obstacleCollider = Obstacles[i].GetComponent<Collider2D>();

            //Up
            if ((Mathf.Approximately(playerMinCorner.y, obstacleCollider.bounds.max.y)
                || playerMinCorner.y >= obstacleCollider.bounds.max.y)
                && playerMinCorner.y + movement.y <= obstacleCollider.bounds.max.y
                && playerMinCorner.x < obstacleCollider.bounds.max.x
                && playerMaxCorner.x > obstacleCollider.bounds.min.x) {

                movement.y = obstacleCollider.bounds.max.y - playerMinCorner.y;
                //Debug.Log("Up!");
                velocityY = 0f;
            }
            //Down
            if ((Mathf.Approximately(playerMaxCorner.y, obstacleCollider.bounds.min.y)
                || playerMaxCorner.y <= obstacleCollider.bounds.min.y)
                && playerMaxCorner.y + movement.y >= obstacleCollider.bounds.min.y
                && playerMinCorner.x < obstacleCollider.bounds.max.x
                && playerMaxCorner.x > obstacleCollider.bounds.min.x) {

                movement.y = obstacleCollider.bounds.min.y - playerMaxCorner.y;
                //Debug.Log("Down!");
                velocityY = 0f;
            }
            //Right
            if ((Mathf.Approximately(playerMinCorner.x, obstacleCollider.bounds.max.x)
                || playerMinCorner.x >= obstacleCollider.bounds.max.x)
                && playerMinCorner.x + movement.x <= obstacleCollider.bounds.max.x
                && playerMinCorner.y < obstacleCollider.bounds.max.y
                && playerMaxCorner.y > obstacleCollider.bounds.min.y) {

                movement.x = obstacleCollider.bounds.max.x - playerMinCorner.x;
                //Debug.Log("Right!");
                velocityX = 0f;
            }
            //Left
            if ((Mathf.Approximately(playerMaxCorner.x, obstacleCollider.bounds.min.x)
                || playerMaxCorner.x <= obstacleCollider.bounds.min.x)
                && playerMaxCorner.x + movement.x >= obstacleCollider.bounds.min.x
                && playerMinCorner.y < obstacleCollider.bounds.max.y
                && playerMaxCorner.y > obstacleCollider.bounds.min.y) {

                movement.x = obstacleCollider.bounds.min.x - playerMaxCorner.x;
                //Debug.Log("Left!");
                velocityX = 0f;
            }



        }

        transform.position += movement;

    }

    private void UpdateVariables() {
    }



    bool Isgrounded() {
        RaycastHit2D minGroundRay = Physics2D.Raycast(playerCollider.bounds.min, Vector3.down, groundDetectionDistance, refrences.GroundLayerMask);
        RaycastHit2D maxGroundRay = Physics2D.Raycast(playerCollider.bounds.min + Vector3.right * playerCollider.bounds.size.x, Vector3.down, groundDetectionDistance, refrences.GroundLayerMask);
        //Debug.DrawRay(playerCollider.bounds.min, Vector3.down * groundDetectionDistance);
        //Debug.DrawRay(playerCollider.bounds.min + Vector3.right * playerCollider.bounds.size.x, Vector3.down * groundDetectionDistance);
        //Debug.Log("Checking ground!" + (minGroundRay.collider != null || maxGroundRay.centroid != null).ToString());
        return minGroundRay.collider != null || maxGroundRay.collider != null;

    }
    void UpdateAnimator() {
        playerAnimator.SetBool("Running", playerCurrentState == PlayerStates.Moving);
        playerAnimator.SetBool("IsJump", playerCurrentState == PlayerStates.Jump);
        playerAnimator.SetBool("IsFalling", playerCurrentState == PlayerStates.Falling);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawRay(transform.position, Vector3.down * groundDetectionDistance);
    }
}
