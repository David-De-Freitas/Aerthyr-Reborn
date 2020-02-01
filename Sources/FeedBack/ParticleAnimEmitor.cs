using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAnimEmitor : MonoBehaviour
{
    new ParticleSystem particleSystem;


    private void Start()
    {
        particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    public void EmitBurst(int count)
    {
        particleSystem.Emit(count);
    }

    public void StartEmit()
    {
        particleSystem.Play();
    }

    public void StopEmit()
    {
        particleSystem.Stop();
    }
}
