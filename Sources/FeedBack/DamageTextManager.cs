using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageTextManager : MonoBehaviour
{
    public Object textPref;
    public Object critPref;

    [Header("Damage Info")]
    [Space]
    [Header("Low")]

    public Color lowColor;

    [Header("Medium")]

    public int medMin;
    public Color MedColor;

    [Header("Hight")]

    public int hightMin;
    public Color hightColor;

    private void Start()
    {
        CanvasWorld.Singleton.damageTextManager = this;
    }

    /// <summary>
    /// Generate a damage particule.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="textTransform"></param>
    public void AddDamageText(int damage, Transform textTransform)
    {
        GameObject textOb = (GameObject)Instantiate(textPref, transform) as GameObject;
        Text text = textOb.GetComponentInChildren<Text>();

        text.color = GetActualColor(damage);
        text.fontSize = GetActualFontSize(damage);
        text.text = damage.ToString();

        textOb.transform.position = textTransform.position;
    }

    /// <summary>
    /// Instantiate the critical damage feedback at the position of the transform.
    /// </summary>
    /// <param name="refTransform"></param>
    public void GenerateCritFeedback(Transform refTransform)
    {
        GameObject critOb = (GameObject)Instantiate(critPref) as GameObject;
        critOb.transform.position = refTransform.position;

        Vector3 scale = critOb.transform.localScale;
        scale.x = Random.Range(0, 2) == 0 ? -1 : 1;
        critOb.transform.localScale = scale;

        critOb.transform.localScale *= Random.Range(1f, 1.4f);
    }

    private Color GetActualColor(int damage)
    {
        Color color = Color.white;

        if (damage < medMin)
        {
            color = Color.Lerp(lowColor, MedColor, damage / medMin);
        }
        else
        {
            color = Color.Lerp(MedColor, hightColor, damage / hightMin);
        }

        return color;


    }

    private int GetActualFontSize(int damage)
    {
        int fontSize = 30;


        return fontSize;
    }
}
