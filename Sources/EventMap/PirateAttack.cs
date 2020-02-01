using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PirateAttack : MonoBehaviour
{
    [Header("Attack Info")]
    public float AttackDuration;

    public float spawnFrequencyMin;
    public float spawnFrequencyMax;

    public float nextSpawnTimer;


    public GameObject castureuil;
    public GameObject cerfbourse;
    public GameObject loulpe;

    public Transform spawnZones;
    BoxCollider2D[] spawnBox;

    public Transform EnemiesParent;

    Canvas canvasTimer;
    Text textTimer;

    Player player;

    [Header("State of Event")]
    public bool eventLaunch = false;
    public State stateEvent = State.disable;

    [SerializeField] Dictionary<string, BoundsEvent> boundsEvents = new Dictionary<string, BoundsEvent>();

    // Use this for initialization
    void Start()
    {
        player = GameManager.Singleton.Player.GetComponent<Player>();

        boundsEvents["left"] = transform.GetChild(0).GetComponent<BoundsEvent>();
        boundsEvents["right"] = transform.GetChild(1).GetComponent<BoundsEvent>();

        spawnBox = spawnZones.GetComponentsInChildren<BoxCollider2D>();

        canvasTimer = GetComponentInChildren<Canvas>();
        textTimer = canvasTimer.transform.GetChild(0).GetComponentInChildren<Text>();
        canvasTimer.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (stateEvent == State.anchorFall)
        {
            if (boundsEvents["left"].GetState() == BoundsEvent.State.onGround && boundsEvents["right"].GetState() == BoundsEvent.State.onGround)
            {
                stateEvent = State.readyCombat;
                CalNextSpawn();
                canvasTimer.gameObject.SetActive(true);
            }
        }
        else if (stateEvent == State.readyCombat)
        {
            UpdateAttack();

        }
    }

    void UpdateAttack()
    {
        AttackDuration -= Time.deltaTime;
        nextSpawnTimer -= Time.deltaTime;

        
        if (AttackDuration <= 0f)
        {
            stateEvent = State.end;
            canvasTimer.gameObject.SetActive(false);
            foreach (KeyValuePair<string, BoundsEvent> entry in boundsEvents)
            {
                entry.Value.SetState(BoundsEvent.State.ascend);
            }
            return;
        }
        if (nextSpawnTimer <= 0)
        {
            
            SpawnEnemy();
            CalNextSpawn();
        }
        textTimer.text = Mathf.Floor(AttackDuration).ToString();

    }

    void SpawnEnemy()
    {
        float rand = Random.Range(0f, 100f);

        TypeEnemy type;

        if (rand < 50f) //50%
        {
            type = TypeEnemy.loulpe;
        }
        else if (rand < 85f) //35%
        {
            type = TypeEnemy.castureuil;
        }
        else //15%
        {
            type = TypeEnemy.cerfbourse;
        }

        CreateEntity(type);
    }

    void CreateEntity(TypeEnemy type)
    {
        GameObject newEnemy;

        switch (type)
        {
            case TypeEnemy.loulpe:
                newEnemy = Instantiate(loulpe, EnemiesParent) as GameObject;
                break;
            case TypeEnemy.castureuil:
                newEnemy = Instantiate(castureuil, EnemiesParent) as GameObject;
                break;
            case TypeEnemy.cerfbourse:
                newEnemy = Instantiate(cerfbourse, EnemiesParent) as GameObject;
                break;
            default:
                newEnemy = Instantiate(loulpe, EnemiesParent) as GameObject;
                break;
        }
        newEnemy.transform.position = CalNewPos();
    }


    Vector3 CalNewPos()
    {
        Vector3 pos = Vector3.zero;

        int boxIndex = Random.Range(0, spawnBox.Length);

        float x = Random.Range(spawnBox[boxIndex].bounds.min.x, spawnBox[boxIndex].bounds.max.x);
        float y = Random.Range(spawnBox[boxIndex].bounds.min.y, spawnBox[boxIndex].bounds.max.y);

        pos.x = x;
        pos.y = y;

        return pos;
    }

    void CalNextSpawn()
    {
        nextSpawnTimer = Random.Range(spawnFrequencyMin, spawnFrequencyMax);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!eventLaunch && stateEvent == State.disable)
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


    public enum State
    {
        disable,
        anchorFall,
        readyCombat,
        end
    }

    private enum TypeEnemy
    {
        loulpe,
        castureuil,
        cerfbourse
    }
}
