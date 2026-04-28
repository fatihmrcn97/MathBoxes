using System.Collections.Generic;
using UnityEngine;


public class CubeSpawner : MonoBehaviour
{
    public static CubeSpawner instance;

    Queue<Cube> cubesQueue = new Queue<Cube>();

    [SerializeField] private int cubesQueueCapacity = 20;
    [SerializeField] private bool autoQueueGrow= true;

    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private Color[] cubeColors;
    [SerializeField] private Material[] cubeMaterials;

    [HideInInspector] public int maxCubeNumber; // 2power 12 = 4096

    private int maxPower = 12;

    private Vector3 defaultSpawnPosition;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        defaultSpawnPosition = transform.position;
        maxCubeNumber = (int)Mathf.Pow(2, maxPower);

        InitializeCubeQueue();
    }

    private void InitializeCubeQueue()
    {
        for (int i = 0; i < cubesQueueCapacity; i++)
        {
            AddCubeToQueue();
        }
    }

    private void AddCubeToQueue()
    {
        Cube cube = Instantiate(cubePrefab, defaultSpawnPosition, Quaternion.identity, transform).GetComponent<Cube>();
        cube.gameObject.SetActive(false);
        cube.IsMainCube = false;
        cubesQueue.Enqueue(cube);
    }

    public int GenerateRandomNumber()
    {
        return (int)Mathf.Pow(2, UnityEngine.Random .Range(1, 6));
    }

    private Color GetColor(int number)
    {
        if (cubeColors == null || cubeColors.Length == 0)
            return Color.white;

        return cubeColors[GetVisualIndex(number) % cubeColors.Length]; // we are getting the top number of value for example :  2^3 => 3. 
    }

    private Material GetMaterial(int number)
    {
        if (cubeMaterials == null || cubeMaterials.Length == 0)
            return null;

        return cubeMaterials[GetVisualIndex(number) % cubeMaterials.Length];
    }

    private int GetVisualIndex(int number)
    {
        return Mathf.Max(0, (int)(Mathf.Log(number) / Mathf.Log(2)) - 1);
    }

    public Cube Spawn(int number, Vector3 position)
    {
        if (cubesQueue.Count == 0)
        {
            if (autoQueueGrow)
            {
                cubesQueueCapacity++;
                AddCubeToQueue();
            }
            else
            {
                Debug.LogError("[cubes queue] : no more cubes available in the pull");
                return null;
            }
        }
        Cube cube = cubesQueue.Dequeue();
        cube.transform.position = position;
        cube.gameObject.SetActive(true);
        cube.SetNumber(number);
        cube.SetVisual(GetColor(number), GetMaterial(number));
        return cube;
    }
    public Cube SpawnRandom()
    {
        return Spawn(GenerateRandomNumber(), defaultSpawnPosition);
    }
    public void Destroy(Cube cube)
    {
        cube.CubeRigidbody.linearVelocity = Vector3.zero;
        cube.CubeRigidbody.angularVelocity= Vector3.zero;
        cube.transform.rotation = Quaternion.identity;
        cube.IsMainCube = false;
        cube.gameObject.SetActive(false);
        cubesQueue.Enqueue(cube);
    }
}
