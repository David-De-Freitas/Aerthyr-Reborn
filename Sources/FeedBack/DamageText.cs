using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    new Rigidbody2D rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        float angle = Random.Range(120, 60);
        Vector2 vel = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
        
        rigidbody.velocity = vel * 10;
    }

    public void DestroyText()
    {
        Destroy(gameObject);
    }
}
