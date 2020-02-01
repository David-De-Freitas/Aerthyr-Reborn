using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapTransition : MonoBehaviour
{
    public GameObject[] nextMapGO;

    public Texture2D image;
    RenderTexture imageRT;

    WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();

    bool scenePass = false;
    Scene previousScene;

    private void Start()
    {
        SceneManager.LoadScene("Scenes/Maps/Tome1/Explo/map_1", LoadSceneMode.Additive);

    }


    private void Update()
    {
        if (scenePass && !previousScene.isLoaded)
        {
            SceneManager.LoadScene("Scenes/Maps/Tome1/Explo/map_1", LoadSceneMode.Additive);
        }

        if (nextMapGO.Length == 0)
        {
           // Scene[] allMaps = new Scene[100];

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                string nameScene = SceneManager.GetSceneAt(i).path;
                if (nameScene == "Assets/Scenes/Maps/Tome1/Explo/map_1.unity")
                {
                    nextMapGO = SceneManager.GetSceneAt(i).GetRootGameObjects();
                    break;
                }
            }



            foreach (GameObject gO in nextMapGO)
            {
                if (gO.CompareTag("MainCamera"))
                {

                    StartCoroutine(ScreenShotImage(gO.GetComponent<Camera>()));

                    break;
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
        camera.transform.position = GameManager.Singleton.Player.transform.position + Vector3.back * 10;
        image = new Texture2D(1920, 1080, TextureFormat.RGB24, false);
        RenderTexture.active = imageRT;
        image.ReadPixels(new Rect(0, 0, 1920, 1080), 0, 0);
        image.Apply();
        RenderTexture.active = null;
        camera.targetTexture = null;
        camera.GetComponent<GameEye2D.Behaviour.SmoothFollow>().enabled = true;
        SetNextSprite(image);

        DisableNextLevel();
    }


    void SetNextSprite(Texture2D texture)
    {
        SpriteRenderer[] spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sprite in spriteRenderers)
        {
            if (sprite.name == "NextLevel")
            {
                sprite.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2);
            }
        }
    }


    public void SetPreviousSprite(Texture2D texture)
    {
        SpriteRenderer[] spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
        print("je passe");
        foreach (SpriteRenderer sprite in spriteRenderers)
        {
            print("je passe 2");

            if (sprite.name == "PreviousLevel")
            {
                print("je passe 3");

                sprite.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2);
            }
        }
    }

    public void SetPreviousScene(Scene scene)
    {
        previousScene = scene;
    }

    void MoveNextLevel(float decal)
    {
        foreach (GameObject gO in nextMapGO)
        {
            Transform transform = gO.GetComponent<Transform>();
            transform.position = transform.position - Vector3.right * decal;
        }
    }

    void EnableNextLevel()
    {
        foreach (GameObject gO in nextMapGO)
        {
            gO.SetActive(true);
        }

    }

    void DisableNextLevel()
    {
        foreach (GameObject gO in nextMapGO)
        {
            gO.SetActive(false);
        }
    }

    public void LaunchNextLevel()
    {
        EnableNextLevel();
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
    }
}

