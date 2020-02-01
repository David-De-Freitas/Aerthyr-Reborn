using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMap : MonoBehaviour
{



    public Texture2D image;
    RenderTexture imageRT;

    WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();
    GameObject[] trasitionMapGo;
    MapTransition mapTransition;

    public Camera cam;
    // Use this for initialization
    public State state = State.nothing;


    void Start()
    {



    }

    // Update is called once per frame
    void Update()
    {


        if (state == State.loadTrasition)
        {
            SceneManager.LoadScene("Scenes/TransitionMap", LoadSceneMode.Additive);

            state = State.waitingLoad;
        }
        else if (state == State.waitingLoad)
        {
            if (SceneManager.GetSceneByName("Transition").isLoaded)
            {
                state = State.screenshot;
            }
        }
        else if (state == State.screenshot)
        {
            if (trasitionMapGo.Length == 0)
            {
                trasitionMapGo = SceneManager.GetSceneByName("Transition").GetRootGameObjects();
                foreach (GameObject gO in trasitionMapGo)
                {
                    if (gO.name == "Transition")
                    {
                        mapTransition = gO.GetComponent<MapTransition>();
                        state = State.screenshot;
                        mapTransition.SetPreviousScene(SceneManager.GetActiveScene());
                        StartCoroutine(ScreenShotImage(cam));
                        break;
                    }
                }
            }
        }

    }


    public IEnumerator ScreenShotImage(Camera camera)
    {

        yield return frameEnd;

        imageRT = new RenderTexture(1920, 1080, 24);
        camera.targetTexture = imageRT;
        camera.Render();
        image = new Texture2D(1920, 1080, TextureFormat.RGB24, false);
        RenderTexture.active = imageRT;
        image.ReadPixels(new Rect(0, 0, 1920, 1080), 0, 0);
        image.Apply();
        RenderTexture.active = null;
        camera.targetTexture = null;
        camera.GetComponent<GameEye2D.Behaviour.SmoothFollow>().enabled = true;
        mapTransition.SetPreviousSprite(image);

        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
    }



    public enum State
    {
        nothing,
        loadTrasition,

        waitingLoad,
        screenshot,
        destroy
    }
}
