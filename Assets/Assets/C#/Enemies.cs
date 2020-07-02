using System;
using UnityEngine;

public class Enemies : MonoBehaviour {
    enum States { Patrol, Chase, Stunned, Transition, Attack, Hit, None };

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
    SpriteRenderer coloredStunnedRenderer;
    [SerializeField]
    GameObject stunnedMask;

    Material darkMaterial;
    [SerializeField]
    float normalSpeed;
    [SerializeField]
    int numOfLives = 3;
    [SerializeField]
    float lightTimeToStun = 3f;
    [SerializeField]
    float maxTimeStunned = 5f;
    [SerializeField]
    float flashlightSpeedDecrease;
    [SerializeField]
    float detectionDistance;
    [SerializeField]
    float distanceToAttack;
    [SerializeField]
    float rechargeTime = 3f;
    [SerializeField]
    float hitForce;
    [SerializeField]
    float hitStopForce;
    [SerializeField]
    float hitStunTime;

    Collider2D lightMaskCollider;

    //State machine
    States lastState = States.None;
    States currentState = States.Patrol;
    States nextState = States.None;

    float attackTime = 0f;

    float animationTime = 0f;
    bool isBeingLit;
    bool wasHit = false;
    bool isStillBeingHit = false;
    int life;
    float litTime = 0f;
    Transform playerTransform;
    float actualSpeed;
    Animator darkAnimator;
    Animator coloredAnimator;
    int currentTarget = 0;
    float startIdleTime = 0f;
    bool isIdle = false;
    float timeWhenStunned;
    float hitTime = 0f;
    float hitMoveSpeed = 0f;
    Vector3 hitDirection = Vector3.zero;


    const float ATTACK_EXTRA_RANGE = 0.5f;
    const float BLINK_TIME = 2f;
    const float MIN_WALKINGSPEED_RATIO = 0.2f;

    // Start is called before the first frame update
    void Start() {
        darkAnimator = darkRenderer.GetComponent<Animator>();
        coloredAnimator = coloredRenderer.GetComponent<Animator>();
        lightMaskCollider = GameObject.FindGameObjectWithTag("LightconeMask").GetComponent<Collider2D>();
        darkMaterial = darkRenderer.material;
        darkMaterial.SetFloat("DissolveIntensity", 0f);
        actualSpeed = normalSpeed;
        playerTransform = MainCharacterControls.mainCharacter.transform;
        life = numOfLives;
        stunnedMask.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        UpdateStun();
        StateMachine();
    }

    private void UpdateEffect() {
        if (nextState == States.Stunned) {
            animationTime = Mathf.Clamp(animationTime + (Time.deltaTime / BLINK_TIME), 0f, 1f);
        }
        else {
            animationTime = Mathf.Clamp(animationTime - (Time.deltaTime / BLINK_TIME), 0f, 1f);
        }
        darkMaterial.SetFloat("DissolveIntensity", animationTime);
    }

    private void UpdateStun() {
        actualSpeed = Mathf.Clamp(actualSpeed + (isBeingLit ? -1 : 1) * flashlightSpeedDecrease * Time.deltaTime, 0, normalSpeed);
        if (actualSpeed <= MIN_WALKINGSPEED_RATIO * normalSpeed) {
            darkAnimator.SetBool("IsWalking", false);
            coloredAnimator.SetBool("IsWalking", false);
        }
        litTime = Mathf.Clamp(litTime + Time.deltaTime * (isBeingLit ? 1 : -1), 0f, lightTimeToStun);

        if (litTime >= lightTimeToStun && currentState != States.Stunned && currentState != States.Transition) {
            currentState = States.Transition;
            nextState = States.Stunned;

        }
    }

