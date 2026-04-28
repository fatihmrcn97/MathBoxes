using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class JellyCubeMotion : MonoBehaviour
{
    [SerializeField] private float spring = 34f;
    [SerializeField] private float damping = 9f;
    [SerializeField] private float scaleFollow = 14f;
    [SerializeField] private float maxWobble = 0.18f;
    [SerializeField] private float maxVelocityStretch = 0.12f;
    [SerializeField] private float velocityStretch = 0.015f;
    [SerializeField] private float spawnKick = -6f;
    [SerializeField] private float launchKick = 8f;
    [SerializeField] private float impactKick = -7f;

    private Rigidbody cubeRigidbody;
    private JellyCubeMeshDeformer meshDeformer;
    private Vector3 baseScale;
    private float wobble;
    private float wobbleVelocity;

    private void Awake()
    {
        cubeRigidbody = GetComponent<Rigidbody>();
        meshDeformer = GetComponent<JellyCubeMeshDeformer>();
        baseScale = transform.localScale;
    }

    private void OnEnable()
    {
        transform.localScale = baseScale;
        wobble = 0f;
        wobbleVelocity = 0f;
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        wobbleVelocity -= wobble * spring * deltaTime;
        wobbleVelocity *= Mathf.Exp(-damping * deltaTime);
        wobble += wobbleVelocity * deltaTime;

        float jelly = Mathf.Clamp(wobble, -maxWobble, maxWobble);
        float stretch = Mathf.Clamp(cubeRigidbody.linearVelocity.magnitude * velocityStretch, 0f, maxVelocityStretch);

        Vector3 jellyScale = new Vector3(
            1f - jelly * 0.45f,
            1f + jelly - stretch * 0.35f,
            1f - jelly * 0.45f + stretch
        );

        Vector3 targetScale = Vector3.Scale(baseScale, jellyScale);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, scaleFollow * deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        float strength = Mathf.Clamp01(collision.relativeVelocity.magnitude / 6f);
        PlayImpact(strength);
    }

    public void PlaySpawn()
    {
        wobbleVelocity += spawnKick;
        AddMeshImpulse(-0.45f);
    }

    public void PlayLaunch()
    {
        wobbleVelocity += launchKick;
        AddMeshImpulse(0.8f);
    }

    public void PlayImpact(float strength)
    {
        wobbleVelocity += impactKick * Mathf.Clamp01(strength);
        AddMeshImpulse(-strength);
    }

    private void AddMeshImpulse(float strength)
    {
        if (meshDeformer == null)
            meshDeformer = GetComponent<JellyCubeMeshDeformer>();

        if (meshDeformer != null)
            meshDeformer.AddImpulse(strength);
    }
}
