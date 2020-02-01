using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D), typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class WorldItem : MonoBehaviour
{
    public Item item;

    new CircleCollider2D collider;
    new Rigidbody2D rigidbody;
    new SpriteRenderer renderer;

    [SerializeField] WorldItemState state;
    float currentBounceTime;
    float bounceSpeed;
    float bounceValue;
    float startPosY;

    private void Start()
    {
        collider = GetComponent<CircleCollider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();

        // Set GameObject
        gameObject.name = "World item : " + item.name;
        gameObject.tag = "Item";
        gameObject.layer = LayerMask.NameToLayer("InteractableObject");
        transform.localScale = Vector2.one * 0.5f;

        GameObject child = new GameObject("collider");
        child.transform.SetParent(transform);
        child.transform.localPosition = Vector3.zero;
        child.layer = gameObject.layer;

        // Set Collider
        collider.radius = 1.3f;
        collider.isTrigger = true;

        CircleCollider2D childCollider = child.AddComponent<CircleCollider2D>();
        childCollider.radius = 0.3f;

        // Set Rigidbody
        Vector2 forceToAdd;

        forceToAdd.x = Random.Range(-2f, 2f);
        forceToAdd.y = Random.Range(4f, 8f);

        rigidbody.freezeRotation = true;
        rigidbody.velocity = forceToAdd;

        // Set Renderer
        if (item.isSaved)
        {
            renderer.sprite = item.savedIcon;
        }
        else
        {
            renderer.sprite = item.icon;
        }
      
        renderer.sortingOrder = 5;

        // Set WorldItem

        state = WorldItemState.Falling;
        bounceSpeed = Random.Range(2f, 2.4f);
        bounceValue = Random.Range(0.05f, 0.1f);
    }

    private void Update()
    {
        if (state == WorldItemState.Falling)
        {
            FallingUpdate();
        }
        else if (state == WorldItemState.Floating)
        {
            FloatingUpdate();
        }
    }

    private void FallingUpdate()
    {
        if (rigidbody.velocity.y < 0)
        {
            Vector2 rayOrigin = transform.position;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.5f, LayerMask.GetMask("mapColliders"));

            if (hit)
            {
                if (hit.collider.gameObject.layer == 10)
                {
                    state = WorldItemState.Floating;
                    rigidbody.bodyType = RigidbodyType2D.Kinematic;
                    rigidbody.velocity = Vector2.zero;

                    startPosY = transform.position.y;
                }
            }
        }
    }

    private void FloatingUpdate()
    {
        currentBounceTime += Time.deltaTime;

        Vector3 newPos = transform.position;
        newPos.y = startPosY + Mathf.Sin(currentBounceTime * bounceSpeed) * bounceValue;
        transform.position = newPos;

    }

    public void PickUp(ref Inventory inventory)
    {
        if (inventory.CanAddItem())
        {
            inventory.AddItem(item);

            Destroy(gameObject);
        }
    }

    public enum WorldItemState
    {
        Falling,
        Floating
    }
}
