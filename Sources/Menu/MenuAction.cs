using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuAction : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        if (System.String.Equals(sceneName , "Exit"))
        {
            //UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }

    }
}