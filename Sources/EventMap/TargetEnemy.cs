using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetEnemy : MonoBehaviour
{
    private Transform target;
    new Camera camera;
    public float LeftAngle;
    public float RightAngle;
    // Use this for initialization
    public Side side;
    Image image;
    Rect cam;

    void Start()
    {
        camera = Camera.main;
        image = GetComponent<Image>();
        cam = new Rect(0, 0, camera.pixelWidth, camera.pixelHeight);
        Vector2 camCenter = new Vector2(cam.xMin + cam.width / 2, cam.yMin + cam.height / 2);

        LeftAngle = Vector2.Angle(Vector2.right, new Vector2(cam.xMin, cam.yMin) - camCenter);
        RightAngle = Vector2.Angle(Vector2.right, new Vector2(cam.xMax, cam.yMin) - camCenter);
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {          
            float distFromTarget = Vector2.Distance(camera.transform.position, target.position);

            if (distFromTarget > 10)
            {
                if (!image.enabled)
                {
                    image.enabled = true;
                }

                float actualAngle = Vector2.Angle(Vector2.right, target.position - camera.transform.position);
                float actualSignedAngle = Vector2.SignedAngle(Vector2.right, target.position - camera.transform.position);

                transform.eulerAngles = Vector3.forward * actualSignedAngle;

                if (actualAngle < LeftAngle && actualAngle > RightAngle)
                {
                    if (target.position.y > camera.transform.position.y)
                    {
                        side = Side.top;
                    }
                    else
                    {
                        side = Side.bot;
                    }
                }
                else
                {
                    if (target.position.x > camera.transform.position.x)
                    {
                        side = Side.right;
                    }
                    else
                    {
                        side = Side.left;
                    }
                }

                Vector2 pos = Vector2.zero;
                switch (side)
                {
                    case Side.left:
                        pos.x = 0;
                        float realAngle = actualSignedAngle;

                        if (actualSignedAngle < 0)
                        {
                            realAngle += 180;
                        }
                        else
                        {
                            realAngle -= 180;
                        }

                        pos.y = (Screen.height / 2f) - (realAngle / RightAngle) * (Screen.height / 2f);
                        transform.localPosition = pos;
                        break;
                    case Side.top:
                        pos.y = Screen.height;
                        pos.x = Screen.width + ((actualSignedAngle - RightAngle) / (LeftAngle - RightAngle)) * -Screen.width;
                        transform.localPosition = pos;
                        break;
                    case Side.right:
                        pos.x = Screen.width;
                        pos.y = (Screen.height / 2f) + (actualSignedAngle / RightAngle) * (Screen.height / 2f);
                        transform.localPosition = pos;
                        break;
                    case Side.bot:
                        pos.y = 0;
                        pos.x = Screen.width + ((actualSignedAngle + RightAngle) / (LeftAngle - RightAngle)) * Screen.width;
                        transform.localPosition = pos;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (image.enabled)
                {
                    image.enabled = false;
                }
            }
        }
        else
        {
            if (image.enabled)
            {
                image.enabled = false;
            }
        }
    }

    public void SetTarget(Enemy enemy)
    {
        if (enemy == null)
        {
            target = null;
        }
        else
        {
            target = enemy.centerEntity;
        }
    }

    public enum Side
    {
        left,
        top,
        right,
        bot
    }
}
