using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxControler : MonoBehaviour
{
    [SerializeField] Transform background;
    [Space]
    [Header("Infos")]
    [SerializeField] int baseZPosition = 100;
    [SerializeField] float layerOffsetY = -100;
    [Space]
    [SerializeField] int maxSortingOrder = -500;
    [SerializeField] int minSortingOrder = -200;
    [Space]
    [SerializeField] Rect rect;
    [SerializeField] Vector2 referenceSize = new Vector2(270, 150);
    [Space]
    int maxObjectPerLayer = 10;
    int minObjectPerLayer = 5;
    [Space]
    [SerializeField] bool globalWind;
    [SerializeField] Vector2 windDirectionnalForce;
    [Space]
    //[Header("Parallax Layers")]
    [SerializeField] List<ParallaxLayer> layers = new List<ParallaxLayer>();

    Camera2D camera2D;
    Object[] parallaxIsles;
    Object[] parallaxClouds;
    int maxLayerNB = 6;

    int offsetBetweenLayers;
    bool objectPerLayerSet = false;

    public List<ParallaxLayer> Layers { get { return layers; } }
    public Object[] ParallaxIsles { get { return parallaxIsles; } }
    public Object[] ParallaxClouds { get { return parallaxClouds; } }
    public int MinSortingOrder { get { return minSortingOrder; } }
    public bool GlobalWind { get { return globalWind; } }
    public Vector2 WindDirectionalForce { get { return windDirectionnalForce; } }

    #region Inspector
#if UNITY_EDITOR

    private void OnValidate()
    {
        int layerNB = 0;
        // Get the nb of ParallaxLayer in Childs
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).CompareTag("ParallaxLayer"))
            {
                layerNB++;
            }
        }
    }

    public void AddParallaxLayer()
    {
        if (layers.Count < maxLayerNB)
        {
            // SET the camera2D if it's not already set and get the Limits of the camera
            if (camera2D == null)
            {
                camera2D = Camera.main.GetComponent<Camera2D>();
                rect = camera2D.cameraLimits;
            }

            SetObjectsInLayers();

            string name = "ParallaxLayer_" + (layers.Count + 1);
            GameObject newLayer = new GameObject(name, typeof(ParallaxLayer));
            ParallaxLayer layer;

            newLayer.tag = "ParallaxLayer";
            newLayer.transform.SetParent(transform, false);

            layer = newLayer.GetComponent<ParallaxLayer>();

            layer.SetID(layers.Count);
            layer.SetLayerRect(rect, layerOffsetY);
            layer.SetObjectsInLayer(Random.Range(minObjectPerLayer, maxObjectPerLayer + 1) * (1 + layers.Count * 2));

            layers.Add(layer);
        }
    }

    /// <summary>
    /// Remove and Destroy all the parallax's layers.
    /// </summary>
    public void RemoveAllLayers()
    {
        for (int i = (layers.Count - 1); i >= 0; i--)
        {
            ParallaxLayer tmp;
            tmp = layers[0];
            layers.Remove(tmp);
            tmp.ParallaxElements.Clear();
            DestroyImmediate(tmp.gameObject);

            ResetLayers();
        }
    }

#endif
    #endregion

    // Use this for initialization

    private void Awake()
    {
        LoadParallaxResources();

        maxObjectPerLayer = 10;
        minObjectPerLayer = 5;

        if (layers.Count > 0)
        {
            InitParallax();
        }
    }

    void Start()
    {
        SetBackground();
    }

    void LoadParallaxResources()
    {
        if (parallaxIsles == null)
        {
            parallaxIsles = Resources.LoadAll("Parallax/Isles");
        }
        if (parallaxClouds == null)
        {
            parallaxClouds = Resources.LoadAll("Parallax/Clouds");
        }
    }

    void InitParallax()
    {
        offsetBetweenLayers = (maxSortingOrder - minSortingOrder) / layers.Count;

        for (int i = 0; i < Layers.Count; i++)
        {
            layers[i].SetPositionZ(baseZPosition + i * Mathf.Abs(offsetBetweenLayers));
        }
    }

    void SetObjectsInLayers()
    {
        if (!objectPerLayerSet)
        {
            float refSizeRatio;
            float currentSizeRatio;

            refSizeRatio = referenceSize.x + referenceSize.y;
            currentSizeRatio = rect.width + rect.height;

            maxObjectPerLayer = Mathf.CeilToInt((maxObjectPerLayer * currentSizeRatio) / refSizeRatio);
            minObjectPerLayer = Mathf.CeilToInt((minObjectPerLayer * currentSizeRatio) / refSizeRatio);

            objectPerLayerSet = true;
        }
    }

    void SetBackground()
    {
        Vector3 position;
        Vector3 scale;
        float scaleValue;

        position.x = rect.xMin + rect.width / 2;
        position.y = rect.yMin + rect.height / 2;
        position.z = 1800f;

        background.position = position;

        scaleValue = (position.z / 10) * 1.5f;
        scale = Vector3.one * scaleValue;

        background.localScale = scale;
    }

    /// <summary>
    /// ADD a parallaxLayer to the list.
    /// </summary>
    /// <param name="layer"></param>
    public void AddLayer(ParallaxLayer layer)
    {
        layers.Add(layer);
    }

    /// <summary>
    /// REMOVE the current layer to the list.
    /// </summary>
    /// <param name="layer"></param>
    public void RemoveLayer(ParallaxLayer layer)
    {
        layers.Remove(layer);
    }

    /// <summary>
    /// RESET all the layers.
    /// To call when a layer is DESTROY or REMOVE from the list.
    /// </summary>
    public void ResetLayers()
    {
        for (int i = 0; i < layers.Count; i++)
        {
            if (layers[i] != null)
            {
                layers[i].name = "ParallaxLayer_" + (i + 1);
                layers[i].SetID(i);
                layers[i].SetObjectsInLayer(Random.Range(minObjectPerLayer, maxObjectPerLayer + 1) * (1 + i * 2));
                layers[i].SetLayerRect(rect, layerOffsetY);
            }
            else
            {
                layers.RemoveAt(i);
            }
        }
    }
}
