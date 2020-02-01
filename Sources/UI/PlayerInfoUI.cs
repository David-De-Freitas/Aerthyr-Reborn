using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{
    [SerializeField] private Image health;
    [SerializeField] private Image stamina;
    [SerializeField]private PlayerInfoGears[] rotatingGears = new PlayerInfoGears[6];
    [Space]
    [SerializeField] private Text level;
    [SerializeField] private Image xpProgressBar;

    private Player playerRef;

	// Use this for initialization
	void Start ()
    {
        playerRef = GameManager.Singleton.Player.GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        UpdateHealthHUD();
        UpdateStaminaHUD();

        UpdateGears();
    }

    void UpdateGears()
    {
        foreach (PlayerInfoGears gears in rotatingGears)
        {
            gears.RotateGears();
        }
    }

    public void UpdateHealthHUD()
    {
        float healthValue;
        healthValue = playerRef.stats.health / playerRef.stats.healthMax;

        health.fillAmount = healthValue;
    }

    public void UpdateStaminaHUD()
    {
        float staminaValue;
        staminaValue = playerRef.stats.stamina / playerRef.stats.staminaMax;

        stamina.fillAmount = staminaValue;
    }

    public void UpdateXpProgressBar(float _factor)
    {
        xpProgressBar.fillAmount = _factor;
    }

    public void UpdateLvl(int _level)
    {
        level.text = "Niveau : " + _level;
    }
}

[System.Serializable]
public class PlayerInfoGears
{
    public Image gears;
    public float rotateSpeed;

    public void RotateGears()
    {
        float fixedRotateSpeed = rotateSpeed * Time.deltaTime;

        gears.transform.Rotate(0f, 0f, fixedRotateSpeed);
    }
}
