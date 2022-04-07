using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX : MonoBehaviour
{
    public static FX instance;

    [SerializeField] private ParticleSystem cubeExplosionFX;

    ParticleSystem.MainModule cubeExplosoionFxMainModule;

    private void Awake()
    {if (instance == null)
            instance = this;
    }

    private void Start()
    {
        cubeExplosoionFxMainModule = cubeExplosionFX.main;

    }

    public void PlayCubeExplosionFx(Vector3 position,Color color)
    {
        cubeExplosoionFxMainModule.startColor = new ParticleSystem.MinMaxGradient(color);
        cubeExplosionFX.transform.position = position;
        cubeExplosionFX.Play();
    }
}
