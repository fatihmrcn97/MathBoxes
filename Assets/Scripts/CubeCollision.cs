using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCollision : MonoBehaviour
{
    Cube cube;
    
    private void Awake()
    {
        cube = GetComponent<Cube>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Cube otherCube = collision.gameObject.GetComponent<Cube>();
        //check if contacted with other cube

        if (otherCube != null && cube.CubeID>otherCube.CubeID)
        {
            //check if both have same number
            if (cube.CubeNumber == otherCube.CubeNumber)
            {

                //Coin count ++
                Coin.instance.IncreaseCoin(cube.CubeNumber);

                Vector3 contactPoint = collision.contacts[0].point;

                //check if cubes number less than max number in cube spawner
                if (otherCube.CubeNumber < CubeSpawner.instance.maxCubeNumber)
                {
                    //spawn a new cube 
                    Cube newCube = CubeSpawner.instance.Spawn(cube.CubeNumber * 2, contactPoint + Vector3.up * 1.6f);
                    float pushForce = 2.5f;
                    newCube.CubeRigidbody.AddForce(new Vector3(0, 3f, 1f) * pushForce, ForceMode.Impulse);

                    //add same torque
                    float randomVal = Random.Range(20f, 20f);
                    Vector3 randomDirection = Vector3.one * randomVal;
                    newCube.CubeRigidbody.AddTorque(randomDirection);
                }

                //the explosio should affect surronded cubes too:
                Collider[] surrondedCubes = Physics.OverlapSphere(contactPoint, 2f);
                float explosionForce = 400f;
                float explosionRadius = 1.5f;
                foreach (Collider coll in surrondedCubes)
                {
                    if(coll.attachedRigidbody != null)
                    {
                        coll.attachedRigidbody.AddExplosionForce(explosionForce, contactPoint, explosionRadius);
                    }
                }

                //TODO explosoin FX
                FX.instance.PlayCubeExplosionFx(contactPoint, cube.CubeColor);

                //DESTROY THE OLD CUBES
                CubeSpawner.instance.Destroy(cube);
                CubeSpawner.instance.Destroy(otherCube);
            }
        }

    }
}
