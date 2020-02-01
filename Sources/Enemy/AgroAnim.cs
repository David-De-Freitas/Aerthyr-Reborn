using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgroAnim : MonoBehaviour
{
    Animator animator;
    SpriteRenderer spriteRenderer;
    Loulpe loulpe;
    private void Start()
    {
        animator = GetComponent<Animator>();
        loulpe = gameObject.GetComponentInParent<Loulpe>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }


    public void PlayAgroAnim()
    {
        animator.Play("Pop");
        spriteRenderer.enabled = true;

    }

    public void StopAgroAnim()
    {
        animator.Play("idlePop");
        loulpe.LaunchAttack();
    }


    public void StopAgroAnim(bool attack)
    {
        animator.Play("idlePop");
        if (attack)
        {
            loulpe.LaunchAttack();
        }
    }

}
