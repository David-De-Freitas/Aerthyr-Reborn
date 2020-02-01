using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditInput : MonoBehaviour
{
    
	void Update ()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            SceneManager.LoadScene("Menu");
        }
	}
}
