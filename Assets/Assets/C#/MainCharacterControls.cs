using System;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(Controller2D))]
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
        internal GameObject Spoon;
        [SerializeField]
        internal Animator SpoonPivotAnimator;
        [SerializeField]
        internal LayerMask GroundLayerMask;
        [SerializeField]
        internal Collider2D PlayerCollider;

        

    }
    [SerializeField]
    Refrences refrences;



    [SerializeField]
    float MouseSensetivity;

    [SerializeField]
    int ShoulderMaxDegree;
    [SerializeField]
    int ShoulderMinDegree;


    
    const float GROUND_DETECTION_DISTANCE=0.1f;
    

    [SerializeField]
    float attackDuration = 1f;
    [SerializeField]
    float stunTime = 1f;

    [SerializeField]
    float minFlashlightOnTime = 1f;
    [SerializeField]
    float maxFlashlightChargeTime = 5f;

    [SerializeField]
    float resetSceneDelay = 2f;

    //General Variables
    PlayerMovement playerMovement;
    Animator playerAnimator;    
    SpriteRenderer spoonRenderer;
    Collider2D spoonCollider;
    Animator spoonAnimator;
    public static MainCharacterControls mainCharacter;
    float deathTime;
    Controller2D controller;




    //State machine
    internal enum PlayerStates { Idle, Moving, Jump, Falling, Hit, Dead, None };
    internal PlayerStates playerCurrentState = PlayerStates.Idle;
    PlayerStates playerLastState = PlayerStates.None;

    float shoulderDegree;
    enum HandStates { Spoon, Flashlight, Transition, None };
    HandStates lastAttackState = HandStates.None;
    HandStates currentAttackState = HandStates.Flashlight;
    HandStates nextAttackState = HandStates.None;
    bool flashlightOn;
    float timeSinceFlashlightOn = 0f;
    float currentFlashlightChargeTime;
    float spoonEffectTime;
    float hitTime = 0f;
    GameObject hittingMonster;


    const float spoonTransitionTime = 0.5f;
    
    const float GROUNDCHECK_RATIO = 0.06f;
   



    private void Awake() {
        mainCharacter = this;
    }


    // Start is called before the first frame update
    void Start() {
        controller = GetComponent<Controller2D>();
        playerAnimator = GetComponentInChildren<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        currentFlashlightChargeTime = maxFlashlightChargeTime;
        spoonRenderer = refrences.Spoon.GetComponent<SpriteRenderer>();
        spoonCollider = refrences.Spoon.GetComponent<Collider2D>();
        spoonAnimator = refrences.SpoonPivotAnimator;
        spoonCollider.enabled = false;
        spoonAnimator.speed = 1 / attackDuration;
        refrences.ShoulderAnimator.speed = 1 / attackDuration;
    }

    // Update is called once per frame
    void Update() {
        Statemachine();
        AttackCheck();
        UpdateAnimator();
        Debug.Log(playerCurrentState);
    }

    private void AttackCheck() {
        switch (currentAttackState) {
            case HandStates.Spoon:
                if (currentAttackState != lastAttackState) {
                    lastAttackState = currentAttackState;
                }




                if (!refrences.ShoulderAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")) {
                    spoonCollider.enabled = false;
                    if (Input.mouseScrollDelta.y != 0f) {
                        currentAttackState = HandStates.Transition;
                        nextAttackState = HandStates.Flashlight;
                    }
                }
                if (Input.GetMouseButtonDown(0)) {
                    refrences.ShoulderAnimator.SetTrigger("Attacking");
                    spoonAnimator.SetTrigger("Attacking");
                    spoonCollider.enabled = true;
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
                        refrences.ShoulderTransform.localRotation = Quaternion.Euler(0f, 0f, 14.114f);
                    }

                    lastAttackState = currentAttackState;
                }
                spoonEffectTime = Mathf.Clamp(spoonEffectTime + (nextAttackState == HandStates.Spoon ? Time.deltaTime/spoonTransitionTime : -Time.deltaTime/spoonTransitionTime), 0f, 1f);
                spoonRenderer.material.SetFloat("SpoonEffectTime", spoonEffectTime);

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
                if (controller.IsGrounded()) {
                    if (Input.GetKeyDown("space") && Input.GetAxis("Vertical") != -1f) {
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
                if (controller.IsGrounded()) {
                    if (Input.GetKeyDown("space") && Input.GetAxis("Vertical") != -1f) {
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
                    playerLastState = playerCurrentState;
                }

                //State end condition

                if (controller.IsGrounded() && controller._velocity.y <= 0f) {
                    playerCurrentState = PlayerStates.Idle;
                    //Debug.Log("Landed");
                }
                else if (controller._velocity.y < 0f) {
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
                if (controller.IsGrounded()) {

                    playerCurrentState = PlayerStates.Idle;
                }

                //State update



                break;
            //////////////////////////////////////////////////
            case PlayerStates.Hit:
                if (playerCurrentState != playerLastState) {
                    hitTime = Time.timeSinceLevelLoad;
                    GameManager.gameManager.LoseLife();
                    playerMovement.Push(hittingMonster.transform.position);

                    playerLastState = playerCurrentState;
                }

                if (Time.timeSinceLevelLoad >= hitTime + stunTime) {
                    playerCurrentState = PlayerStates.Idle;
                }

                break;
            case PlayerStates.None:
                break;

            /////////////////////////////////////////////////////////////
            case PlayerStates.Dead:
                if (playerCurrentState != playerLastState) {

                    playerLastState = playerCurrentState;
                }


                if(Time.timeSinceLevelLoad >= deathTime + resetSceneDelay) {
                    GameManager.gameManager.ResetScene();
                }


                break;
        }
    }


    public void Dead() {
        playerCurrentState = PlayerStates.Dead;
        refrences.ShoulderTransform.gameObject.SetActive(false);
        deathTime = Time.timeSinceLevelLoad;
    }

    public void WasHit(GameObject enemy) {
        if(playerCurrentState != PlayerStates.Dead && playerCurrentState != PlayerStates.Hit && playerCurrentState != PlayerStates.None) {
            playerCurrentState = PlayerStates.Hit;
            hittingMonster = enemy;
        }
        
        Debug.Log("Was hit!");
    }

    public void CollectedBattery() {
        Debug.Log("Battery!");
        currentFlashlightChargeTime = maxFlashlightChargeTime;
    }
    bool Isgrounded() {
        Vector3 minPoint = refrences.PlayerCollider.bounds.min + Vector3.right * GROUNDCHECK_RATIO * refrences.PlayerCollider.bounds.size.x;
        Vector3 maxPoint = refrences.PlayerCollider.bounds.min + Vector3.right * refrences.PlayerCollider.bounds.size.x - Vector3.right * (GROUNDCHECK_RATIO * refrences.PlayerCollider.bounds.size.x);
        RaycastHit2D minGroundRay = Physics2D.Raycast(minPoint, Vector3.down, GROUND_DETECTION_DISTANCE, refrences.GroundLayerMask);
        RaycastHit2D maxGroundRay = Physics2D.Raycast(maxPoint, Vector3.down, GROUND_DETECTION_DISTANCE, refrences.GroundLayerMask);
        Debug.DrawRay(maxPoint , Vector3.down * GROUND_DETECTION_DISTANCE);
        Debug.DrawRay(minPoint, Vector3.down * GROUND_DETECTION_DISTANCE);
        //Debug.Log("Checking ground!" + (minGroundRay.collider != null || maxGroundRay.centroid != null).ToString());
        return minGroundRay.collider != null || maxGroundRay.collider != null;

    }
    void UpdateAnimator() {
        playerAnimator.SetBool("Running", playerCurrentState == PlayerStates.Moving);
        playerAnimator.SetBool("IsJump", playerCurrentState == PlayerStates.Jump);
        playerAnimator.SetBool("IsFalling", playerCurrentState == PlayerStates.Falling);
        playerAnimator.SetBool("IsDead", playerCurrentState == PlayerStates.Dead);
    }

    private void OnDrawGizmos() {
        spoonCollider = refrences.Spoon.GetComponent<Collider2D>();
        Gizmos.DrawRay(transform.position, Vector3.down * GROUND_DETECTION_DISTANCE);
        Gizmos.DrawWireCube(spoonCollider.bounds.center, spoonCollider.bounds.size);
    }
}
