using UnityEngine;

public class Batteries : MonoBehaviour {
    [SerializeField]
    float detectionDistance = 1f;
    [SerializeField]
    float respawnTime = 5f;
    [SerializeField]
    float TransitionTime = 1.5f;
    [SerializeField]
    SpriteRenderer darkRenderer;
    [SerializeField]
    SpriteRenderer coloredRenderer;


    bool IsActive = true;
    bool InTransition = false;
    float pickupTime;
    Material darkMaterial;
    Material coloredMaterial;
    float effectTime = 0f;
    Transform playerTransform;
    // Start is called before the first frame update
    void Start() {
        coloredRenderer.material = darkMaterial = darkRenderer.material;
        darkMaterial.SetFloat("DissolveIntensity", 0f);
        playerTransform = MainCharacterControls.mainCharacter.transform;
    }

    // Update is called once per frame
    void Update() {
        Debug.Log(IsActive + " ,playerTransform: " + Vector3.Distance(playerTransform.position, transform.position));
        if (Vector3.Distance(playerTransform.position, transform.position) <= detectionDistance && IsActive) {
            MainCharacterControls.mainCharacter.CollectedBattery();
            IsActive = false;
            InTransition = true;
            effectTime = 0f;
            pickupTime = Time.timeSinceLevelLoad;
        }
        else if (!IsActive && Time.timeSinceLevelLoad >= pickupTime + respawnTime) {
            InTransition = true;
            IsActive = true;
            effectTime = 1f;
        }
        if (InTransition) {
            effectTime = Mathf.Clamp(effectTime + (IsActive ? -1 : 1) * Time.deltaTime / TransitionTime, 0f, 1f);
            darkMaterial.SetFloat("DissolveIntensity", effectTime);
            InTransition = effectTime != 1f && effectTime != 0f;
        }


    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, detectionDistance);
    }
}
