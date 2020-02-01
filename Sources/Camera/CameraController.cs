using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEye2D.Focus;

public class CameraController : MonoBehaviour
{
    Player player;

    public CameraState state;

    [Header("PLAYER FOLLOW")]
    public Vector2 focusAreaSize;
    [Space]
    public float vecticalOffset;
    [Space]
    public float followRange;
    [Space]
    public Vector2 followBaseSpeed;
    public float currentFollowSpeed;
    [Space]
    public Vector2 targetPos;
    public Vector2 startPos;
    public Vector2 currentPos;
    [Space]
    public bool fastFollow = false;

    [Header("LOOK DOWN")]
    public float lookDownDistance;
    public float timeTolookDown;

    Vector2 centerPos;
    float timeToMove;
    float currentTimeToMove;
    F_Transform playerFocus;

    private void Start()
    {
        player = GameManager.Singleton.Player.GetComponent<Player>();
        startPos = currentPos = Vector2.zero;
        playerFocus = player.GetComponent<F_Transform>();
        state = CameraState.PlayerFollow;
        currentTimeToMove = 0F;
        transform.position = playerFocus.transform.position - Vector3.forward * 10;
    }

    private void LateUpdate()
    {
        centerPos = player.centerTransform.position + Vector3.up * vecticalOffset;

        switch (state)
        {
            case CameraState.PlayerFollow:
                PlayerFollow();
                break;
            case CameraState.LookDown:
                LookDown();
                break;
        }
    }

    #region Player Follow

    private void PlayerFollow()
    {
        CalculateTarget();
        CalculateFollowSpeed();
        CalculateTimeToMove(centerPos);

        if (timeToMove != 0)
        {
            if (currentTimeToMove < timeToMove)
            {
                currentTimeToMove += Time.deltaTime;
            }
            else
            {
                currentTimeToMove = timeToMove;
            }

            float timePerc = currentTimeToMove / timeToMove;
            currentPos = Vector2.Lerp(startPos, targetPos, timePerc);
            

            playerFocus.offset = currentPos + Vector2.up * vecticalOffset;
        }
        //transform.position = CameraCorrection((Vector3)centerPos + (Vector3)currentPos + Vector3.back * 10);
    }

    private void CalculateTarget()
    {
        Vector2 normalizedPlayerDir;
        Vector2 previousTarget = targetPos;

        normalizedPlayerDir = player.stats.velocity.normalized;
        targetPos = (normalizedPlayerDir * followRange);

        if (targetPos.y > 0)
        {
            targetPos.y = 0;
        }

        targetPos.y /= 1.5f;

        if (targetPos != previousTarget)
        {
            //if (targetPos == Vector2.zero)
            //{
            //    //recenterTimerActive = true;
            //    currentRecenterDelay = 0f;
            //}
            startPos = currentPos;
            currentTimeToMove = 0f;
        }
    }

    private void CalculateFollowSpeed()
    {
        Vector2 normalizedPlayerDir;

        float ratioPosX;
        float ratioPosY;

        float speedMultiplier = 1f;

        Vector2 newSpeed;

        if (Vector2.Distance(targetPos, currentPos) > followRange && !fastFollow)
        {
            fastFollow = true;
        }
        else if (targetPos == currentPos)
        {
            fastFollow = false;
        }

        //if (fastFollow)
        //{
        //    speedMultiplier = 2f;
        //}

        normalizedPlayerDir = player.stats.velocity.normalized;

        ratioPosY = Mathf.Abs(targetPos.y) / followRange;
        ratioPosX = 1 - ratioPosY;

        newSpeed.x = ratioPosX * followBaseSpeed.x;
        newSpeed.y = ratioPosY * followBaseSpeed.y;

        currentFollowSpeed = (newSpeed.x + newSpeed.y) * speedMultiplier;

    }

    private void CalculateTimeToMove(Vector2 centerPos)
    {
        float distance = Vector2.Distance(targetPos, startPos);   
        timeToMove = distance / currentFollowSpeed; 
    }

    #endregion

    #region Look Down

    private void LookDown()
    {
        if (currentPos.y != lookDownDistance)
        {
            if (currentTimeToMove < timeToMove)
            {
                currentTimeToMove += Time.deltaTime;
            }
            else
            {
                currentTimeToMove = timeToMove;
            }
        }

        float timePerc = currentTimeToMove / timeToMove;
        currentPos = Vector2.Lerp(startPos, targetPos, timePerc);
        playerFocus.offset = currentPos + Vector2.up * vecticalOffset;

        //transform.position = CameraCorrection((Vector3)centerPos + (Vector3)currentPos + Vector3.back * 10);
    }

    #endregion

    private Vector3 CameraCorrection(Vector3 pos)
    {
        Vector3 newPos = pos;

        newPos *= 100F;
        newPos.x = Mathf.FloorToInt(newPos.x);
        newPos.y = Mathf.FloorToInt(newPos.y);
        newPos /= 100;
        return newPos;
    }

    private void OnDrawGizmos()
    {
        //if (Application.isPlaying)
        //{
        //    Vector3 centerPos = player.centerTransform.position + Vector3.up * vecticalOffset;

        //    Gizmos.color = new Color(1, 0, 0, 0.5f);
        //    Gizmos.DrawSphere(centerPos, followRange);
        //    Gizmos.color = new Color(0, 1, 0, 0.5f);
        //    Gizmos.DrawSphere(centerPos + (Vector3)targetPos, 0.5f);
        //    Gizmos.color = new Color(0, 0, 1, 0.5f);
        //    Gizmos.DrawSphere(centerPos + (Vector3)currentPos, 0.5f);
        //}

    }

    public void SetCameraState(CameraState newState)
    {
        if (newState != state)
        {
            if (newState == CameraState.PlayerFollow)
            {
                timeToMove = 0f;
                currentTimeToMove = 0f;
            }
            else if (newState == CameraState.LookDown)
            {
                startPos = currentPos;
                targetPos = Vector2.down * lookDownDistance;
                timeToMove = timeTolookDown;
                currentTimeToMove = 0f;
            }

            state = newState;
        }
    }

    public enum CameraState
    {
        PlayerFollow,
        LookDown,
    }
}
