using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class Controller2D : MonoBehaviour {

    public LayerMask obstacleMask;
    [SerializeField]
    LayerMask platformMask;

	const float skinWidth = .03f;
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;
    public float groundCheckDistance = 0.4f;
	float horizontalRaySpacing;
    float verticalRaySpacing;

    BoxCollider2D playerCollider;
    Vector3 initColliderSize = Vector3.one;
    float _colliderHeight = 1f;
    internal float colliderHeight {
        get {
            return _colliderHeight;
        }
        set {
            value = Mathf.Max(value, 0.1f);

            Debug.Log("Size: " + playerCollider.bounds.size + " ,Offset: " + playerCollider.offset);
            playerCollider.offset = new Vector2(playerCollider.offset.x, (value -1)*initColliderSize.y/2    );
            playerCollider.size = new Vector2(playerCollider.size.x, value*initColliderSize.y);
            CalculateRaySpacing();
            _colliderHeight = value;
        }
    }
    RaycastOrigins raycastOrigins;

    internal Vector3 _velocity;
	public CollisionInfo collisions;

	void Start() {
        playerCollider = GetComponent<BoxCollider2D>();
        initColliderSize = playerCollider.size;
        colliderHeight = _colliderHeight;
	}

	public Vector3 Move(Vector3 velocity) {
        _velocity = velocity;
		UpdateRaycastOrigins ();
		collisions.Reset ();

		if (velocity.x != 0) {
			HorizontalCollisions ();
		}
		if (velocity.y != 0) {
			VerticalCollisions ();
		}

		transform.Translate (_velocity,Space.World);
        return _velocity;
	}

	void HorizontalCollisions() {
		float directionX = Mathf.Sign (_velocity.x);
		float rayLength = Mathf.Abs (_velocity.x) + skinWidth;
		
		for (int i = 0; i < horizontalRayCount; i ++) {
			Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, obstacleMask);

			Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength,Color.blue);

			if (hit) {
				_velocity.x = (hit.distance - skinWidth) * directionX;
				rayLength = hit.distance;

				collisions.left = directionX == -1;
				collisions.right = directionX == 1;
			}
		}
	}

	void VerticalCollisions() {
		float directionY = Mathf.Sign (_velocity.y);
        float rayLength = Mathf.Abs(_velocity.y) + skinWidth;
        bool overlap = false;
        LayerMask layers = obstacleMask;
        if ((directionY == -1)) {
            layers = obstacleMask + platformMask;
        }
        for (int i = 0; i < verticalRayCount && !overlap; i ++) {
			Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + _velocity.x);


            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, layers);
            if(hit.collider != null) {
                overlap = hit.collider.OverlapPoint(rayOrigin + Vector2.up*skinWidth);
                //Debug.Log(overlap);
            }
            

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength,overlap? Color.red:Color.blue);

			if (hit && !overlap) {
				_velocity.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}
	}

    public bool IsGrounded() {
        float directionY = -1;
        float rayLength = groundCheckDistance + skinWidth;

        for (int i = 0; i < verticalRayCount; i++) {
            Vector2 rayOrigin = (directionY == -1 ? raycastOrigins.bottomLeft : raycastOrigins.topLeft) + Vector2.up *_velocity.y;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + _velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, obstacleMask + platformMask);

            if (hit) {
                return true;
            }
        }
        return false;
    }

	void UpdateRaycastOrigins() {
		Bounds bounds = playerCollider.bounds;
		bounds.Expand (skinWidth * -2);

		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}

	void CalculateRaySpacing() {
		Bounds bounds = playerCollider.bounds;
		bounds.Expand (skinWidth * -2);

		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	public struct CollisionInfo {
		public bool above, below;
		public bool left, right;

		public void Reset() {
			above = below = false;
			left = right = false;
		}
	}

}
