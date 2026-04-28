using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class JellyCubeMeshDeformer : MonoBehaviour
{
    [SerializeField] private int subdivisions = 8;
    [SerializeField] private float idleAmplitude = 0.012f;
    [SerializeField] private float impulseAmplitude = 0.055f;
    [SerializeField] private float waveSpeed = 7f;
    [SerializeField] private float waveFrequency = 3.4f;
    [SerializeField] private float impulseDecay = 5.5f;
    [SerializeField] private float velocityInfluence = 0.006f;

    private MeshFilter meshFilter;
    private Mesh runtimeMesh;
    private Vector3[] baseVertices;
    private Vector3[] baseNormals;
    private Vector3[] deformedVertices;
    private float[] vertexMasks;
    private Bounds baseBounds;
    private float impulse;
    private float phase;
    private Rigidbody cubeRigidbody;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        cubeRigidbody = GetComponent<Rigidbody>();
        phase = Mathf.Abs(GetInstanceID() * 0.137f);
        BuildRuntimeMesh();
    }

    private void OnEnable()
    {
        impulse = 0f;
    }

    private void Update()
    {
        if (runtimeMesh == null || baseVertices == null)
            return;

        impulse = Mathf.MoveTowards(impulse, 0f, impulseDecay * Time.deltaTime);

        float time = Time.time * waveSpeed + phase;
        float velocityAmount = cubeRigidbody == null ? 0f : cubeRigidbody.linearVelocity.magnitude * velocityInfluence;
        float amplitude = idleAmplitude + Mathf.Abs(impulse) * impulseAmplitude + velocityAmount;

        for (int i = 0; i < baseVertices.Length; i++)
        {
            Vector3 vertex = baseVertices[i];
            Vector3 normal = baseNormals[i];
            Vector3 normalized = NormalizeInBounds(vertex);

            float waveA = Mathf.Sin((normalized.x * 1.7f + normalized.y * 0.8f + normalized.z * 1.2f) * waveFrequency + time);
            float waveB = Mathf.Sin((normalized.x * -0.9f + normalized.y * 1.5f + normalized.z * 0.7f) * (waveFrequency * 1.35f) - time * 1.18f);
            float offset = (waveA * 0.65f + waveB * 0.35f) * amplitude * vertexMasks[i];

            deformedVertices[i] = vertex + normal * offset;
        }

        runtimeMesh.vertices = deformedVertices;
        runtimeMesh.RecalculateNormals();
    }

    public void AddImpulse(float strength)
    {
        impulse = Mathf.Clamp(impulse + strength, -1f, 1f);
    }

    private void BuildRuntimeMesh()
    {
        Mesh sourceMesh = meshFilter.sharedMesh;
        baseBounds = sourceMesh == null ? new Bounds(Vector3.zero, Vector3.one) : sourceMesh.bounds;

        int steps = Mathf.Clamp(subdivisions, 2, 16);
        int verticesPerFace = (steps + 1) * (steps + 1);
        int quadsPerFace = steps * steps;

        baseVertices = new Vector3[verticesPerFace * 6];
        baseNormals = new Vector3[baseVertices.Length];
        deformedVertices = new Vector3[baseVertices.Length];
        vertexMasks = new float[baseVertices.Length];
        Vector2[] uvs = new Vector2[baseVertices.Length];
        int[] triangles = new int[quadsPerFace * 6 * 6];

        int vertexIndex = 0;
        int triangleIndex = 0;

        AddFace(Vector3.right, Vector3.forward, Vector3.up, steps, ref vertexIndex, ref triangleIndex, uvs, triangles);
        AddFace(Vector3.left, Vector3.back, Vector3.up, steps, ref vertexIndex, ref triangleIndex, uvs, triangles);
        AddFace(Vector3.up, Vector3.right, Vector3.forward, steps, ref vertexIndex, ref triangleIndex, uvs, triangles);
        AddFace(Vector3.down, Vector3.right, Vector3.back, steps, ref vertexIndex, ref triangleIndex, uvs, triangles);
        AddFace(Vector3.forward, Vector3.left, Vector3.up, steps, ref vertexIndex, ref triangleIndex, uvs, triangles);
        AddFace(Vector3.back, Vector3.right, Vector3.up, steps, ref vertexIndex, ref triangleIndex, uvs, triangles);

        runtimeMesh = new Mesh();
        runtimeMesh.name = "Runtime Jelly Cube Mesh";
        runtimeMesh.MarkDynamic();
        runtimeMesh.vertices = baseVertices;
        runtimeMesh.normals = baseNormals;
        runtimeMesh.uv = uvs;
        runtimeMesh.triangles = triangles;
        runtimeMesh.bounds = ExpandBounds(baseBounds, impulseAmplitude + idleAmplitude + velocityInfluence * 10f);

        meshFilter.sharedMesh = runtimeMesh;
    }

    private void AddFace(Vector3 normal, Vector3 tangent, Vector3 bitangent, int steps, ref int vertexIndex, ref int triangleIndex, Vector2[] uvs, int[] triangles)
    {
        int faceStart = vertexIndex;

        for (int y = 0; y <= steps; y++)
        {
            for (int x = 0; x <= steps; x++)
            {
                float u = x / (float)steps;
                float v = y / (float)steps;
                Vector3 local = normal * 0.5f + tangent * (u - 0.5f) + bitangent * (v - 0.5f);

                baseVertices[vertexIndex] = ScaleToBounds(local);
                baseNormals[vertexIndex] = normal;
                uvs[vertexIndex] = new Vector2(u, v);
                vertexMasks[vertexIndex] = GetFaceCenterMask(u, v);
                vertexIndex++;
            }
        }

        for (int y = 0; y < steps; y++)
        {
            for (int x = 0; x < steps; x++)
            {
                int a = faceStart + y * (steps + 1) + x;
                int b = a + 1;
                int c = a + steps + 1;
                int d = c + 1;

                triangles[triangleIndex++] = a;
                triangles[triangleIndex++] = c;
                triangles[triangleIndex++] = b;
                triangles[triangleIndex++] = b;
                triangles[triangleIndex++] = c;
                triangles[triangleIndex++] = d;
            }
        }
    }

    private Vector3 ScaleToBounds(Vector3 normalizedVertex)
    {
        return new Vector3(
            baseBounds.center.x + normalizedVertex.x * baseBounds.size.x,
            baseBounds.center.y + normalizedVertex.y * baseBounds.size.y,
            baseBounds.center.z + normalizedVertex.z * baseBounds.size.z
        );
    }

    private Vector3 NormalizeInBounds(Vector3 vertex)
    {
        return new Vector3(
            baseBounds.size.x <= 0f ? 0f : (vertex.x - baseBounds.center.x) / baseBounds.size.x,
            baseBounds.size.y <= 0f ? 0f : (vertex.y - baseBounds.center.y) / baseBounds.size.y,
            baseBounds.size.z <= 0f ? 0f : (vertex.z - baseBounds.center.z) / baseBounds.size.z
        );
    }

    private Bounds ExpandBounds(Bounds bounds, float amount)
    {
        bounds.Expand(amount * 2f);
        return bounds;
    }

    private float GetFaceCenterMask(float u, float v)
    {
        float edgeDistance = Mathf.Min(Mathf.Min(u, 1f - u), Mathf.Min(v, 1f - v));
        return Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(edgeDistance / 0.22f));
    }

    private void OnDestroy()
    {
        if (runtimeMesh != null)
            Destroy(runtimeMesh);
    }
}
