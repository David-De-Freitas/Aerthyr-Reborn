using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Node.Dialogue
{
    public class LinkNode : BaseNode
    {
        public DialogueNode previousNode;
        public DialogueNode targetNode;
        public LinkState linkState;
        float previousNodeOffSetY;

        public override void DrawWindow()
        {

        }

        public override void DrawCurve()
        {
            Rect targetRect = new Rect();
            Rect previousRect = previousNode.windowRect;
            previousRect.y += previousNodeOffSetY;

            if (previousNode == null)
            {
                DialogueEditor.DeletteNode(this);
            }

            if (linkState == LinkState.creating)
            {
                DialogueEditor.DrawNodeCurve(previousRect, DialogueEditor.mousePosition, true, Color.cyan);
            }
            else if (linkState == LinkState.enable)
            {
                if (targetNode == null)
                {
                    DialogueEditor.DeletteNode(this);
                }
                targetRect = targetNode.windowRect;


                int indexOnEnterList = 0;
                for (int i = 0; i < targetNode.enterList.Count; i++)
                {
                    if (targetNode.enterList[i] == this)
                    {
                        indexOnEnterList = i;
                        break;
                    }
                }
                
                targetRect.y += targetNode.windowRect.height/(targetNode.enterList.Count+1) * (indexOnEnterList+1);


                targetRect.width = 1;
                targetRect.height = 1;
                DialogueEditor.DrawNodeCurve(previousRect, targetRect, true, Color.cyan);
            }
        }

        public void SetTarget(BaseNode node)
        {
            if (node == null || previousNode == node)
            {
                DialogueEditor.DeletteNode(this);
                return;
            }
            targetNode = node as DialogueNode;
            ((DialogueNode)node).enterList.Add(this);
            linkState = LinkState.enable;
        }

        public void StartLink(DialogueNode dialogueNode, float offSetY)
        {
            dialogueNode.exitList.Add(this);
            DialogueEditor.AddLinkNode(this);
            previousNode = dialogueNode;
            linkState = LinkState.creating;
            previousNodeOffSetY = offSetY;
        }

        public enum LinkState
        {
            nothing,
            creating,
            enable,
            disable
        }
    }
}
