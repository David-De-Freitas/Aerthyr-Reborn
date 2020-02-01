using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventCamp : MonoBehaviour
{
    public Transform enemiesParent;
    public EventEndScene endScene;

    [SerializeField] TargetEnemy targetEnemy;
    [Space]
    [SerializeField] float timeUntilTargetDisplaying = 20f;
    float actualTime;

    GameManager gameManager;

    Canvas canvasCountEnemy;
    Text countText;

    Coroutine NearestEnemyRoutine;

    int actualCount;
    bool eventEnd;

    List<Enemy> allEnemies = new List<Enemy>();

    // Use this for initialization
    private void Start()
    {
        gameManager = GameManager.Singleton;

        canvasCountEnemy = GetComponentInChildren<Canvas>();
        countText = canvasCountEnemy.GetComponentInChildren<Text>();

        InitEnemiesList();

        actualTime = timeUntilTargetDisplaying;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (!eventEnd)
        {
            int Count = 0;

            foreach (Enemy enemy in allEnemies)
            {
                if (enemy != null && enemy.stats.health > 0)
                {
                    Count++;
                }
            }

            if (Count != actualCount)
            {
                actualCount = Count;

                if (actualCount == 0)
                {
                    eventEnd = true;
                    canvasCountEnemy.gameObject.SetActive(false);
                    endScene.isActive = true;
                }
                else
                {
                    countText.text = actualCount.ToString();
                    actualTime = timeUntilTargetDisplaying;
                }

                if (NearestEnemyRoutine != null)
                {
                    StopCoroutine(NearestEnemyRoutine);
                }

                targetEnemy.SetTarget(null);
            }

            if (actualTime > 0f)
            {
                actualTime -= Time.deltaTime;

                if (actualTime <= 0f)
                {
                    NearestEnemyRoutine = StartCoroutine(GetNearestEnemy());
                }
            }
        }
    }

    private void InitEnemiesList()
    {
        foreach (Enemy enemy in enemiesParent.GetComponentsInChildren<Enemy>())
        {
            allEnemies.Add(enemy);
        }
    }

    private IEnumerator GetNearestEnemy()
    {
        while (true)
        {
            Enemy nearestEnemy = null;
            float savedDistFromPlayer = 1000;

            foreach (Enemy enemy in allEnemies) // FIND THE NEAREST ENEMY
            {
                if (enemy != null && enemy.stats.health > 0)
                {
                    if (nearestEnemy == null)
                    {
                        nearestEnemy = enemy;
                        savedDistFromPlayer = Vector2.Distance((Vector2)enemy.centerEntity.position, (Vector2)gameManager.Player.centerTransform.position);
                    }
                    else
                    {
                        float distFromPlayer = Vector2.Distance((Vector2)enemy.centerEntity.position, (Vector2)gameManager.Player.centerTransform.position);

                        if (savedDistFromPlayer > distFromPlayer)
                        {
                            nearestEnemy = enemy;
                            savedDistFromPlayer = distFromPlayer;
                        }
                    }
                }
            }

            targetEnemy.SetTarget(nearestEnemy);

            yield return new WaitForSeconds(1f);
        }
    }
}
