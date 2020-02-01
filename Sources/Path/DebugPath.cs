using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugPath : MonoBehaviour {

    public Object MapNbPref;


	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void AddNbMap(string nb,Vector3 pos)
    {
        Instantiate(MapNbPref,transform);
        
        transform.GetChild(transform.childCount-1).GetComponent<RectTransform>().position = pos;
        transform.GetChild(transform.childCount-1).GetComponent<Text>().text = nb;
        //Text newText = newGo.GetComponent<Text>();
        //newText.text = nb.ToString();

        //mapNb.Add(newGo);
    }
}
