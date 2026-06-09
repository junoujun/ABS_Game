using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class HomePlate : MonoBehaviour
{
    [Header("Home Plate Size (inch)")]
    [SerializeField] private float widthInch = 17f;
    [SerializeField] private float rectDepthInch = 8.5f;
    [SerializeField] private float triangleDepthInch = 8.5f;
    [SerializeField] private float thicknessInch = 1f;

    private const float InchToMeter = 0.0254f;

    private void Awake()
    {
        CreateMesh();
    }

    private void OnValidate()
    {
        CreateMesh();
    }

    private void CreateMesh()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null) return;

        float width = widthInch * InchToMeter;
        float rectDepth = rectDepthInch * InchToMeter;
        float triDepth = triangleDepthInch * InchToMeter;
        float thickness = thicknessInch * InchToMeter;

        float halfW = width / 2f;
        float halfT = thickness / 2f;
        float totalDepth = rectDepth + triDepth;
        float centerOffsetZ = totalDepth / 2f;

        Vector3[] top =
        {
            new Vector3(-halfW, halfT, 0f - centerOffsetZ),
            new Vector3( halfW, halfT, 0f - centerOffsetZ),
            new Vector3( halfW, halfT, rectDepth - centerOffsetZ),
            new Vector3( 0f,    halfT, totalDepth - centerOffsetZ),
            new Vector3(-halfW, halfT, rectDepth - centerOffsetZ)
        };

        Vector3[] bottom =
        {
            new Vector3(-halfW, -halfT, 0f - centerOffsetZ),
            new Vector3( halfW, -halfT, 0f - centerOffsetZ),
            new Vector3( halfW, -halfT, rectDepth - centerOffsetZ),
            new Vector3( 0f,    -halfT, totalDepth - centerOffsetZ),
            new Vector3(-halfW, -halfT, rectDepth - centerOffsetZ)
        };

        Vector3[] vertices =
        {
            top[0], top[1], top[2], top[3], top[4],
            bottom[0], bottom[1], bottom[2], bottom[3], bottom[4]
        };

        int[] triangles =
        {
            // 윗면
            0, 1, 2,
            0, 2, 4,
            4, 2, 3,

            // 아랫면
            5, 7, 6,
            5, 9, 7,
            9, 8, 7,

            // 옆면
            0, 5, 1,
            1, 5, 6,

            1, 6, 2,
            2, 6, 7,

            2, 7, 3,
            3, 7, 8,

            3, 8, 4,
            4, 8, 9,

            4, 9, 0,
            0, 9, 5
        };

        Mesh mesh = new Mesh();
        mesh.name = "HomePlateMesh";
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.sharedMesh = mesh;
    }
}