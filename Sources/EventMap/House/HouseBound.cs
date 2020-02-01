using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class HouseBound : MonoBehaviour
{

    [SerializeField] House_Lobby house;
    [SerializeField] Type type;

    enum Type
    {
        Enter,
        Exit
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player != null)
        {
            switch (type)
            {
                case Type.Enter:
                    house.InHouse(true);
                    break;
                case Type.Exit:
                    house.InHouse(false);
                    break;
                default:
                    break;
            }
        }
    }
}
