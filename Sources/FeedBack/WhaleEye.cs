using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhaleEye : MonoBehaviour
{
    public Transform eye;
    public Transform ListPositionEye;

    public float speedFollow;
    public float speed;

    public float radius;
    public float distance;

    public float changeTargetInMin;
    public float changeTargetInMax;
    float changeTargetIn;


    public Vector2 start;
    public Vector2 end;
    public float timer;
    public float timeNeed;

    bool follow;
    bool moving;
    Transform target;
    Transform[] posEyePossible;
    int indexTarget;
    // Use this for initialization
    void Start()
    {
        radius = GetComponent<CircleCollider2D>().radius;
        target = GameManager.Singleton.Player.transform;
        posEyePossible = new Transform[ListPositionEye.childCount];
        for (int i = 0; i < posEyePossible.Length; i++)
        {
            posEyePossible[i] = ListPositionEye.GetChild(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (follow)
        {
            if (Vector3.Distance(target.position, transform.position) > 13)
            {
                follow = false;
                indexTarget = Random.Range(0, posEyePossible.Length);
                changeTargetIn = Random.Range(changeTargetInMin, changeTargetInMax);
                start = eye.position;
                end = CalculateTarget();

                timer = 0;
                timeNeed = distance / speed;
                return;
            }

            if (end != CalculateTarget())
            {
                end = CalculateTarget();
                start = eye.localPosition;

                timer = 0;
                timeNeed = distance / speedFollow;
            }

            timer += Time.deltaTime;
            if (timer > timeNeed)
            {
                timer = timeNeed;
            }
            eye.localPosition = Vector2.Lerp(start, end, timer / timeNeed);

        }
        else
        {
            if (Vector3.Distance(target.position, transform.position) < 9)
            {
                follow = true;
                return;
            }
            changeTargetIn -= Time.deltaTime;

            if (changeTargetIn <= 0)
            {
                changeTargetIn = Random.Range(changeTargetInMin, changeTargetInMax);

                int newIndex = Random.Range(0, posEyePossible.Length);

                if (newIndex == indexTarget)
                {
                    if (newIndex == 0)
                    {
                        newIndex++;
                    }
                    else if (newIndex == posEyePossible.Length - 1)
                    {
                        newIndex--;
                    }
                    else
                    {
                        int rand = Random.Range(0, 2);
                        newIndex += -2 * rand + 1;
                    }
                }
                
                indexTarget = newIndex;


                start = eye.position;
                end = CalculateTarget();

                timer = 0;
                timeNeed = distance / speed;
            }

            timer += Time.deltaTime;
            if (timer > timeNeed)
            {
                timer = timeNeed;
            }

            if (timeNeed != 0)
            {
                eye.position = Vector2.Lerp(start, end, timer / timeNeed);
            }

        }
    }


    Vector2 CalculateTarget()
    {
        if (follow)
        {

            Vector2 targetPosEyes = target.position - transform.position;
            targetPosEyes.Normalize();
            targetPosEyes.x *= radius;
            targetPosEyes.y *= (radius / 1.5f);
            distance = Vector2.Distance(eye.position, targetPosEyes);
            return targetPosEyes;
        }

        distance = Vector2.Distance(eye.position, posEyePossible[indexTarget].position);
        return posEyePossible[indexTarget].position;


    }

}
