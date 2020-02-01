using UnityEngine;
using System.Collections;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(ParallaxControler))]
[CanEditMultipleObjects]
public class ParallaxControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ParallaxControler controler = (ParallaxControler)target;
        if (GUILayout.Button("ADD LAYER"))
        {
            controler.AddParallaxLayer();
        }

        if (GUILayout.Button("REMOVE ALL LAYERS"))
        {
            controler.RemoveAllLayers();
        }
    }
}

[CustomEditor(typeof(ParallaxLayer))]
[CanEditMultipleObjects]
public class ParallaxLayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ParallaxLayer controler = (ParallaxLayer)target;
        if (GUILayout.Button("REMOVE & DESTROY"))
        {
            controler.RemoveAndDestroy();
        }
    }
}
#endif