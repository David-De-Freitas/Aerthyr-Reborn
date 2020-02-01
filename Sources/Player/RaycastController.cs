using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
    public LayerMask collisionMask;

    protected const float skinWidth = 0.015f;
    const float dstBetweenRays = .25f;

    protected int horizontalRayCount;
    protected int verticalRayCount;

    protected float horizontalRaySpacing;
    protected float verticalRaySpacing;

    protected BoxCollider2D boxCollider;
    public BoxCollider2D BoxCollider
    {
        get
        {
            return boxCollider;
        }
    }
    protected RaycastOrigins raycastOrigins;

    protected virtual void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }
    // Use this for initialization
    protected virtual void Start ()
    {
        CalculateRaySpacing();
    }

    protected void UpdateRaycastOrigins()
    {
        Bounds bounds = boxCollider.bounds;
        CalculateRaySpacing();
       // bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y + 0.05f);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y + 0.05f);

        raycastOrigins.bottomCenter = new Vector2(bounds.max.x / 2, bounds.min.y + 0.05f);

        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;

        horizontalRayCount = Mathf.RoundToInt(boundsHeight / dstBetweenRays);
        verticalRayCount = Mathf.RoundToInt(boundsWidth / dstBetweenRays);

        if (verticalRayCount % 2 == 0)
        {
            verticalRayCount++;
        }

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight, bottomCenter;
    }
}
