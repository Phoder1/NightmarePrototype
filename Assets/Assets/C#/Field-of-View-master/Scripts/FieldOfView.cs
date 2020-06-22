using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour {

	public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

	public LayerMask obstacleMask;

	[HideInInspector]
	public List<Transform> visibleTargets = new List<Transform>();

	public float meshResolution;
	public int edgeResolveIterations;
    public float edgeDstThreshold;
    [Tooltip("How much ground to reveal below the character eyes.")]
    [SerializeField]
    float edgeRevealDst = 0.1f;
    [Tooltip("Eyes height compared to the center of the character.")]
    [SerializeField]
    float eyesHeight = 0.6f;

	public MeshFilter viewMeshFilter;
	Mesh viewMesh;

    void Start() {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
    }

	internal void DrawFieldOfView() {
		int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
		float stepAngleSize = viewAngle / stepCount;
		List<ViewCastInfo> viewPoints = new List<ViewCastInfo> ();
		ViewCastInfo oldViewCast = new ViewCastInfo ();
		for (int i = 0; i <= stepCount; i++) {
            float angle = transform.eulerAngles.z - viewAngle / 2 + stepAngleSize * i;
            //Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * viewRadius, Color.red);
			ViewCastInfo newViewCast = ViewCast (angle);

            if (i > 0) {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded)) {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA.point != Vector3.zero) {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB.point != Vector3.zero) {
                        viewPoints.Add(edge.pointB);
                    }
                }

            }
            


            viewPoints.Add(newViewCast);
			oldViewCast = newViewCast;
		}

		int vertexCount = viewPoints.Count + 1;
		Vector3[] vertices = new Vector3[vertexCount];
		int[] triangles = new int[(vertexCount-2) * 3];

		vertices [0] = Vector3.zero;
		for (int i = 0; i < vertexCount - 1; i++) {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i].point);
            //Debug.DrawLine(viewPoints[i].point, viewPoints[i].normal);
            if (vertices[i + 1].y <= eyesHeight && viewPoints[i].normal == Vector2.up) {
                vertices[i + 1] += Vector3.down * edgeRevealDst;
            }
            else if (viewPoints[i].normal.x == -1 || viewPoints[i].normal.x == 1) {
                Debug.DrawLine(viewPoints[i].point, viewPoints[i].point + Vector3.Normalize(Quaternion.Euler(0f, 0f, Vector3.Angle(viewPoints[i].normal,-vertices[i+1])) * viewPoints[i].normal));
                //Debug.Log(Vector3.Angle(vertices[i + 1], viewPoints[i].normal));
                vertices[i + 1] -= (Vector3)viewPoints[i].normal * Mathf.Lerp(edgeRevealDst, 0f, Vector3.Angle(viewPoints[i].normal, -vertices[i + 1]) / 90);
            }



            //vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2) {
				triangles [i * 3] = 0;
				triangles [i * 3 + 1] = i + 1;
				triangles [i * 3 + 2] = i + 2;
			}
		}

		viewMesh.Clear ();

		viewMesh.vertices = vertices;
		viewMesh.triangles = triangles;
		viewMesh.RecalculateNormals ();
	}


	EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast) {
		float minAngle = minViewCast.angle;
		float maxAngle = maxViewCast.angle;
		ViewCastInfo minPoint = new ViewCastInfo (false,Vector3.zero,0f,0f,Vector3.zero);
		ViewCastInfo maxPoint = new ViewCastInfo(false, Vector3.zero, 0f, 0f, Vector3.zero);

        for (int i = 0; i < edgeResolveIterations; i++) {
			float angle = (minAngle + maxAngle) / 2;
			ViewCastInfo newViewCast = ViewCast (angle);

			bool edgeDstThresholdExceeded = Mathf.Abs (minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
			if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded) {
				minAngle = angle;
				minPoint = newViewCast;
			} else {
				maxAngle = angle;
				maxPoint = newViewCast;
			}
		}

		return new EdgeInfo (minPoint, maxPoint);
	}




    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
        if (!angleIsGlobal) {
            angleInDegrees += transform.eulerAngles.z;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
    }

    ViewCastInfo ViewCast(float globalAngle) {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit2D hit;

        if (hit = Physics2D.Raycast(transform.position, dir, viewRadius, obstacleMask)) {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle, hit.normal);
        }
        else {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle, hit.normal);
        }
    }

    public struct ViewCastInfo {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;
        public Vector2 normal;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle, Vector2 _normal) {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
            normal = _normal;
        }
    }

	public struct EdgeInfo {
		public ViewCastInfo  pointA;
		public ViewCastInfo pointB;

		public EdgeInfo(ViewCastInfo _pointA, ViewCastInfo _pointB) {
			pointA = _pointA;
			pointB = _pointB;
		}
	}

}