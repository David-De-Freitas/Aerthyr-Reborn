using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemHandler : MonoBehaviour
{
    Image image;
	// Use this for initialization
	void Start ()
    {
        image = GetComponent<Image>();
        image.enabled = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (image.enabled)
        {
            transform.position = Input.mousePosition;
        }
	}

    public void SetIcon(Sprite icon)
    {
        image.sprite = icon;
        image.preserveAspect = true;

        transform.position = Input.mousePosition;

        image.enabled = true;
    }

    public void ResetIcon()
    {
        image.sprite = null;
        image.enabled = false;
    }
}
