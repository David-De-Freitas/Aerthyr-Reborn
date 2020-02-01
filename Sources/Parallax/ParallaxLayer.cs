using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Header("LAYER INFOS")]
    [SerializeField] int layerID;
    [SerializeField] int objectsInLayer;
    [SerializeField] Rect layerRect;
    [Header("ISLES")]
    [SerializeField] Vector2 spaceBetweenIsles = new Vector3(50, 20);
    [SerializeField] Vector2 positionsPossibilities;
    [Header("CLOUDS")]
    [SerializeField] bool hasClouds;
    [SerializeField] int cloudsNumber;
    [SerializeField] float cloudsOffsetY;
    [SerializeField] Vector2 minSpaceBetweenClouds;
    [Header("WIND")]
    [SerializeField] Vector2 windDirectionalForce;
    [Space]
    [Header("ELEMENTS IN LAYER")]
    [SerializeField] List<ParallaxElement> parallaxElements = new List<ParallaxElement>();

#if UNITY_EDITOR
    [Space]
    [Header("Debug")]
    public bool drawDebug = true;
    [ContextMenuItem("Randomize Color", "RandomizeColor")]
    public Color rectColor = Color.black;
#endif

    public ParallaxControler Controler { get; private set; }

    #region ACCESSORS
    public Rect LayerRect { get { return layerRect; } }
    public Vector2 SpaceBetweenElements { get { return spaceBetweenIsles; } }
    public Vector2 PositionsPossibilities { get { return positionsPossibilities; } }
    public List<ParallaxElement> ParallaxElements { get { return parallaxElements; } set { parallaxElements = value; } }
    public Vector2 WindDirectionalForce { get { return windDirectionalForce; } }
    #endregion

    private void Start()
    {
        Controler = GetComponentInParent<ParallaxControler>();
        SpawnParallax();
        windDirectionalForce += Controler.WindDirectionalForce;
    }


    private void SpawnParallax()
    {
        SpawnIsles();

        if (hasClouds)
        {
            SpawnClouds();
        }
    }

    private void SpawnIsles()
    {
        // OBJECT LAYER SPAWN
        for (int i = 0; i < objectsInLayer; i++)
        {
            int objectID = Random.Range(0, Controler.ParallaxIsles.Length);

            Object paralaxObject = Controler.ParallaxIsles[objectID];
            GameObject go = (GameObject)Instantiate(paralaxObject, transform) as GameObject;

            SpriteRenderer renderer = go.GetComponent<SpriteRenderer>();
            SpriteRenderer childrenRenderer = go.transform.GetChild(0).GetComponent<SpriteRenderer>();

            // Position
            go.transform.position = CalculatePosition(i);
            go.transform.localScale = CalculateScale(go.transform.localScale);
            // Layer
            SetSpriteRenderers(ref renderer, ref childrenRenderer, go.transform.position.z);
            // Add to the List
            ParallaxElement element = go.GetComponent<ParallaxElement>();
            element.SetLayer(this);

            parallaxElements.Add(go.GetComponent<ParallaxElement>());
        }
    }

    private void SpawnClouds()
    {
        for (int i = 0; i < cloudsNumber; i++)
        {
            int cloudID = Random.Range(0, Controler.ParallaxClouds.Length);
            Object cloudObj = Controler.ParallaxClouds[cloudID];
            GameObject cloudGo = (GameObject)Instantiate(cloudObj, transform) as GameObject;

            cloudGo.transform.localScale = CalculateCloudScale(cloudGo.transform.localScale);

            int posXnb;
            int posYnb;

            posXnb = Mathf.CeilToInt(layerRect.width / minSpaceBetweenClouds.x);
            posYnb = Mathf.CeilToInt(layerRect.height / minSpaceBetweenClouds.y);

            int posXid;
            int posYid;

            posXid = Random.Range(0, posXnb);
            posYid = Random.Range(0, posYnb);

            Vector3 pos = cloudGo.transform.position;
            pos.x = layerRect.xMin + posXid * minSpaceBetweenClouds.x;
            pos.y = layerRect.yMin + posYid * minSpaceBetweenClouds.y + cloudsOffsetY;
            pos.z = (parallaxElements.Count > 0) ? (parallaxElements[parallaxElements.Count -1].transform.position.z + 5) : (transform.position.z + 20);
            cloudGo.transform.position = pos;

            SpriteRenderer renderer = cloudGo.GetComponent<SpriteRenderer>();
            renderer.sortingOrder = (int)(-pos.z) + Controler.MinSortingOrder + i;

            ParallaxElement element = cloudGo.GetComponent<ParallaxElement>();
            element.SetLayer(this);
            parallaxElements.Add(cloudGo.GetComponent<ParallaxElement>());
        }
    }

    Vector3 CalculatePosition(int objectID)
    {
        Vector3 position;

        float xOffset = Random.Range(-10, 10) * 2;
        float yOffset = Random.Range(-5, 5) * 2;
        float zOffset = (objectID * 2) + (objectID + 1);

        position.x = Random.Range(0, positionsPossibilities.x) * SpaceBetweenElements.x + layerRect.xMin + xOffset;
        position.y = Random.Range(0, positionsPossibilities.y) * SpaceBetweenElements.y + layerRect.yMin + yOffset;
        position.z = transform.position.z + zOffset;

        return position;
    }

    Vector3 CalculateScale(Vector3 baseScale)
    {
        Vector3 scale = Vector3.one;

        float scaleValue;
        int minScale;
        int maxScale;

        minScale = (int)baseScale.x;
        maxScale = (int)baseScale.x + layerID + 1;
        scaleValue = Random.Range(minScale, maxScale);

        scale.x = scaleValue;
        scale.y = scaleValue;

        if (Random.Range(0, 11) % 2 == 1)
        {
            scale.x *= -1;
        }

        return scale;
    }

    Vector3 CalculateCloudScale(Vector3 baseScale)
    {
        Vector3 scale = Vector3.one;

        float scaleValue;
        int minScale;
        int maxScale;

        minScale = (int)baseScale.x;
        maxScale = (int)baseScale.x * (1 + Mathf.CeilToInt(layerID * 0.8f));
        scaleValue = Random.Range(minScale, maxScale);

        scale.x = scaleValue;
        scale.y = scaleValue;

        if (Random.Range(0, 11) % 2 == 1)
        {
            scale.x *= -1;
        }

        return scale;
    }

    void SetSpriteRenderers(ref SpriteRenderer parentRenderer, ref SpriteRenderer childRenderer, float positionZ)
    {
        parentRenderer.sortingOrder = (int)(-positionZ) + Controler.MinSortingOrder;
        childRenderer.sortingOrder = parentRenderer.sortingOrder + 1;
        //Color
        Color newColor = childRenderer.color;
        newColor.a = (positionZ * 1f) / 1500f;

        childRenderer.color = newColor;
    }

    public void SetLayerRect(Rect refRect, float layerOffsetY)
    {
        float widthAddingPerLayer = (layerID + 1) * 300f + layerID * 200f;
        float heightAddingPerLayer = layerID * -2.5f;

        float yOffsetPerLayer = -10 * layerID;

        float newWidth;
        float newHeight;

        newWidth = refRect.width + widthAddingPerLayer;
        newHeight = refRect.height + heightAddingPerLayer;

        if (newHeight < 15f)
        {
            newHeight = 15f;
        }

        layerRect.xMin = refRect.xMin - widthAddingPerLayer / 2;
        layerRect.yMin = refRect.yMin + heightAddingPerLayer / 2 + layerOffsetY + yOffsetPerLayer;

        layerRect.xMax = layerRect.xMin + newWidth;
        layerRect.yMax = layerRect.yMin + newHeight;

        CalculatePositionsPossibilities();

#if UNITY_EDITOR
        RandomizeColor();
#endif
    }

    public void SetPositionZ(float posZ)
    {
        transform.position += Vector3.forward * posZ;
    }

    public void SetObjectsInLayer(int value)
    {
        objectsInLayer = value;
    }

    public void SetID(int value)
    {
        layerID = value;
    }

    public void DeleteAllElements()
    {
        for (int i = 0; i < parallaxElements.Count; i++)
        {
            Destroy(parallaxElements[i].gameObject);
        }

        parallaxElements.Clear();
    }

    private void CalculatePositionsPossibilities()
    {
        positionsPossibilities.x = (int)((layerRect.width) / spaceBetweenIsles.x);
        positionsPossibilities.y = (int)((layerRect.height) / spaceBetweenIsles.y);
    }

    public void RemoveAndDestroy()
    {
        ParallaxControler controler = GetComponentInParent<ParallaxControler>();
        controler.RemoveLayer(this);
        controler.ResetLayers();

        DestroyImmediate(gameObject, true);
    }

    private void OnValidate()
    {
        CalculatePositionsPossibilities();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (drawDebug)
        {
            DrawGizmosRect(layerRect, rectColor);
        }
    }

    private void DrawGizmosRect(Rect rect, Color color)
    {
        //Get the rect's corner positions
        Vector3 topLeft = new Vector3(rect.xMin, rect.yMax, transform.position.z);
        Vector3 botLeft = new Vector3(rect.xMin, rect.yMin, transform.position.z);
        Vector3 topRight = new Vector3(rect.xMax, rect.yMax, transform.position.z);
        Vector3 botRight = new Vector3(rect.xMax, rect.yMin, transform.position.z);

        // Set the color
        Gizmos.color = color;

        //Draw the rect
        Gizmos.DrawLine(topLeft, botLeft);
        Gizmos.DrawLine(topRight, botRight);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(botLeft, botRight);
    }

    private void RandomizeColor()
    {
        rectColor.r = Random.value;
        rectColor.g = Random.Range(0f, 1f);
        rectColor.b = Random.Range(0f, 1f);
        rectColor.a = 1f;
    }
#endif
}
