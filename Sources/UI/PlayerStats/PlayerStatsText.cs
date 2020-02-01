using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsText : MonoBehaviour
{
    float statValue;
    float baseStatValue;

    bool isHover;
    bool previousHoverState;

    string prefix;
    string suffix;

    Text textComponent;

    Color positiveColor;
    Color negativeColor;
    Color temporaryAffectedColor;

    public bool temporyAffected;
    public bool affectedByFury;

    private void Awake()
    {
        textComponent = GetComponent<Text>();
        previousHoverState = false;
        positiveColor = new Color((78f / 255f), (175f / 255f), (8f / 255f));
        negativeColor = new Color((175f / 255f), (7f / 255f), (24f / 255f));
        temporaryAffectedColor = new Color((214f / 255f), (192f / 255f), (27f / 255f));
    }

    // Use this for initialization
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        isHover = IsPointerHover();

        if (isHover != previousHoverState)
        {
            previousHoverState = isHover;

            UpdateText();
        }

    }

    bool IsPointerHover()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, Input.mousePosition);
    }

    void UpdateText()
    {
        if (isHover)
        {
            string text;

            if (statValue > baseStatValue)
            {
                float diffValue = (Mathf.CeilToInt(statValue * 10) - Mathf.CeilToInt(baseStatValue * 10)) / 10f;
                text = prefix + "(" + baseStatValue.ToString("0.#") + " + " + diffValue.ToString("0.#") + ")" + suffix;
            }
            else if (statValue < baseStatValue)
            {
                float diffValue = (Mathf.CeilToInt(baseStatValue * 10) - Mathf.CeilToInt(statValue * 10)) / 10f;
                text = prefix + "(" + baseStatValue.ToString("0.#") + " - " + diffValue.ToString("0.#") + ")" + suffix;
            }
            else
            {
                text = prefix + statValue.ToString("0.#") + suffix;
            }

            textComponent.text = text;
        }
        else
        {
            textComponent.text = prefix + statValue.ToString("0.#") + suffix;
        }
    }

    /// <summary>
    /// Set the current and the base value of the stat.
    /// </summary>
    /// <param name="_value"></param>
    /// <param name="_baseValue"></param>
    public void SetStats(float _value, float _baseValue, string _prefix, string _suffix)
    {
        Color newColor = Color.white;

        statValue = _value;
        baseStatValue = _baseValue;

        prefix = _prefix;
        suffix = _suffix;

        if (!temporyAffected)
        {
            if (statValue > baseStatValue)
            {
                newColor = positiveColor;
            }
            else if (statValue < baseStatValue)
            {
                newColor = negativeColor;
            }
            else
            {
                newColor = Color.white;
            }
        }
        else
        {
            newColor = temporaryAffectedColor;
        }

        textComponent.color = newColor;

        UpdateText();
    }
}
