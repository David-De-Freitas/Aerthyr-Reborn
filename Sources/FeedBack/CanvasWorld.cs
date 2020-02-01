using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasWorld : MonoBehaviour
{



    private static CanvasWorld singleton = null;
    public static CanvasWorld Singleton
    {
        get
        {
            return singleton;
        }
    }

    public DamageTextManager damageTextManager;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
