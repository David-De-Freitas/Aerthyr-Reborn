using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public Color colorDefault = new Color(255,0,0);
    public Color colorHover = new Color(255, 0, 0);
    private Text theText;

    private void Start()
    {
        theText = GetComponentInChildren<Text>();
        theText.color = colorDefault; //Or however you do your color
        theText.fontSize = 30;
        theText.fontStyle = FontStyle.Bold;
    }

    // DEFAULT
    public void OnPointerExit(PointerEventData eventData)
    {
        theText.color = colorDefault; //Or however you do your color
        theText.fontSize = 30;
        theText.fontStyle = FontStyle.Bold;
    }

    //HOVER
    public void OnPointerEnter(PointerEventData eventData)
    {
        theText.color = colorHover; //Or however you do your color
        theText.fontSize = 40;
        theText.fontStyle = FontStyle.BoldAndItalic;
    }


}
