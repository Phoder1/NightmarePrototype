using UnityEngine;

public class Enemies : MonoBehaviour {
    enum States { Patrol, Chase, Stunned, Dead };

    [SerializeField]
    bool IsFlying = false;

    [SerializeField]
    Transform[] targets;

    [SerializeField]
    Transform darkTransform;
    [SerializeField]
    Transform coloredTransform;
    [SerializeField]
    float idleTime = 1f;
    [SerializeField]
    float speed;
    Animator darkAnimator;
    Animator coloredAnimator;
    SpriteRenderer darkRenderer;
    SpriteRenderer coloredRenderer;

    int currentTarget = 0;
    Animator animator;
    const float MIN_TARGET_DISTANCE = 0.1f;
    States enemystate = States.Patrol;
    float time = 0f;
    bool isIdle = false;
    // Start is called before the first frame update
    void Start() {
        darkAnimator = darkTransform.GetComponent<Animator>();
        coloredAnimator = coloredTransform.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
            if (enemystate == States.Patrol) {
            float targetPosY = (IsFlying ? targets[currentTarget].position.y : transform.position.y);
                Vector3 targetPos = new Vector3(targets[currentTarget].position.x, targetPosY, transform.position.z);
                Vector3 nextPos = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                if(transform.position.x - nextPos.x > 0) {
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 180f, transform.rotation.eulerAngles.z);
                }
                else if(transform.position.x - nextPos.x < 0) {
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 0f, transform.rotation.eulerAngles.z);
                }
                
                transform.position = nextPos;

                if (Vector3.Distance(transform.position, targetPos) < MIN_TARGET_DISTANCE && !isIdle) {
                    darkAnimator.SetBool("IsIdle", true);
                    coloredAnimator.SetBool("IsIdle", true);
                    darkAnimator.SetBool("IsWalking", false);
                    coloredAnimator.SetBool("IsWalking", false);
                    time = Time.timeSinceLevelLoad;
                    isIdle = true;

                }
                

                if (isIdle && Time.timeSinceLevelLoad >= time + idleTime) {
                    darkAnimator.SetBool("IsWalking", true);
                    coloredAnimator.SetBool("IsWalking", true);
                    isIdle = false;
                    currentTarget++;
                    if (currentTarget >= targets.Length) {
                        currentTarget = 0;
                    }
                }
            }
            // Other states



        }

    }