    private void StateMachine() {

        switch (currentState) {
            ////////////////////////////////////////////////////
            case States.Patrol:
                if (currentState != lastState) {
                    isIdle = false;

                    lastState = currentState;
                }
                Patrol();
                if (Vector3.Distance(transform.position, playerTransform.position) <= detectionDistance) {
                    currentState = States.Chase;
                }
                else if (Vector3.Distance(transform.position, playerTransform.position) <= distanceToAttack) {
                    currentState = States.Attack;
                }
                else if (wasHit) {

                    currentState = States.Hit;
                    life--;
                    wasHit = false;
                }



                break;

            ////////////////////////////////////////
            case States.Chase:
                if (currentState != lastState) {
                    darkAnimator.SetBool("IsWalking", true);
                    coloredAnimator.SetBool("IsWalking", true);
                    lastState = currentState;
                    isIdle = false;
                }
                Chase();
                Debug.DrawLine(transform.position, transform.position + transform.right * distanceToAttack);
                if (Vector3.Distance(transform.position, playerTransform.position) > detectionDistance) {
                    currentState = States.Patrol;
                }
                else if (Vector3.Distance(transform.position, playerTransform.position) <= distanceToAttack) {
                    currentState = States.Attack;
                }
                else if (wasHit) {
                    currentState = States.Hit;
                    life--;
                    wasHit = false;

                }
                break;

            ///////////////////////////////////////
            case States.Stunned:
                if (currentState != lastState) {
                    timeWhenStunned = Time.timeSinceLevelLoad;
                    stunnedMask.SetActive(true);
                    lastState = currentState;
                }
                Stunned();
                wasHit = false;
                if (Time.timeSinceLevelLoad >= timeWhenStunned + maxTimeStunned && !isBeingLit) {
                    stunnedMask.SetActive(false);
                    currentState = States.Transition;
                    nextState = States.Patrol;
                    life = numOfLives;
                }
                break;

            ////////////////////////////////////////////////////
            case States.Transition:
                if (currentState != lastState) {
                    darkAnimator.SetBool("IsWalking", false);
                    coloredAnimator.SetBool("IsWalking", false);
                    if (nextState == States.Stunned) {
                        darkMaterial.SetFloat("Respawn", 0f);
                        LeanTween.alpha(coloredStunnedRenderer.gameObject, 1f, BLINK_TIME).setEaseInCirc();
                    }
                    else {
                        darkMaterial.SetFloat("Respawn", 1f);
                        LeanTween.alpha(coloredStunnedRenderer.gameObject, 0f, BLINK_TIME).setEaseOutCirc();
                    }

                    lastState = currentState;
                }
                UpdateEffect();
                wasHit = false;
                if (animationTime == 1f || animationTime == 0f) {
                    
                    currentState = nextState;
                }
                break;

            ////////////////////////////////////////////////////
            case States.Attack:
                if (currentState != lastState) {
                    darkAnimator.SetBool("IsWalking", false);
                    coloredAnimator.SetBool("IsWalking", false);
                    MainCharacterControls.mainCharacter.WasHit(transform.position);
                    attackTime = Time.timeSinceLevelLoad;

                    lastState = currentState;
                }


                if (Time.timeSinceLevelLoad >= attackTime + rechargeTime) {
                    currentState = States.Chase;
                }
                else if (wasHit) {
                    currentState = States.Hit;
                    life--;
                    wasHit = false;

                }

                break;

            ////////////////////////////////////////////////////
            case States.None:
                break;

            ////////////////////////////////////////////////////
            case States.Hit:
                if (currentState != lastState) {
                    darkAnimator.SetBool("IsWalking", false);
                    darkAnimator.SetTrigger("IsHit");
                    hitDirection = transform.position - playerTransform.position;
                    hitTime = Time.timeSinceLevelLoad;
                    hitMoveSpeed = hitForce;

                    lastState = currentState;
                }
                hitDirection.z = 0f;
                if (!IsFlying) {
                    hitDirection.y = 0f;
                }

                hitMoveSpeed = Mathf.Max(hitMoveSpeed - hitStopForce * Time.deltaTime, 0f);
                transform.position += hitDirection * hitMoveSpeed * Time.deltaTime;

                if (life == 0 && hitMoveSpeed == 0f) {
                    currentState = States.Transition;
                    nextState = States.Stunned;
                    life = numOfLives;
                }
                else if (Time.timeSinceLevelLoad >= hitTime + hitStunTime) {
                    currentState = States.Chase;

                }



                break;
        }
    }

