using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestAndBox : MonoBehaviour
{
    [SerializeField] LootTable lootTable;
    [Space]
    [SerializeField] ObjectType type;
    [Space]
    [SerializeField] int spawnChance;
    [SerializeField] int hitToDestroy;
    [Header("STATES RENDERS")]
    [SerializeField] bool useVisualStates;
    [SerializeField] Sprite[] statesSprites;
    [Header("PARTICLES")]
    [SerializeField] ParticleSystem[] destructionParticles;
    [SerializeField] ParticleSystem[] onHitParticles;
    [Header("AUDIO")]
    [SerializeField] AudioClip destructionAudio;
    [SerializeField] AudioClip onHitAudio;
    int hitCount;

    new BoxCollider2D collider2D;
    new SpriteRenderer renderer;
    new AudioSource audio;

    public enum ObjectType
    {
        Chest, Box
    }

    // Use this for initialization
    void Start()
    {
        int spawnRoll;

        spawnRoll = Random.Range(0, 101);
        if (spawnRoll > spawnChance)
        {
            Destroy(gameObject);
        }
        else
        {
            collider2D = GetComponent<BoxCollider2D>();
            renderer = GetComponent<SpriteRenderer>();
            audio = GetComponent<AudioSource>();

            hitCount = 0;
            UpdateVisualState();
        }
    }

    private void Update()
    {
        if (hitCount >= hitToDestroy)
        {
            if (!destructionParticles[0].isPlaying)
            {
                Destroy(gameObject);
            }
        }
    }

    void UpdateVisualState()
    {
        if (useVisualStates)
        {
            if (hitCount < hitToDestroy && hitCount < statesSprites.Length)
            {
                renderer.sprite = statesSprites[hitCount];
            }
        }
    }

    public void Hit()
    {
        if (hitCount < hitToDestroy)
        {
            hitCount++;
            UpdateVisualState();
            for (int i = 0; i < onHitParticles.Length; i++)
            {
                onHitParticles[i].Play();
            }
            if (hitCount >= hitToDestroy)
            {
                collider2D.enabled = false;
                renderer.enabled = false;
                audio.clip = destructionAudio;
                audio.Play();

                if (lootTable != null)
                {
                    lootTable.Drop(transform.position);
                }

                for (int i = 0; i < destructionParticles.Length; i++)
                {
                    destructionParticles[i].Play();
                }
            }
        }
    }
}
