using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicBoomBehaviour : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] float lifeTime;
    [Space]
    [SerializeField] float baseDamages;
    [SerializeField] float explosionDamageMultiplier;

    bool hasHit;
    float directionX;
    float currentLifeTime;

    Player player;
    ContactFilter2D contactFilter;
    GameObject children;

    new CircleCollider2D collider;

    // Use this for initialization
    void Start()
    {
        collider = GetComponent<CircleCollider2D>();
        children = transform.GetChild(0).gameObject;

        hasHit = false;
        currentLifeTime = lifeTime;
    }

    // Update is called once per frame
    void Update()
    {
        children.transform.Rotate(Vector3.forward * directionX * -1, rotateSpeed * Time.deltaTime);
        transform.Translate(Vector3.right * directionX * moveSpeed * Time.deltaTime);

        CheckHit();

        UpdateLifeTime();
    }

    void UpdateLifeTime()
    {
        if (currentLifeTime > 0)
        {
            currentLifeTime -= Time.deltaTime;
        }
        else
        {
            Explode();
            Destroy(gameObject);
        }
    }

    void Explode()
    {
        collider.radius = 2.5f;
        hasHit = false;
        baseDamages *= explosionDamageMultiplier;
        moveSpeed = 0f;

        CheckHit();
    }

    void CheckHit()
    {
        if (!hasHit)
        {
            Collider2D[] collider2Ds = new Collider2D[200];

            Physics2D.OverlapCollider(collider, contactFilter, collider2Ds);
            foreach (Collider2D collider in collider2Ds)
            {
                if (collider != null)
                {
                    Enemy enemyHit = collider.GetComponent<Enemy>();
                    Boss bossHit = collider.GetComponent<Boss>();
                    ChestAndBox chestAndBoxHit = collider.GetComponent<ChestAndBox>();

                    // Enemy Hit
                    if (enemyHit != null)
                    {
                        Vector2 enemyPos = enemyHit.transform.position;
                        Vector2 vector = enemyPos - (Vector2)transform.position;
                        Vector2 dir = vector.normalized;
                        float dist = vector.magnitude;

                        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, dir, dist, LayerMask.NameToLayer("mapColliders"));

                        if (!hit)
                        {
                            int damages = Mathf.CeilToInt(baseDamages * player.stats.capacitiesDamagesM);

                            // FeedBacks
                            enemyHit.TakeDamage(damages);
                           
                            hasHit = true;
                            currentLifeTime = 0;
                        }
                    }
                    // BOSS HIT CHECK
                    if (bossHit != null)
                    {
                        Vector2 playerCenter = (Vector2)player.centerTransform.position;
                        Vector2 bossPos = bossHit.transform.position;
                        Vector2 vector = bossPos - playerCenter;
                        Vector2 dir = vector.normalized;
                        float dist = vector.magnitude;

                        RaycastHit2D hit = Physics2D.Raycast(player.centerTransform.position, dir, dist, LayerMask.NameToLayer("mapColliders"));

                        if (!hit)
                        {
                            int damages = Mathf.CeilToInt(baseDamages * player.stats.capacitiesDamagesM);

                            // FeedBacks
                            bossHit.TakeDamage(damages);
                           
                            hasHit = true;
                            currentLifeTime = 0;
                        }
                    }
                    // CHEST HIT CHECK
                    if (chestAndBoxHit != null)
                    {
                        chestAndBoxHit.Hit();

                        hasHit = true;
                        currentLifeTime = 0;
                    }
                }
            }
        }
    }

    public void Init(Player _player, float _baseDamages)
    {
        player = _player;
        directionX = Mathf.Sign(player.transform.localScale.x);
        Vector3 scale = transform.localScale;
        scale.x *= directionX;
        transform.localScale = scale;

        baseDamages = _baseDamages;
    }
}
