using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using System.Text;

public class dialogue : MonoBehaviour
{
    public enum ActionType
    {
        dialog,
        leave,
        nextScene
    }

    public struct Action
    {
        public string reponseText;
        public ActionType type;
        public string nextDialog;
    }
    [SerializeField] Canvas canvas;
    [SerializeField] string fileName = "Dialogue_lobby";
    [Space]
    [SerializeField] Text questionIG;
    [SerializeField] Button[] reponsesIG;

    readonly string path = "C:/Users/Etudiant4/Desktop/Dialogues.txt";
    TextAsset textFile;

    string question;
    int nbRep;
    Action[] reponses;

    bool isReady = false;

    private void debugDialog()
    {
        Debug.Log(question);
        for (int i = 0; i < nbRep; i++)
        {
            Debug.Log(reponses[i].reponseText + reponses[i].nextDialog);
        }
    }

    void Start()
    {
        canvas.gameObject.SetActive(false);
    }

    private void Update()
    {

        if (isReady)
        {
            isReady = false;
            questionIG.text = question;

            for (int i = 0; i < reponsesIG.Length; i++)
            {
                if (i < nbRep)
                {
                    reponsesIG[i].gameObject.SetActive(true);
                    reponsesIG[i].GetComponentInChildren<Text>().text = reponses[i].reponseText;
                }
                else
                {
                    reponsesIG[i].gameObject.SetActive(false);
                }
            }
        }

    }

    public void OnAnswerClic(int index)
    {
        switch (reponses[index].type)
        {
            case ActionType.dialog:
                isReady = loadDialog(path, reponses[index].nextDialog);
                break;
            case ActionType.leave:
                isReady = false;
                CloseDialog();
                break;
            case ActionType.nextScene:
                isReady = false;
                StartExpedition();
                break;
            default:
                break;
        }
    }

    private bool loadDialog(string path, string dialogName)
    {
        string[] textLines;
        string line;

        textFile = Resources.Load("TextFiles/" + fileName) as TextAsset;
       
        textLines = textFile.text.Split('\n');

        for (int lineId = 0; lineId < textLines.Length; lineId++)
        {
            line = textLines[lineId];
           
            if (line.IndexOf("name:") != -1)
            {
               // print(line);
                //find dialogue name
                if (line.IndexOf(dialogName) != -1)
                {
                    //load the dialogue found
                    while (line.IndexOf("<eod>") == -1)
                    {
                        lineId++;
                        line = textLines[lineId];
                        //get question
                        if (line.IndexOf("<q>") != -1)
                        {
                            string result = StringWithinBounds("<d>", "<ed>", line);
                            question = result;
                        }
                        //get answer(s)
                        if (line.IndexOf("<r>") != -1)
                        {
                            string answerNb = line.Substring(line.IndexOf("<r>") + "<r>".Length, 2);
                            int resultNb = int.Parse(answerNb);

                            reponses = new Action[resultNb];
                            nbRep = resultNb;

                            for (int i = 0; i < resultNb; i++)
                            {
                                //get reponseText
                                lineId++;
                                line = textLines[lineId];
                                int provi = i + 1;
                                string borne1 = "<" + provi + ">";
                                reponses[i].reponseText = StringWithinBounds(borne1, "->", line);
                                // Debug.Log(this.reponses[i].reponseText + i);

                                if (line.IndexOf("<a>") != -1)
                                {
                                    int length = line.Length - line.IndexOf("<a>");
                                    string tempAction = line.Substring(line.IndexOf("<a>"), length);

                                    //get name of next dialog
                                    if (line.IndexOf("dialog") != -1)
                                    {
                                        reponses[i].type = ActionType.dialog;
                                        reponses[i].nextDialog = tempAction.Substring(tempAction.IndexOf("dialog") + "dialog".Length);
                                    }
                                    //close dialog
                                    else if (line.IndexOf("nextScene") != -1)
                                    {
                                        reponses[i].type = ActionType.nextScene;
                                    }
                                    else if (line.IndexOf("leave") != -1)
                                    {
                                        reponses[i].type = ActionType.leave;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return true;
    }

    private string StringWithinBounds(string lBound, string rBound, string phrase)
    {
        int left = phrase.IndexOf(lBound) + lBound.Length;
        int right = phrase.LastIndexOf(rBound);

        string result = phrase.Substring(left, right - left);
        return result;
    }

    void StartExpedition()
    {
        CustomCursorManager.Singleton.SetState(CustomCursorManager.CursorState.Hidden);
        GameManager.Singleton.InstanceManager.StartExpeditionGeneration();
        GameManager.Singleton.InstanceManager.SwitchToNextMap();
        GameManager.Singleton.gameData.gameProgress.expeditionStartedCount++;
    }

    public void CloseDialog()
    {
        GameManager.Singleton.Player.SetControlBlocked(false);
        canvas.gameObject.SetActive(false);
        CustomCursorManager.Singleton.SetState(CustomCursorManager.CursorState.Hidden);
    }

    public void InitDialog()
    {
        GameManager.Singleton.Player.SetControlBlocked(true);
        isReady = loadDialog(path, "dialogue_lobby");
        canvas.gameObject.SetActive(true);
        CustomCursorManager.Singleton.SetState(CustomCursorManager.CursorState.Normal);
    }
}
