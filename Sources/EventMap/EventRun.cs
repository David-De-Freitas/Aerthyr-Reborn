using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventRun : MonoBehaviour
{
    public Transform piranhasParent;
    Transform target;
    Player player;

    public Transform[] piranhas;
    public Vector3[] piranhasInitPos;

    ContactFilter2D filter;
    public Collider2D circle;

    public float PPS;
    float speed = 6;
    float speedRotation = 5;

    bool alreadyInCollid;
    float timerDamage;
    void Start()
    {
        target = GameManager.Singleton.Player.transform;
        player = GameManager.Singleton.Player;
        piranhas = new Transform[piranhasParent.childCount];
        for (int i = 0; i < piranhas.Length; i++)
        {
            piranhas[i] = piranhasParent.GetChild(i);
        }
    }

    void Update()
    {
        Vector2 dir = target.position - piranhasParent.position;
        dir.Normalize();

        if (Vector2.Distance(target.position, piranhasParent.position) > 2)
        {
            piranhasParent.Translate(Vector3.zero + (Vector3)dir * speed * Time.deltaTime);
        }
        //UpdateMovement();
        UpdateRotation();



        Collider2D[] collider2Ds = new Collider2D[200];

        Physics2D.OverlapCollider(circle, filter, collider2Ds);
        bool hit = false;
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider != null)
            {
                Player playerHit = collider.GetComponent<Player>();

                if (playerHit != null)
                {
                    hit = true;
                    timerDamage += Time.deltaTime;

                    if (timerDamage > 0.4f)
                    {
                        player.TakeDamage((PPS / 100 * player.stats.healthMax) * timerDamage);
                        timerDamage = 0f;
                    }
                    break;
                }
            }
        }
        if (alreadyInCollid && !hit)
        {
            player.TakeDamage((PPS / 100 * player.stats.healthMax) * timerDamage);
            timerDamage = 0f;
        }
        alreadyInCollid = hit;
    }

    void UpdateMovement()
    {
        for (int i = 0; i < piranhas.Length; i++)
        {
            piranhas[i].position = piranhasInitPos[i] + (Vector3.up * Mathf.Sin(Time.time) / 2);
        }
    }

    void UpdateRotation()
    {
        foreach (Transform piranha in piranhas)
        {
            Vector3 vectorToTarget = target.position - piranha.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            if (target.position.x < piranha.position.x)
            {
                piranha.localScale = new Vector3(1, -1, 1);
            }
            else
            {
                piranha.localScale = new Vector3(1, 1, 1);
            }

            piranha.rotation = Quaternion.Slerp(piranha.rotation, q, Time.deltaTime * speedRotation);
        }
    }



}
