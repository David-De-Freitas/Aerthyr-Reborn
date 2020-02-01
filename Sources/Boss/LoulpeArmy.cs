using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoulpeArmy : MonoBehaviour
{
    [Header("Global")]
    public bool active = false;
    public float directionX = 1;
    public float speedLoulpe;
    public bool lookingRight = true;
    [Space]
    [Header("Attack")]
    [SerializeField] private float damage;
    [SerializeField] private Vector2 knockbackForce;
    [Space]
    [Header("Feedback")]
    public ParticleSystem leftParticle;
    public ParticleSystem rightParticle;

    Transform loulpeGroup;
    Transform[] loulpes;
    Collider2D attackBox;

    ContactFilter2D hitFilter;

    float timeToCheckNewHit;
    float currentTimeToCheckNewHit;

    bool leftParticleBlocked = false;
    bool rightParticleBlocked = false;

    // Use this for initialization
    void Start()
    {
        attackBox = GetComponentInChildren<BoxCollider2D>();

        timeToCheckNewHit = 1f;

        loulpeGroup = transform.GetChild(0);
        int nbLoulpe = transform.GetChild(0).childCount;

        loulpes = new Transform[nbLoulpe];
        for (int i = 0; i < nbLoulpe; i++)
        {
            loulpes[i] = transform.GetChild(0).GetChild(i);
        }

        foreach (Transform loulpe in loulpes)
        {
            loulpe.GetComponent<Animator>().Update(Random.value);
        }
        leftParticle.Stop();
        rightParticle.Stop();

        damage = Mathf.CeilToInt(damage * GameManager.Singleton.gameData.GetEnemiesStatsFactor());
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            Vector3 move = Vector3.right * speedLoulpe * Time.deltaTime * directionX;

            loulpeGroup.transform.Translate(move);

            CheckHitUpdate();

            for (int i = 0; i < loulpes.Length; i++)
            {
                Transform loulpe = loulpes[i];
                if (lookingRight)
                {
                    if (loulpe.transform.position.x > rightParticle.transform.position.x)
                    {
                        loulpe.GetComponent<SpriteRenderer>().enabled = false;
                        if (i == loulpes.Length - 1)
                        {
                            active = false;
                            rightParticle.Stop();
                        }
                        else if (i == 0)
                        {
                            rightParticle.Play();
                        }
                    }
                    else if (loulpe.transform.position.x > leftParticle.transform.position.x)
                    {
                        loulpe.GetComponent<SpriteRenderer>().enabled = true;
                        if (i == loulpes.Length - 1)
                        {
                            leftParticle.Stop();
                            leftParticleBlocked = true;
                        }
                        else if (i == 0 && !leftParticleBlocked)
                        {
                            leftParticle.Play();
                        }
                    }
                }
                else
                {
                    if (loulpe.transform.position.x < leftParticle.transform.position.x)
                    {
                        loulpe.GetComponent<SpriteRenderer>().enabled = false;
                        if (i == loulpes.Length - 1)
                        {
                            active = false;
                            leftParticle.Stop();
                        }
                        else if (i == 0)
                        {
                            leftParticle.Play();
                        }
                    }
                    else if (loulpe.transform.position.x < rightParticle.transform.position.x)
                    {
                        loulpe.GetComponent<SpriteRenderer>().enabled = true;
                        if (i == loulpes.Length - 1)
                        {
                            rightParticle.Stop();
                            rightParticleBlocked = true;
                        }
                        else if (i == 0 && !rightParticleBlocked)
                        {
                            rightParticle.Play();
                        }
                    }
                }
            }
        }
    }

    void CheckHitUpdate()
    {
        if (currentTimeToCheckNewHit <= 0f)
        {
            CheckHit();
        }
        else
        {
            currentTimeToCheckNewHit -= Time.deltaTime;
        }
    }

    void CheckHit()
    {
        Collider2D[] collider2Ds = new Collider2D[200];

        Physics2D.OverlapCollider(attackBox, hitFilter, collider2Ds);
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider != null)
            {
                Player playerHit = collider.GetComponent<Player>();

                if (playerHit != null)
                {
                    int factorMove = (lookingRight) ? 1 : -1;
                    Vector2 force = knockbackForce;
                    force.x *= factorMove;

                    playerHit.SetOppenentAttackDir(force.x);
                    playerHit.TakeDamage(damage, force);

                    currentTimeToCheckNewHit = timeToCheckNewHit;
                    break;
                }
            }
        }
    }


    public void ActiveLoulpeArmy(float BossScaleX)
    {
        active = true;
        directionX = BossScaleX;

        if (BossScaleX == 1)
        {
            Vector3 newPos = loulpeGroup.transform.position;
            newPos.x = leftParticle.transform.position.x;
            loulpeGroup.transform.position = newPos;
            lookingRight = true;
            Vector3 scale = Vector3.one;

            loulpeGroup.transform.localScale = scale;
        }
        else
        {
            Vector3 newPos = loulpeGroup.transform.position;
            newPos.x = rightParticle.transform.position.x;
            loulpeGroup.transform.position = newPos;

            lookingRight = false;

            Vector3 scale = Vector3.one;
            scale.x = -1;
            loulpeGroup.transform.localScale = scale;
        }
    }

    public void DisableLoulpeArmy()
    {
        active = false;
    }
}

