using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventEndScene : MonoBehaviour
{
    [SerializeField] Type goTo;
    InstanceManager instanceManager;
    public bool isActive = true;

    enum Type
    {
        Lobby,
        Lobby_increaseDiff,
        NextScene
    }

    private void Start()
    {
        instanceManager = GameManager.Singleton.InstanceManager;
    }

    public void Activate()
    {
        if (isActive)
        {
            switch (goTo)
            {
                case Type.Lobby:
                    instanceManager.EndExpedition();
                    break;
                case Type.Lobby_increaseDiff:
                    instanceManager.EndExpedition();
                    GameManager.Singleton.gameData.IncreaseGameDifficulty();
                    break;
                case Type.NextScene:
                    instanceManager.SwitchToNextMap();
                    break;
                default:
                    break;
            }
        }
    }
}
