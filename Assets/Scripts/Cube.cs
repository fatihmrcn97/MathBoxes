using UnityEngine;
using TMPro;
public class Cube : MonoBehaviour
{
    static int staticID = 0;

    [SerializeField] private TMP_Text[] numbersText;

    [HideInInspector] public int CubeID;

    [HideInInspector] public Color CubeColor;
    [HideInInspector] public int CubeNumber;
    [HideInInspector] public Rigidbody CubeRigidbody;
    [HideInInspector] public bool IsMainCube;


    private MeshRenderer cubeMeshRenderer;
    private JellyCubeMotion jellyMotion;
    private JellyCubeMeshDeformer meshDeformer;
    private Material runtimeMaterial;

    private void Awake()
    {
        CubeID = staticID++;
        cubeMeshRenderer = GetComponent<MeshRenderer>();
        CubeRigidbody = GetComponent<Rigidbody>();
        meshDeformer = GetComponent<JellyCubeMeshDeformer>();

        if (meshDeformer == null)
            meshDeformer = gameObject.AddComponent<JellyCubeMeshDeformer>();

        jellyMotion = GetComponent<JellyCubeMotion>();

        if (jellyMotion == null)
            jellyMotion = gameObject.AddComponent<JellyCubeMotion>();
    }


    public void SetColor(Color color)
    {
        SetVisual(color, null);
    }

    public void SetVisual(Color color, Material jellyMaterial)
    {
        CubeColor = color;
        CubeColor.a = 1f;

        if (jellyMaterial == null)
        {
            cubeMeshRenderer.material.color = CubeColor;
        }
        else
        {
            ApplyJellyMaterial(jellyMaterial, CubeColor);
        }

        jellyMotion.PlaySpawn();
    }

    public void SetNumber(int number)
    {
        CubeNumber = number;
        for (int i = 0; i < 6; i++)
        {
            numbersText[i].text = number.ToString();
        }
    }

    public void PlayJellyLaunch()
    {
        jellyMotion.PlayLaunch();
    }

    private void ApplyJellyMaterial(Material sourceMaterial, Color color)
    {
        if (runtimeMaterial == null || runtimeMaterial.shader != sourceMaterial.shader)
        {
            if (runtimeMaterial != null)
                Destroy(runtimeMaterial);

            runtimeMaterial = new Material(sourceMaterial);
            cubeMeshRenderer.material = runtimeMaterial;
        }
        else
        {
            runtimeMaterial.CopyPropertiesFromMaterial(sourceMaterial);
        }

        ApplyJellyColor(runtimeMaterial, color);
    }

    private void ApplyJellyColor(Material material, Color color)
    {
        if (material.HasProperty("_Color"))
            material.SetColor("_Color", color);

        if (material.HasProperty("_Smooth"))
            material.SetFloat("_Smooth", 0.99f);

        if (material.HasProperty("_Specular"))
            material.SetFloat("_Specular", 0.16f);

        if (material.HasProperty("_NormalIntensity"))
            material.SetFloat("_NormalIntensity", 0.16f);

        if (material.HasProperty("_WPOIntensity"))
            material.SetFloat("_WPOIntensity", 0f);

        if (material.HasProperty("_WPOAxisMask"))
            material.SetFloat("_WPOAxisMask", 0f);

        if (material.HasProperty("_AxisFalloffErosion"))
            material.SetFloat("_AxisFalloffErosion", 0.5f);

        if (material.HasProperty("_AxisFalloffSmoothness"))
            material.SetFloat("_AxisFalloffSmoothness", 0.5f);
    }

    private void OnDestroy()
    {
        if (runtimeMaterial != null)
            Destroy(runtimeMaterial);
    }
}
