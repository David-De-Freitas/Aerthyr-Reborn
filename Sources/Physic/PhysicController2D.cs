using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PhysicController2D : MonoBehaviour
{
    [SerializeField] private CollisionInfo2D collisionInfo2D;

    // Use this for initialization
    void Start ()
    {
        collisionInfo2D = new CollisionInfo2D();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D[] contactPoint = new ContactPoint2D[1];
        collision.GetContacts(contactPoint);

        float contactAngle = Vector2.SignedAngle(Vector2.up, contactPoint[0].normal);

        if (Mathf.Abs(contactAngle) < 65)
        {
            collisionInfo2D.collidersBelow.Add(collision.collider);
        }
        else if (Mathf.Abs(contactAngle) <= 90 )
        {
            if (Mathf.Sign(contactAngle) == 1)
            {
                collisionInfo2D.collidersRight.Add(collision.collider);
            }
            else
            {
                collisionInfo2D.collidersLeft.Add(collision.collider);
            }
        }
        else
        {
            collisionInfo2D.collidersAbove.Add(collision.collider);
        }

        collisionInfo2D.Update();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collisionInfo2D.collidersAbove.Contains(collision.collider))
        {
            collisionInfo2D.collidersAbove.Remove(collision.collider);
        }
        else if (collisionInfo2D.collidersBelow.Contains(collision.collider))
        {
            collisionInfo2D.collidersBelow.Remove(collision.collider);
        }
        else if (collisionInfo2D.collidersRight.Contains(collision.collider))
        {
            collisionInfo2D.collidersRight.Remove(collision.collider);
        }
        else if (collisionInfo2D.collidersLeft.Contains(collision.collider))
        {
            collisionInfo2D.collidersLeft.Remove(collision.collider);
        }

        collisionInfo2D.Update();
    }

    [System.Serializable]
    public class CollisionInfo2D
    {
        [SerializeField] private bool above, below, right, left;

        public List<Collider2D> collidersAbove;
        public List<Collider2D> collidersBelow;
        public List<Collider2D> collidersRight;
        public List<Collider2D> collidersLeft;

        public bool Above { get { return above; } }
        public bool Below { get { return below; } }
        public bool Right { get { return right; } }
        public bool Left { get { return left; } }

        public CollisionInfo2D()
        {
            above = below = right = left = false;
            collidersAbove = new List<Collider2D>();
            collidersBelow = new List<Collider2D>();
            collidersRight = new List<Collider2D>();
            collidersLeft = new List<Collider2D>();
        }

        public void Update()
        {
            above = collidersAbove.Count != 0;
            below = collidersBelow.Count != 0;
            right = collidersRight.Count != 0;
            left = collidersLeft.Count != 0;
        }
    }
}
