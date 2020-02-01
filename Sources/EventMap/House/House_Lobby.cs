using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House_Lobby : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] SpriteRenderer side;
    [SerializeField] SpriteRenderer anchorChain;
    [SerializeField] SpriteRenderer anchor;
    [Space]
    [SerializeField] HouseBound enterBound;
    [SerializeField] HouseBound exitBound;
    [Header("DATA")]
    [SerializeField] bool IsInHouse;
    [Range(0.5f, 4f)]
    [SerializeField] float sideUpdateSpeed, anchorUpdateSpeed;
    Color transparent;

    // Use this for initialization
    void Start()
    {
        transparent = new Color(1, 1, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRenderers();
    }

    void UpdateRenderers()
    {
        if (IsInHouse)
        {
            if (anchor.color.a > 0.001f)
            {
                anchor.color = Color.Lerp(anchor.color, transparent, anchorUpdateSpeed * Time.deltaTime);
                anchorChain.color = Color.Lerp(anchorChain.color, transparent, anchorUpdateSpeed * Time.deltaTime);
            }

            if (anchor.color.a < 0.3f)
            {
                if (side.color.a > 0.001f)
                {
                    side.color = Color.Lerp(side.color, transparent, sideUpdateSpeed * Time.deltaTime);
                }
            }
        }
        else
        {
            if (side.color.a < 1f)
            {
                side.color = Color.Lerp(side.color, Color.white, sideUpdateSpeed * Time.deltaTime);
            }

            if (side.color.a > 0.8f)
            {
                if (anchor.color.a < 1f)
                {
                    anchor.color = Color.Lerp(anchor.color, Color.white, anchorUpdateSpeed * Time.deltaTime);
                    anchorChain.color = Color.Lerp(anchorChain.color, Color.white, anchorUpdateSpeed * Time.deltaTime);
                }
            }
        }
    }

    public void InHouse(bool state)
    {
        if (IsInHouse != state)
        {
            IsInHouse = state;
        }
    }
}
