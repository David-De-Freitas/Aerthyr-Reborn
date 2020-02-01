using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAttackBoss : MonoBehaviour
{
    [SerializeField]
    public ParticleSystem[] attackParticle = new ParticleSystem[4];
    public int[] burstCount = new int[4];

    public void EmitBurst(int nbAttack)
    {
        attackParticle[nbAttack].Emit(burstCount[nbAttack]);
    }

    public void StartEmit(int nbAttack)
    {
        attackParticle[nbAttack].Play();
    }

    public void StopEmit(int nbAttack)
    {
        attackParticle[nbAttack].Stop();
    }

}