    private void Stunned() {

    }

    private void Chase() {
        Vector3 nextChasePosition = MoveTowards(playerTransform.position);
        if ((nextChasePosition - transform.position).magnitude != 0f && isIdle) {
            isIdle = false;
        }

    }
    private void Patrol() {
        if(targets.Length > 0) {
            Vector3 targetPos = MoveTowards(targets[currentTarget].targetsTransform.position);
            if (isIdle && Time.timeSinceLevelLoad >= startIdleTime + targets[currentTarget].maxIdleTime) {
                isIdle = false;
                currentTarget++;
                if (currentTarget >= targets.Length) {
                    currentTarget = 0;
                }
            }
        }
    }

    private Vector3 MoveTowards(Vector3 target) {
        darkAnimator.speed = actualSpeed / normalSpeed;
        float targetPosY = (IsFlying ? target.y : transform.position.y);
        Vector3 targetPos = new Vector3(target.x, targetPosY, transform.position.z);
        Vector2 minPoint = areaLimits.bounds.min + darkRenderer.bounds.extents;
        Vector3 maxPoint = areaLimits.bounds.max - darkRenderer.bounds.extents;
        Vector3 nextPos = Vector3.MoveTowards(transform.position, targetPos, actualSpeed * Time.deltaTime);

        if (transform.position.x - nextPos.x > 0) {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 180f, transform.rotation.eulerAngles.z);
        }
        else if (transform.position.x - nextPos.x < 0) {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 0f, transform.rotation.eulerAngles.z);
        }

        nextPos = new Vector3(Mathf.Clamp(nextPos.x, minPoint.x, maxPoint.x), Mathf.Clamp(nextPos.y, minPoint.y, maxPoint.y), transform.position.z);

        //Debug.Log(transform.position + " => " + nextPos + " , " + (nextPos - transform.position).magnitude + " , " + transform.name);
        //Debug.Log("Is idle: " + isIdle.ToString() + " ,start idle time: " + startIdleTime);

        if ((nextPos - transform.position).magnitude == 0f && !isIdle) {
            darkAnimator.SetBool("IsWalking", false);
            coloredAnimator.SetBool("IsWalking", false);
            startIdleTime = Time.timeSinceLevelLoad;
            isIdle = true;
        }
        else if (!isIdle) {
            darkAnimator.SetBool("IsWalking", true);
            coloredAnimator.SetBool("IsWalking", true);
        }
        else {
            darkAnimator.SetBool("IsWalking", false);
            coloredAnimator.SetBool("IsWalking", false);
        }




        //Debug.Log("Magnitude: " + Vector3.Distance(transform.position, nextPos) + " ,Frame speed: " + normalSpeed * Time.deltaTime);


        transform.position = nextPos;

        return nextPos;
    }

    private void OnCollisionStay2D(Collision2D collision) {
        //Debug.Log("Is lit?: " + isBeingLit.ToString());
        ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];
        collision.GetContacts(contacts);
        for (int i = 0; i < collision.contactCount; i++) {
            if (contacts[i].collider == lightMaskCollider) {
                isBeingLit = true;
            }
            if (contacts[i].collider.tag == "Spoon") {
                if (currentState == States.Chase || currentState == States.Patrol && isStillBeingHit == false) {
                    wasHit = true;
                }

                isStillBeingHit = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        isBeingLit = false;
        isStillBeingHit = false;
    }


    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, darkRenderer.bounds.size);
        if(areaLimits != null) {
            Gizmos.DrawWireCube(areaLimits.transform.position + (Vector3)areaLimits.offset, areaLimits.bounds.size);
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanceToAttack);

    }
}





