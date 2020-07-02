using UnityEngine;

public class Collectibles : MonoBehaviour {
    enum CollectibleTypes { Battery, Key, Health }

    [SerializeField]
    CollectibleTypes collectibleType = CollectibleTypes.Battery;
    [SerializeField]
    Sprite batterySprite;
    [SerializeField]
    Sprite keySprite;
    [SerializeField]
    Sprite healthSprite;
    [SerializeField]
    float detectionDistance = 1f;
    [SerializeField]
    bool Respawn;
    [SerializeField]
    float respawnTime = 5f;
    [SerializeField]
    float TransitionTime = 1.5f;
    [SerializeField]
    int healthRegain = 3;


    bool IsActive = true;
    bool InTransition = false;
    float pickupTime;
    Material material;
    float effectTime = 0f;
    Transform playerTransform;
    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        switch (collectibleType) {
            case CollectibleTypes.Battery:
                spriteRenderer.sprite = batterySprite;
                break;
            case CollectibleTypes.Key:
                spriteRenderer.sprite = keySprite;
                break;
            case CollectibleTypes.Health:
                spriteRenderer.sprite = healthSprite;
                break;
            default:
                spriteRenderer.sprite = batterySprite;
                break;
        }

        material = spriteRenderer.material;
        material.SetFloat("DissolveIntensity", 0f);
        playerTransform = MainCharacterControls.mainCharacter.transform;
    }

    // Update is called once per frame
    void Update() {
        if (Vector3.Distance(playerTransform.position, transform.position) <= detectionDistance && IsActive) {
            switch (collectibleType) {
                case CollectibleTypes.Battery:
                    MainCharacterControls.mainCharacter.CollectedBattery();
                    break;
                case CollectibleTypes.Key:
                    GameManager.gameManager.changeKeys(1);
                    break;
                case CollectibleTypes.Health:
                    GameManager.gameManager.changeHealth(healthRegain);
                    break;
                default:
                    break;
            }

            IsActive = false;
            InTransition = true;
            effectTime = 0f;
            pickupTime = Time.timeSinceLevelLoad;
            
        }
        else if (!IsActive && Respawn && Time.timeSinceLevelLoad >= pickupTime + respawnTime) {
            InTransition = true;
            IsActive = true;
            effectTime = 1f;
        }
        if (InTransition) {
            effectTime = Mathf.Clamp(effectTime + (IsActive ? -1 : 1) * Time.deltaTime / TransitionTime, 0f, 1f);
            material.SetFloat("DissolveIntensity", effectTime);
            InTransition = effectTime != 1f && effectTime != 0f;
            gameObject.SetActive(Respawn || InTransition);
        }


    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, detectionDistance);
    }
}
