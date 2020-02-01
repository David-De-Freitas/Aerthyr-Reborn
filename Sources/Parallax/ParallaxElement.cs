using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxElement : MonoBehaviour
{
    [SerializeField] ParallaxElementType type;
    [Header("Bouncing parameters")]
    [SerializeField] bool isBouncing;
    [Range(0.8f, 1.5f)]
    [SerializeField] float maxBounceValue = 1f;
    [Range(0.4f, 1.2f)]
    [SerializeField] float minBounceValue = 0.5f;
    [SerializeField] float bounceValue;
    [Range(0.5f, 2f)]
    [SerializeField] float bounceSpeed = 1f;
    [Space]
    [Header("Wind parameters")]
    public bool isAffectedByWind;
    [Range(0f, 1f)]
    [SerializeField] float windResistance;

    float currentBounceTime;
    float startPosY;
    Bounds bounds;

    SpriteRenderer spriteRenderer;
    ParallaxLayer layer;

    // Use this for initialization
    void Start()
    {
        bounceValue = Random.Range(minBounceValue, maxBounceValue + 1);
        currentBounceTime = Random.Range(0, bounceValue);
        startPosY = transform.position.y;

        spriteRenderer = GetComponent<SpriteRenderer>();
        bounds = spriteRenderer.sprite.bounds;

        SetBounds();
    }

    // Update is called once per frame
    void Update()
    {
        StatesUpdate();
    }

    void SetBounds()
    {
        Vector3 scale;
        scale = Vector3.one;
        scale.x = Mathf.Abs(transform.localScale.x);
        scale.y = Mathf.Abs(transform.localScale.y);

        Vector3 boundsSize;
        boundsSize = bounds.size;

        boundsSize.x *= scale.x;
        boundsSize.y *= scale.y;

        bounds.size = boundsSize;
    }

    void StatesUpdate()
    {
        if (type == ParallaxElementType.Isle)
        {
            BouncingUpdate();
        }

        if (isAffectedByWind || layer.Controler.GlobalWind)
        {
            WindAffectedMovements();
        }
    }

    void BouncingUpdate()
    {
        if (isBouncing)
        {
            currentBounceTime += Time.deltaTime;

            Vector3 newPos = transform.position;
            newPos.y = startPosY + Mathf.Sin(currentBounceTime * bounceSpeed) * bounceValue;
            transform.position = newPos;
        }
    }

    void WindAffectedMovements()
    {
        Vector3 movement;
        Vector3 position;

        if (isAffectedByWind)
        {
            movement.x = layer.WindDirectionalForce.x * (1f - windResistance);
            movement.y = layer.WindDirectionalForce.y * (1f - windResistance);
        }
        else
        {
            movement.x = layer.Controler.WindDirectionalForce.x * (1f - windResistance);
            movement.y = layer.Controler.WindDirectionalForce.y * (1f - windResistance);
        }

        movement.z = 0f;

        transform.Translate(movement * Time.deltaTime);
        bounds.center = transform.position;
        position = transform.position;

        if (Mathf.Sign(layer.WindDirectionalForce.x) > 0)
        {
            if (bounds.min.x > layer.LayerRect.xMax)
            {
                position.x = layer.LayerRect.xMin - (bounds.extents.x + 20);
            }
        }
        else
        {
            if (bounds.max.x < layer.LayerRect.xMin)
            {
                position.x = layer.LayerRect.xMax + (bounds.extents.x + 20);
            }
        }

        if (Mathf.Sign(layer.WindDirectionalForce.y) > 0)
        {
            if (bounds.min.y > layer.LayerRect.yMax)
            {
                position.y = layer.LayerRect.yMin - (bounds.extents.y + 20);
            }
        }
        else
        {
            if (bounds.max.y < layer.LayerRect.yMin)
            {
                position.y = layer.LayerRect.yMax + (bounds.extents.y + 20);
            }
        }

        transform.position = position;
    }

    public void SetLayer(ParallaxLayer _layer)
    {
        layer = _layer;
    }

    public enum ParallaxElementType
    {
        Isle,
        Cloud
    }
}
