using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(Rigidbody2D))]
public class WorldMoney : MonoBehaviour
{
    new SpriteRenderer renderer;
    new CircleCollider2D collider2D;
    new Rigidbody2D rigidbody;

    Player player;
    Inventory inventory;

    [SerializeField] State state;

    float activationTime;

    bool onGround;

    enum State
    {
        Spawn,
        WaitDetection,
        Aspirated
    }

    // Use this for initialization
    void Start()
    {
        activationTime = 1.2f;
        state = State.Spawn;
        onGround = false;

        player = GameManager.Singleton.Player;
        inventory = player.GetComponentInChildren<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Spawn)
        {
            if (activationTime > 0f)
            {
                activationTime -= Time.deltaTime;
            }
            else
            {
                state = State.WaitDetection;
            }
        }
        else if (state == State.WaitDetection)
        {
            if (Vector2.Distance(transform.position, player.centerTransform.position) <= 3.5f)
            {
                state = State.Aspirated;
            }
        }
        else if (state == State.Aspirated)
        {
            Vector3 dir = player.centerTransform.position - transform.position;
            rigidbody.velocity = dir.normalized * 2000f * Time.deltaTime;

            if (Vector2.Distance(player.centerTransform.position, transform.position) <= 0.05f)
            {
                inventory.AddMoney(1);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!onGround && collision.gameObject.layer == LayerMask.NameToLayer("mapColliders"))
        {
            if (rigidbody.velocity.y < 0)
            {
                rigidbody.bodyType = RigidbodyType2D.Kinematic;
                rigidbody.velocity = Vector2.zero;
                rigidbody.gravityScale = 0;
                collider2D.isTrigger = true;
                onGround = true;
            }
        }
    }

    public void Init(Vector2 spawnPos, Sprite icon)
    {
        renderer = GetComponent<SpriteRenderer>();
        collider2D = GetComponent<CircleCollider2D>();
        rigidbody = GetComponent<Rigidbody2D>();

        renderer.sprite = icon;
        collider2D.radius = 0.2f;
        collider2D.isTrigger = true;

        Vector2 velocity;

        velocity.x = Random.Range(-3f, 3f);
        velocity.y = Random.Range(4f, 5f);

        rigidbody.velocity = velocity;

        transform.position = spawnPos;
        transform.localScale = Vector2.one * 0.5f;
        transform.Rotate(Vector3.forward * Random.Range(0, 360));
        gameObject.tag = "Money";
        gameObject.layer = LayerMask.NameToLayer("InteractableObject");
        gameObject.name = "WorldMoney";

        state = State.Spawn;
    }
}
