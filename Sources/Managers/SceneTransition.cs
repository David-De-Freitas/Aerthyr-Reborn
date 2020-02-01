using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] Animator animator;

    InstanceManager instanceManager;
    string sceneName;

    // Use this for initialization
    void Start()
    {
        instanceManager = GetComponentInParent<InstanceManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartTransitionTo(string _sceneName)
    {
        Player player = GameManager.Singleton.Player;

        sceneName = _sceneName;
        animator.SetTrigger("Out");

        player.SetControlBlocked(true);
        player.stats.isInvincible = true;
    }

    void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }

    void OnTransitionFinished()
    {
        Player player = GameManager.Singleton.Player;

        player.SetControlBlocked(false);
        player.stats.isInvincible = false;

        instanceManager.SetCanChangeMap(true);
    }
}
