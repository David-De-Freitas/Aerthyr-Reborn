using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Node.Dialogue
{
    public class DialogueEditor : EditorWindow
    {
        #region Variables
        static List<BaseNode> windows = new List<BaseNode>();

        public static Vector3 mousePosition;

        bool makeTransition;
        bool clickedOnWindow;

        int selectedIndex;
        static BaseNode selectedNode;

        static int TextNodeCount;
        public static bool makeLink;

        public static Vector2 panOffset;

        public static float zoomDelta = 0.1f;
        public static float minZoom = 1f;
        public static float maxZoom = 4f;
        public static float panSpeed = 1.2f;

        private Texture2D gridTex;

        public Color backColor;


        public enum UserActions
        {
            addTextNode,
            deleteNode,
            commentNode
        }
        #endregion


        #region init
        [MenuItem("Node Editor/Dialogue")]
        static void ShowEditor()
        {
            DialogueEditor editor = EditorWindow.GetWindow<DialogueEditor>();
            editor.minSize = new Vector2(300, 300);
        }

        public DialogueEditor()
        {
            backColor = new Color(59 / 255, 62 / 255, 74 / 255);



        }

        private void OnEnable()
        {

            windows.Clear();
            panOffset = Vector2.zero;
            gridTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Scripts/Editor/NodeDialogues/Texture/Grid.png");

            if (windows.Count == 0)
            {
                AddDialogueNode(new Vector2(30, 30));
            }
        }

        #endregion


        #region GUI Methods

        private void OnGUI()
        {

            Event e = Event.current;
            mousePosition = e.mousePosition;

            if (position.Contains(mousePosition))
            {
                DialogueEditor editor = EditorWindow.GetWindow<DialogueEditor>();
                editor.wantsMouseMove = true;
            }
            UserInput(e);
            DrawWindows();
        }

        void DrawWindows()
        {
            BeginWindows();
            DrawGrid();
            foreach (BaseNode node in windows)
            {
                node.DrawCurve();
            }

            for (int i = 0; i < windows.Count; i++)
            {
                //Rect winRect = windows[i].windowRect;
                //winRect.position = windows[i].windowLocalPos + panOffset;
                windows[i].windowRect = GUI.Window(i, windows[i].windowRect, DrawNodeWindow, windows[i].windowTitle);

            }

            EndWindows();
        }

        void DrawNodeWindow(int id)
        {
            windows[id].DrawWindow();
            GUI.DragWindow();

        }

        private void DrawGrid()
        {
            //var size = window.Size.size;
            //var center = size / 2f;

            //float zoom = ZoomScale;

            //// Offset from origin in tile units
            //float xOffset = -(center.x * zoom + panOffset.x) / _gridTex.width;
            //float yOffset = ((center.y - size.y) * zoom + panOffset.y) / _gridTex.height;

            //Vector2 tileOffset = new Vector2(xOffset, yOffset);

            //// Amount of tiles
            //float tileAmountX = Mathf.Round(size.x * zoom) / _gridTex.width;
            //float tileAmountY = Mathf.Round(size.y * zoom) / _gridTex.height;

            //Vector2 tileAmount = new Vector2(tileAmountX, tileAmountY);

            // Draw tiled background
            GUI.DrawTextureWithTexCoords(new Rect(0, 0, 1920, 1080), gridTex, new Rect(0, 0, 64, 64));
        }

        void UserInput(Event e)
        {


            if (e.button == 1)
            {
                if (!makeLink)
                {
                    if (e.type == EventType.MouseDown)
                    {
                        RightClick(e);
                    }
                }
                else
                {
                    if (e.type == EventType.MouseDown)
                    {
                        makeLink = false;
                        DeletteNode(selectedNode);
                    }
                }

            }
            if (e.button == 0)
            {
                if (e.type == EventType.MouseDown)
                {
                    if (makeLink)
                    {
                        makeLink = false;
                        LinkNode link = selectedNode as LinkNode;
                        link.SetTarget(LinkCollide(e.mousePosition));
                    }

                }
                if (e.type == EventType.MouseDrag)
                {
                    bool hitWindow = false;
                    for (int i = 0; i < windows.Count; i++)
                    {
                        if (windows[i].windowRect.Contains(e.mousePosition))
                        {
                            hitWindow = true;
                            break;
                        }
                    }
                    if (!hitWindow)
                    {
                        panOffset += Event.current.delta;
                    }
                }
            }
        }

        void RightClick(Event e)
        {
            clickedOnWindow = false;
            selectedIndex = -1;
            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].windowRect.Contains(e.mousePosition))
                {
                    clickedOnWindow = true;
                    selectedNode = windows[i];
                    selectedIndex = i;
                    break;
                }
            }
            if (!clickedOnWindow)
            {
                AddNewNode(e);
            }
            else
            {
                ModifyNode(e);
            }
        }

        void AddNewNode(Event e)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Add TextNode"), false, ContextCallBack, UserActions.addTextNode);
            menu.AddItem(new GUIContent("Add Comment"), false, ContextCallBack, UserActions.commentNode);

            menu.ShowAsContext();
            e.Use();
        }

        void ModifyNode(Event e)
        {
            string content = "";
            if (selectedNode is DialogueNode)
            {
                content = "Delete";
            }
            else if (selectedNode is DialogueText)
            {
                content = "Close";
            }
            else if (selectedNode is Answer)
            {
                content = "Close";
            }
            else
            {
                return;
            }

            GenericMenu menu = new GenericMenu();
            menu.AddSeparator("");
            menu.AddItem(new GUIContent(content), false, ContextCallBack, UserActions.deleteNode);
            menu.ShowAsContext();
            e.Use();
        }

        void ContextCallBack(object o)
        {
            UserActions a = (UserActions)o;

            switch (a)
            {
                case UserActions.addTextNode:
                    AddDialogueNode(mousePosition);
                    break;
                case UserActions.deleteNode:
                    DeletteNode(selectedNode);
                    break;
                case UserActions.commentNode:
                    break;
                default:
                    break;
            }

        }


        #endregion

        #region Helper Methods

        public static DialogueNode AddDialogueNode(Vector2 pos)
        {
            DialogueNode dialogueNode = CreateInstance<DialogueNode>();
            dialogueNode.windowRect = new Rect(pos.x, pos.y, 200, 300);
            dialogueNode.windowTitle = "TextNode n:" + TextNodeCount;
            TextNodeCount++;
            windows.Add(dialogueNode);
            return dialogueNode;
        }

        public static Answer AddAnwserEdit(Answer answer, Vector2 pos)
        {
            answer.windowRect = new Rect(pos.x, pos.y, 500, 300);
            answer.windowTitle = "Answer Editing";
            windows.Add(answer);
            return answer;
        }

        public static DialogueText AddDialogueTextEdit(DialogueText text, Vector2 pos)
        {
            text.windowRect = new Rect(pos.x, pos.y, 500, 300);
            text.windowTitle = "DialogueText Editing";
            windows.Add(text);
            return text;
        }

        public static void DrawNodeCurve(Rect start, Vector3 endPos, bool left, Color curveColor)
        {
            Vector3 startPos = new Vector3(
                (left) ? start.x + start.width : start.x,
                start.y,
                0
                );

            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;

            Color shadow = new Color(0, 0, 0, 0.06f);

            for (int i = 0; i < 3; i++)
            {
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadow, null, (i + 1) * 0.5f);
            }
            Handles.DrawBezier(startPos, endPos, startTan, endTan, curveColor, null, 3);
        }

        public static void DrawNodeCurve(Rect start, Rect end, bool left, Color curveColor)
        {
            Vector3 startPos = new Vector3(
                (left) ? start.x + start.width : start.x,
                start.y,
                0
                );

            Vector3 endPos = new Vector3(end.x + (end.width * 0.5f), end.y + (end.height * 0.5f), 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;

            Color shadow = new Color(0, 0, 0, 0.06f);

            for (int i = 0; i < 3; i++)
            {
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadow, null, (i + 1) * 0.5f);
            }
            Handles.DrawBezier(startPos, endPos, startTan, endTan, curveColor, null, 3);
        }

        public static void DeletteNode(BaseNode node)
        {
            if (windows.Contains(node))
            {
                if (node is DialogueNode)
                {
                    TextNodeCount--;
                    foreach (LinkNode link in ((DialogueNode)node).enterList)
                    {
                        ((DialogueNode)link.previousNode).exitList.Remove(link);
                        windows.Remove(link);
                    }
                    foreach (LinkNode link in ((DialogueNode)node).exitList)
                    {
                        ((DialogueNode)link.previousNode).enterList.Remove(link);
                        windows.Remove(link);
                    }
                }
                else if (node is LinkNode)
                {
                    if (((DialogueNode)((LinkNode)node).previousNode).exitList.Contains(node as LinkNode))
                    {
                        ((DialogueNode)((LinkNode)node).previousNode).exitList.Remove(node as LinkNode);
                    }
                    if ((DialogueNode)((LinkNode)node).targetNode != null)
                    {
                        if (((DialogueNode)((LinkNode)node).targetNode).enterList.Contains(node as LinkNode))
                        {
                            ((DialogueNode)((LinkNode)node).targetNode).enterList.Remove(node as LinkNode);
                        }
                    }
                }
                windows.Remove(node);
            }
        }

        public static void AddLinkNode(LinkNode link)
        {
            windows.Add(link);
            makeLink = true;
            selectedNode = link as BaseNode;
        }

        public static BaseNode LinkCollide(Vector2 mouse)
        {
            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].windowRect.Contains(mouse))
                {
                    return windows[i];
                }
            }
            return null;
        }

        public Vector2 MouseToPan(Vector2 mouse)
        {

            return mouse + panOffset;
        }
        #endregion
    }
}
