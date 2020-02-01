using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventBossCombat : MonoBehaviour
{
    [SerializeField] Boss boss;
    [SerializeField] Canvas bossHUD;
    [SerializeField] Image healthBarAmount;
    Player player;

    [Header("State of Event")]
    bool eventLaunch = false;
    public State stateEvent = State.disable;
    BoxCollider2D boxCollider2D;

    [SerializeField] Dictionary<string, BoundsEvent> boundsEvents = new Dictionary<string, BoundsEvent>();

    // Use this for initialization
    void Start()
    {
        player = GameManager.Singleton.Player.GetComponent<Player>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        boundsEvents["left"] = transform.GetChild(0).GetComponent<BoundsEvent>();
        boundsEvents["right"] = transform.GetChild(1).GetComponent<BoundsEvent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (stateEvent == State.anchorFall)
        {
            if (boundsEvents["left"].GetState() == BoundsEvent.State.onGround && boundsEvents["left"].GetState() == BoundsEvent.State.onGround)
            {
                stateEvent = State.playerRun;
            }
        }
        if (stateEvent == State.playerRun)
        {
            //   Camera.main.GetComponent<Camera2D>().ClearFocusList();
            //   Camera.main.GetComponent<Camera2D>().AddFocus(boss.GetComponent<GameEye2D.Focus.F_Transform>());
            if (player.transform.position.x >= transform.GetChild(2).transform.position.x)
            {
                // end move player 
                stateEvent = State.readyCombat;
                boss.ActivateBoss();
                bossHUD.gameObject.SetActive(true);
            }
        }
        else if (stateEvent == State.readyCombat)
        {
            UpdateHUD();
        }
        else if (stateEvent == State.anchorAscend)
        {
            if (boundsEvents["left"].GetState() == BoundsEvent.State.disable && boundsEvents["left"].GetState() == BoundsEvent.State.disable)
            {
                stateEvent = State.end;
            }
        }
    }

    private void UpdateHUD()
    {
        healthBarAmount.fillAmount = (float)boss.Stats.health / (float)boss.Stats.maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!eventLaunch)
            {
                eventLaunch = true;
                stateEvent = State.anchorFall;
                foreach (KeyValuePair<string, BoundsEvent> entry in boundsEvents)
                {
                    entry.Value.SetState(BoundsEvent.State.fall);
                }
            }
        }
    }

    public void EndEvent()
    {
        foreach (KeyValuePair<string, BoundsEvent> entry in boundsEvents)
        {
            entry.Value.SetState(BoundsEvent.State.ascend);
            stateEvent = State.anchorAscend;
        }
    }

    public enum State
    {
        disable,
        anchorFall,
        playerRun,
        readyCombat,
        anchorAscend,
        end
    }
}
