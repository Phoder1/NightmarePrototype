using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Enemies : MonoBehaviour {
    enum States { Patrol, Chase, Stunned, None};

    [Serializable]
    class Target {
        [SerializeField]
        internal Transform targetsTransform;

        [SerializeField]
        internal float maxIdleTime;
    }


    [SerializeField]
    bool IsFlying = false;
    [SerializeField]
    Target[] targets;
    [SerializeField]
    BoxCollider2D areaLimits;
    [SerializeField]
    SpriteRenderer darkRenderer;
    [SerializeField]
    SpriteRenderer coloredRenderer;
    [SerializeField]
    SpriteRenderer whiteRenderer;
    [SerializeField]
    SpriteRenderer coloredStunnedRenderer;
    Material darkMaterial;
    [SerializeField]
    float normalSpeed;
    [SerializeField]
    float timeToStun = 3f;
    [SerializeField]
    float maxTimeStunned = 5f;
    [SerializeField]
    float flashlightSpeedDecrease;
    [SerializeField]
    float detectionDistance;
    [SerializeField]
    Collider2D lightMaskCollider;
    [SerializeField]
    float animationTime = 0f;

    bool isBeingLit;
    float litTime = 0f;
    Transform playerTransform;
    float actualSpeed;
    Animator darkAnimator;
    Animator coloredAnimator;
    Transform darkTransform;
    Transform coloredTransform;
    Vector3 lastPlayerPos;
    int currentTarget = 0;
    Animator animator;
    States enemystate = States.Patrol;
    float startIdleTime = 0f;
    bool isIdle = false;
    float timeWhenStunned;
    

    const float MIN_TARGET_DISTANCE = 0.1f;
    const float MAX_ALPHA_WHITE_BLINK = 0.8f;
    const float MIN_ALPHA_WHITE_BLINK = 0f;
    const float BLINK_TIME = 2f;
    const float MIN_WALKINGSPEED_RATIO = 0.2f;

    // Start is called before the first frame update
    void Start() {
        darkAnimator = darkRenderer.GetComponent<Animator>();
        coloredAnimator = coloredRenderer.GetComponent<Animator>();
        darkTransform = darkRenderer.GetComponent<Transform>();
        coloredTransform = coloredRenderer.GetComponent<Transform>();
        darkMaterial = darkRenderer.material;
        actualSpeed = normalSpeed;
        playerTransform = MainCharacterControls.mainCharacter.transform;
        lastPlayerPos = playerTransform.position;
    }

    // Update is called once per frame
    void Update() {
        
        StateMachine();
        UpdateStun();
        lastPlayerPos = playerTransform.position;
        darkMaterial.SetFloat("DissolveIntensity", animationTime);
    }

    private void UpdateStun() {
        actualSpeed = Mathf.Clamp(actualSpeed + (isBeingLit ? -1 : 1) * flashlightSpeedDecrease * Time.deltaTime, 0, normalSpeed);
        if (actualSpeed <= MIN_WALKINGSPEED_RATIO * normalSpeed) {
            darkAnimator.SetBool("IsWalking", false);
            coloredAnimator.SetBool("IsWalking", false);
        }
        litTime = Mathf.Clamp(litTime + Time.deltaTime * (isBeingLit ? 1 : -1), 0f, timeToStun);
        //Debug.Log(litTime);
        if (litTime >= timeToStun && enemystate != States.Stunned) {
            darkAnimator.SetBool("IsWalking", false);
            coloredAnimator.SetBool("IsWalking", false);
            enemystate = States.Stunned;
            timeWhenStunned = Time.timeSinceLevelLoad;
            //coloredRenderer.maskInteraction = SpriteMaskInteraction.None;
        }
    }

    private void StateMachine() {
        switch (enemystate) {
            case States.Patrol:
                Patrol();
                if (Vector3.Distance(transform.position, playerTransform.position) <= detectionDistance) {
                    enemystate = States.Chase;
                }
                break;
            case States.Chase:
                Chase();
                if (Vector3.Distance(transform.position, playerTransform.position) > detectionDistance) {
                    enemystate = States.Patrol;
                }
                break;
            case States.Stunned:
                Stunned();
                if (Time.timeSinceLevelLoad >= timeWhenStunned + maxTimeStunned && !isBeingLit && litTime == 0f) {
                    enemystate = States.Patrol;
                    //LeanTween.alpha(darkRenderer.gameObject, MAX_ALPHA_WHITE_BLINK, BLINK_TIME).setEaseInSine();
                    //LeanTween.alpha(coloredRenderer.gameObject, MIN_ALPHA_WHITE_BLINK, BLINK_TIME).setEaseInSine();
                }
                break;
            case States.None:
                
                break;
        }
    }

    private void Stunned() {
        animationTime += Time.deltaTime*0.3f;
    }

    private void Chase() {
        if (isIdle && Vector3.Distance(transform.position, playerTransform.position) >= MIN_TARGET_DISTANCE) {
            darkAnimator.SetBool("IsWalking", true);
            coloredAnimator.SetBool("IsWalking", true);
            isIdle = false;
        }
        Vector3 playerPos = MoveTowards(playerTransform.position);
    }
    private void Patrol() {
        Vector3 targetPos = MoveTowards(targets[currentTarget].targetsTransform.position);
        if (Vector3.Distance(transform.position, targetPos) < MIN_TARGET_DISTANCE && !isIdle) {
            //Debug.Log(Vector3.Distance(transform.position, targetPos));
            if (lastPlayerPos == playerTransform.position) {
                darkAnimator.SetBool("IsWalking", false);
                coloredAnimator.SetBool("IsWalking", false);
            }
            else {
                darkAnimator.SetBool("IsWalking", true);
                coloredAnimator.SetBool("IsWalking", true);
            }
            startIdleTime = Time.timeSinceLevelLoad;
            isIdle = true;

        }
        if (isIdle && Time.timeSinceLevelLoad >= startIdleTime + targets[currentTarget].maxIdleTime) {
            darkAnimator.SetBool("IsWalking", true);
            coloredAnimator.SetBool("IsWalking", true);
            isIdle = false;
            currentTarget++;
            if (currentTarget >= targets.Length) {
                currentTarget = 0;
            }
        }
    }

    private Vector3 MoveTowards(Vector3 target) {
        darkAnimator.speed = actualSpeed / normalSpeed;
        float targetPosY = (IsFlying ? target.y : transform.position.y);
        Vector3 targetPos = new Vector3(target.x, targetPosY, transform.position.z);
        Vector3 nextPos = Vector3.MoveTowards(transform.position, targetPos, actualSpeed * Time.deltaTime);

        float minX = (areaLimits.transform.position.x + areaLimits.offset.x - areaLimits.bounds.extents.x) + (darkRenderer.bounds.extents.x >= coloredRenderer.bounds.extents.x ? darkRenderer.bounds.extents.x : coloredRenderer.bounds.extents.x);
        float maxX = (areaLimits.transform.position.x + areaLimits.offset.x + areaLimits.bounds.extents.x) - (darkRenderer.bounds.extents.x >= coloredRenderer.bounds.extents.x ? darkRenderer.bounds.extents.x : coloredRenderer.bounds.extents.x);
        float minY = (areaLimits.transform.position.y + areaLimits.offset.y - areaLimits.bounds.extents.y) + (darkRenderer.bounds.extents.y >= coloredRenderer.bounds.extents.y ? darkRenderer.bounds.extents.y : coloredRenderer.bounds.extents.y);
        float maxY = (areaLimits.transform.position.y + areaLimits.offset.y + areaLimits.bounds.extents.y) - (darkRenderer.bounds.extents.y >= coloredRenderer.bounds.extents.y ? darkRenderer.bounds.extents.y : coloredRenderer.bounds.extents.y);
        if ((Vector3.Distance(nextPos, targetPos) < MIN_TARGET_DISTANCE || (nextPos.x < minX || nextPos.x>maxX)) && !isIdle) {
            darkAnimator.SetBool("IsIdle", true);
            coloredAnimator.SetBool("IsIdle", true);
            darkAnimator.SetBool("IsWalking", false);
            coloredAnimator.SetBool("IsWalking", false);
            startIdleTime = Time.timeSinceLevelLoad;
            isIdle = true;
        }
        if (transform.position.x - nextPos.x > 0) {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 180f, transform.rotation.eulerAngles.z);
        }
        else if (transform.position.x - nextPos.x < 0) {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 0f, transform.rotation.eulerAngles.z);
        }
        nextPos = new Vector3(Mathf.Clamp(nextPos.x, minX, maxX), Mathf.Clamp(nextPos.y, minY, maxY), transform.position.z);



        //Debug.Log("Magnitude: " + Vector3.Distance(transform.position, nextPos) + " ,Frame speed: " + normalSpeed * Time.deltaTime);


        transform.position = nextPos;

        return targetPos;
    }

    private void OnCollisionStay2D(Collision2D collision) {
        //Debug.Log("Is lit?: " + isBeingLit.ToString());
        ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];
        collision.GetContacts(contacts);
        for (int i = 0; i < collision.contactCount; i++) {
            if (contacts[i].collider == lightMaskCollider) {
                isBeingLit = true;

            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        isBeingLit = false;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, darkRenderer.bounds.size);
        Gizmos.DrawWireCube(areaLimits.transform.position + (Vector3)areaLimits.offset, areaLimits.bounds.size);
        Gizmos.DrawWireSphere(transform.position, detectionDistance);
    }
}





