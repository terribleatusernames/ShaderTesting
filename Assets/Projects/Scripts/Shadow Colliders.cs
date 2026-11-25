using UnityEngine;

public class ShadowColliders : MonoBehaviour
{
    public Transform lightTransform;

    private MeshFilter filter;

    public LayerMask layerFilter;

    public GameObject polygonColliderHolder;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        filter = GetComponent<MeshFilter>();

        layerFilter = LayerMask.GetMask("Floor");

        var mesh = filter.mesh;

        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            Vector3 firstPoint = transform.TransformPoint(mesh.vertices[mesh.triangles[i]]);
            Vector3 secondPoint = transform.TransformPoint(mesh.vertices[mesh.triangles[i + 1]]);
            Vector3 thirdPoint = transform.TransformPoint(mesh.vertices[mesh.triangles[i + 2]]);

            Vector2[] points = new Vector2[] { CastRay(lightTransform.position, firstPoint), CastRay(lightTransform.position, secondPoint), CastRay(lightTransform.position, thirdPoint) };

            PolygonCollider2D col = polygonColliderHolder.AddComponent<PolygonCollider2D>();
            col.SetPath(0, points);
        }
    }

    private Vector3 CastRay(Vector3 origin, Vector3 vertex)
    {
        Ray ray = new Ray(origin, (vertex - origin).normalized);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, layerFilter))
        {
            return hit.point;
        }

        return Vector3.zero;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
