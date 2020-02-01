using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Node
{
    public abstract class BaseNode : ScriptableObject
    {
        public Vector2 windowLocalPos;
        public Rect windowRect;
        public string windowTitle = "";

        public virtual void DrawWindow()
        {
            windowTitle = EditorGUILayout.DelayedTextField("Title", windowTitle);

        }

        public virtual void DrawCurve()
        {

        }
        

        public virtual void NodeDeleted(BaseNode node)
        {

        }
        

    }
}