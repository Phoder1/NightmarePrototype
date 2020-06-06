using UnityEngine;

public class Enemies : MonoBehaviour {
    enum States { Patrol, Chase, Stunned, Dead };

    [SerializeField]
    bool IsFlying = false;
    [SerializeField]
    Transform[] targets;
    [SerializeField]
    BoxCollider2D areaLimits;
    [SerializeField]
    SpriteRenderer darkRenderer;
    [SerializeField]
    SpriteRenderer coloredRenderer;
    [SerializeField]
    float maxIdleTime = 1f;
    [SerializeField]
    float normalSpeed;
    [SerializeField]
    float stunTime = 3f;
    [SerializeField]
    float flashlightSpeedDecrease;
    [SerializeField]
    float detectionDistance;
    [SerializeField]
    LayerMask lightConeMask = 9;


    bool isBeingLit;
    Transform playerTransform;
    float actualSpeed;
    Animator darkAnimator;
    Animator coloredAnimator;
    Transform darkTransform;
    Transform coloredTransform;

    int currentTarget = 0;
    Animator animator;
    const float MIN_TARGET_DISTANCE = 0.1f;
    States enemystate = States.Patrol;
    float startIdleTime = 0f;
    bool isIdle = false;
    private void Awake() {
        darkAnimator = darkRenderer.GetComponent<Animator>();
        coloredAnimator = coloredRenderer.GetComponent<Animator>();
        darkTransform = darkRenderer.GetComponent<Transform>();
        coloredTransform = coloredRenderer.GetComponent<Transform>();
    }
    // Start is called before the first frame update
    void Start() {
        actualSpeed = normalSpeed;
        playerTransform = MainCharacterControls.mainCharacter.transform;
    }

    // Update is called once per frame
    void Update() {
        switch (enemystate) {
            case States.Patrol:
                Patrol();
                break;
            case States.Chase:
                Chase();
                break;
            case States.Stunned:
                Stunned();
                break;
            case States.Dead:
                Dead();
                break;
        }

        if (Vector3.Distance(transform.position, playerTransform.position) <= detectionDistance) {
            enemystate = States.Chase;
        }
        else {
            enemystate = States.Patrol;
        }

        if (!isBeingLit) {
            actualSpeed = Mathf.Clamp(actualSpeed + flashlightSpeedDecrease, 0, normalSpeed);
        }
    }

    private void Dead() {

    }

    private void Stunned() {

    }

    private void Chase() {
        Vector3 playerPos = MoveTowards(playerTransform.position);
        if (Vector3.Distance(transform.position, playerPos) >= MIN_TARGET_DISTANCE) {
            darkAnimator.SetBool("IsWalking", true);
            coloredAnimator.SetBool("IsWalking", true);
            isIdle = false;
        }


    }
    private void Patrol() {
        Vector3 targetPos = MoveTowards(targets[currentTarget].position);
        if (Vector3.Distance(transform.position, targetPos) < MIN_TARGET_DISTANCE && !isIdle) {
            darkAnimator.SetBool("IsIdle", true);
            coloredAnimator.SetBool("IsIdle", true);
            darkAnimator.SetBool("IsWalking", false);
            coloredAnimator.SetBool("IsWalking", false);
            startIdleTime = Time.timeSinceLevelLoad;
            isIdle = true;

        }
        if (isIdle && Time.timeSinceLevelLoad >= startIdleTime + maxIdleTime) {
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



        float targetPosY = (IsFlying ? target.y : transform.position.y);
        Vector3 targetPos = new Vector3(target.x, targetPosY, transform.position.z);
        Vector3 nextPos = Vector3.MoveTowards(transform.position, targetPos, normalSpeed * Time.deltaTime);
        if (transform.position.x - nextPos.x > 0) {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 180f, transform.rotation.eulerAngles.z);
        }
        else if (transform.position.x - nextPos.x < 0) {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 0f, transform.rotation.eulerAngles.z);
        }
        float minX = (areaLimits.transform.position.x + areaLimits.offset.x - areaLimits.bounds.extents.x) + (darkRenderer.bounds.extents.x >= coloredRenderer.bounds.extents.x ? darkRenderer.bounds.extents.x : coloredRenderer.bounds.extents.x);
        float maxX = (areaLimits.transform.position.x + areaLimits.offset.x + areaLimits.bounds.extents.x) - (darkRenderer.bounds.extents.x >= coloredRenderer.bounds.extents.x ? darkRenderer.bounds.extents.x : coloredRenderer.bounds.extents.x);
        float minY = (areaLimits.transform.position.y + areaLimits.offset.y - areaLimits.bounds.extents.y) + (darkRenderer.bounds.extents.y >= coloredRenderer.bounds.extents.y ? darkRenderer.bounds.extents.y : coloredRenderer.bounds.extents.y);
        float maxY = (areaLimits.transform.position.y + areaLimits.offset.y + areaLimits.bounds.extents.y) - (darkRenderer.bounds.extents.y >= coloredRenderer.bounds.extents.y ? darkRenderer.bounds.extents.y : coloredRenderer.bounds.extents.y);


        if (Vector3.Distance(transform.position, targetPos) < MIN_TARGET_DISTANCE && !isIdle) {
            darkAnimator.SetBool("IsIdle", true);
            coloredAnimator.SetBool("IsIdle", true);
            darkAnimator.SetBool("IsWalking", false);
            coloredAnimator.SetBool("IsWalking", false);
            startIdleTime = Time.timeSinceLevelLoad;
            isIdle = true;
        }

        transform.position = new Vector3(
    Mathf.Clamp(nextPos.x, minX, maxX),
    Mathf.Clamp(nextPos.y, minY, maxY),
    transform.position.z);

        return targetPos;
    }

    private void OnCollisionStay2D(Collision2D collision) {
        ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];
        collision.GetContacts(contacts);
        for (int i = 0; i < collision.contactCount; i++) {
            if(contacts[i].otherCollider.gameObject.layer == lightConeMask) {
                actualSpeed = Mathf.Clamp(actualSpeed - flashlightSpeedDecrease, 0, normalSpeed);
                if (!isBeingLit) {

                    isBeingLit = true;
                }
                
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





