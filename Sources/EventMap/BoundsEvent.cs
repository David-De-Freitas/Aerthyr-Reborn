using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsEvent : MonoBehaviour
{
    [SerializeField] Sprite anchor;
    [SerializeField] Sprite anchorInpact;



    public ParticleSystem particle;
    public State state;
    public float speedFalling;
    private Transform anchorT;

    //Lerpfalling info
    public float StartPos;
    float timeNeed;
    float timer = 0;

    [Header("ChainSprite")]
    public Sprite spriteChain;

    BoxCollider2D boxCollider2;

    // Use this for initialization
    void Start()
    {
        anchorT = GetComponentInChildren<SpriteRenderer>().transform;
        anchorT.Translate(Vector3.up * StartPos);
        timeNeed = StartPos / speedFalling;

        boxCollider2 = GetComponent<BoxCollider2D>();
        GenerateChain();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.fall)
        {
            Vector3 position = anchorT.localPosition;
            boxCollider2.enabled = true;

            timer += Time.deltaTime;
            if (timer > timeNeed)
            {
                timer = 0;
                state = State.onGround;
                position.y = 0;
                if (particle != null)
                {
                    particle.Emit(100);
                }
            }
            else
            {
                position.y = Mathf.Lerp(StartPos, 0, timer / timeNeed);
            }

            anchorT.localPosition = position;
        }
        else if (state == State.ascend)
        {
            Vector3 position = anchorT.localPosition;
            boxCollider2.enabled = false;

            timer += Time.deltaTime;
            if (timer > timeNeed)
            {
                timer = 0;
                state = State.disable;
                position.y = StartPos;
            }
            else
            {
                position.y = Mathf.Lerp(0, StartPos, timer / timeNeed);
            }

            anchorT.localPosition = position;
        }

    }

    public void SetState(State newState)
    {
        state = newState;
    }
    public State GetState()
    {
        return state;
    }

    private void GenerateChain()
    {
        Vector3 prevPosition = anchorT.GetChild(0).position;
        Vector3 prevScale = anchorT.GetChild(0).localScale;
        int prevLayer = anchorT.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder;
        for (int i = 0; i < 10; i++)
        {
            GameObject newGo = new GameObject("Chain");
            newGo.transform.SetParent(anchorT);

            newGo.AddComponent<SpriteRenderer>().sprite = spriteChain;
            newGo.GetComponent<SpriteRenderer>().sortingOrder = prevLayer + i + 1;
            ;
            Vector3 newPosition = prevPosition;
            newPosition.y += 2.595F;
            newGo.transform.position = newPosition;
            newGo.transform.localScale = prevScale;
            prevPosition = newPosition;
        }
    }

    public enum State
    {
        disable,
        fall,
        onGround,
        ascend
    }
}
