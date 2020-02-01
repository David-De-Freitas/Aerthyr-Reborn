using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditorInternal;

namespace Node.Dialogue
{
    public class DialogueNode : BaseNode
    {
        public List<LinkNode> enterList = new List<LinkNode>();
        public List<LinkNode> exitList = new List<LinkNode>();

        List<Answer> answers = new List<Answer>();
        DialogueText dialogueText;
        string text;

        public override void DrawWindow()
        {
            if (dialogueText == null)
            {
                dialogueText = CreateInstance<DialogueText>();
            }
            DrawDialogueText();
            DrawAnswers();


        }

        private void DrawDialogueText()
        {
            var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
            EditorGUILayout.LabelField("DialogueText", style, GUILayout.ExpandWidth(true));

            GUILayout.BeginHorizontal();
            dialogueText.text = EditorGUILayout.TextField(dialogueText.text, GUILayout.Height(20));
            if (GUILayout.Button("E", GUILayout.Width(20)))
            {
                DialogueEditor.AddDialogueTextEdit(dialogueText, new Vector2(windowRect.xMax + 40, windowRect.y));
                dialogueText.editing = true;
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            dialogueText.noAnswers = GUILayout.Toggle(dialogueText.noAnswers, "No Anwser");
            if (dialogueText.noAnswers)
            {
                if (GUILayout.Button("L", GUILayout.Width(20)))
                {
                    dialogueText.linkNode = CreateInstance<LinkNode>();
                    dialogueText.linkNode.StartLink(this, 70);
                }
                if (dialogueText.linkNode != null && dialogueText.linkNode.linkState == LinkNode.LinkState.disable)
                {
                    dialogueText.linkNode.linkState = LinkNode.LinkState.enable;
                }
            }
            else
            {
                if (dialogueText.linkNode != null)
                {
                    dialogueText.linkNode.linkState = LinkNode.LinkState.disable;
                }
            }
            GUILayout.EndHorizontal();
        }

        private void DrawAnswers()
        {
            EditorGUILayout.Separator();
            var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
            EditorGUILayout.LabelField("Answers", style, GUILayout.ExpandWidth(true));

            EditorGUI.BeginDisabledGroup(dialogueText.noAnswers);
            if (GUILayout.Button("Add answer"))
            {
                Answer answer = CreateInstance<Answer>();
                answers.Add(answer);
            }
            int indexAnwser = 0;

            foreach (Answer answer in answers)
            {
                GUILayout.BeginHorizontal();
                answer.text = EditorGUILayout.TextField(answer.text, GUILayout.Height(20));
                if (GUILayout.Button("E", GUILayout.Width(20)))
                {
                    DialogueEditor.AddAnwserEdit(answer, new Vector2(windowRect.xMax + 40, windowRect.y));
                    answer.editing = true;
                }
                if (GUILayout.Button("L", GUILayout.Width(20)))
                {
                    answer.linkNode = CreateInstance<LinkNode>();
                    answer.linkNode.StartLink(this, 135 + exitList.Count * 20);
                }
                GUILayout.EndHorizontal();
                indexAnwser++;
            }
            EditorGUI.EndDisabledGroup();
        }

        public override void DrawCurve()
        {

        }

    }

    public class Answer : BaseNode
    {
        public bool editing;
        public string text;
        public LinkNode linkNode;

        public override void DrawWindow()
        {
            if (GUILayout.Button("Close", GUILayout.Width(60)))
            {
                editing = false;
                DialogueEditor.DeletteNode(this);
            }
            text = EditorGUILayout.TextArea(text, GUILayout.Height(windowRect.height));

        }

    }

    public class DialogueText : BaseNode
    {
        public bool editing;
        public string text;
        public bool noAnswers;
        public LinkNode linkNode;

        public override void DrawWindow()
        {
            if (GUILayout.Button("Close", GUILayout.Width(60)))
            {
                editing = false;
                DialogueEditor.DeletteNode(this);
            }
            text = EditorGUILayout.TextArea(text, GUILayout.Height(windowRect.height));

        }


    }
}
