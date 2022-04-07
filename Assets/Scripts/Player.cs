using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float pushForce;
    [SerializeField] private float cubeMaxPosX;
    [Space]
    [SerializeField] private TouchSilder touchSlider;

   private Cube mainCube;

    private bool isPointerDown;
    private Vector3 cubePos;


    private bool canMove;


    private void Start()
    {
        // Spawn new cube
        SpawnCube();
        canMove = true;
        // listen to slider events

        touchSlider.OnPointerDownEvent += OnPointerDown;
        touchSlider.OnPointerDragEvent += OnPointerDrag;
        touchSlider.OnPointerUpEvent += OnPointerUp;
    }

    private void OnPointerDown()
    {
        isPointerDown = true;
    }
    private void OnPointerDrag(float xMovement)
    {
        if (isPointerDown)
        {
            cubePos = mainCube.transform.position;
            cubePos.x = xMovement*cubeMaxPosX;
        }
    }
    private void OnPointerUp()
    {
        if (isPointerDown && canMove)
        {
            isPointerDown = false;
            canMove = false;
            //push the cube
            mainCube.CubeRigidbody.AddForce(Vector3.forward * pushForce, ForceMode.Impulse);

            //spawn a new cube aftrer 0.3 sec;
            Invoke("SpawnNewCube", 0.3f);
        }

    }

    private void SpawnNewCube()
    {

        mainCube.IsMainCube = false;
        canMove = true;
        SpawnCube();
    }

    private void OnDestroy()
    {
        //remove listeners
        touchSlider.OnPointerDownEvent -= OnPointerDown;
        touchSlider.OnPointerDragEvent -= OnPointerDrag;
        touchSlider.OnPointerUpEvent -= OnPointerUp;
    }


    private void Update()
    {
        if (isPointerDown)
        {
            mainCube.transform.position = Vector3.Lerp(mainCube.transform.position, cubePos, moveSpeed * Time.deltaTime);
        }
    }

    private void SpawnCube()
    {
        mainCube = CubeSpawner.instance.SpawnRandom();
        mainCube.IsMainCube = true;

        //reset cubes Pos
        cubePos = mainCube.transform.position;
    }
}
