using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] Transform child;
    [SerializeField] Vector2 velocity;
    [SerializeField] Vector2 dir;
    [Space]
    [SerializeField] float speed;
    [SerializeField] float rotateSpeed;
    [Space]
    [SerializeField] int damage;
    [SerializeField] List<LayerMask> collidingWith = new List<LayerMask>();
    CircleCollider2D circleCollider;

    // Use this for initialization
    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

        UpdateMove();
        CheckCollide();

    }

    public void Init(Vector2 newDirection, int rawDamage)
    {
        dir = newDirection;
        damage = rawDamage;

        CalculateMovements();
    }

    void CalculateMovements()
    {
        velocity = dir * speed;
        rotateSpeed *= Mathf.Sign(dir.x);
    }

    void UpdateMove()
    {
        transform.Translate(velocity * Time.deltaTime);
        child.Rotate(Vector3.back, rotateSpeed * Time.deltaTime);
    }

    void CheckCollide()
    {
        foreach (LayerMask layer in collidingWith)
        {
            Collider2D[] colliders2D = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius, layer);
            foreach (Collider2D collid in colliders2D)
            {
                if (collid.CompareTag("Player"))
                {
                    GameManager.Singleton.Player.TakeDamage(damage);
                }

                DestroyProjectile();
                break;
            }
        }
    }


    void DestroyProjectile()
    {
        Destroy(gameObject);
        //emit particle;
    }
}
