using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

public enum PowerUpType
{
    Attack,
    Dash,
    Spell,
    SpellPiercing,
    AttackDamageUp,
    SpellDamageUp,
    HealthUp,
}

public class RunePillar : MonoBehaviour
{
    [Serializable]
    public class PowerUp
    {
        public PowerUpType powerUpType;
        public Sprite icon;
    }

    public SpriteRenderer iconSpriteRenderer;

    public PowerUpType powerUpType;
    public List<PowerUp> powerUpIcons;
    private Dictionary<PowerUpType, Sprite> sprites;

    private ButtonPrompt buttonPrompt;

    private void Awake()
    {
        sprites = new Dictionary<PowerUpType, Sprite>();
        foreach (PowerUp powerUp in powerUpIcons)
        {
            sprites.Add(powerUp.powerUpType, powerUp.icon);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        buttonPrompt = UIManager.Instance.ShowPromptHint(transform);
        buttonPrompt.SetPromptText(powerUpType.ToString());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (!buttonPrompt) return;

        UIManager.Instance.HidePromptHint(buttonPrompt.gameObject);
        buttonPrompt = null;
    }

    // Update is called once per frame
    private void Update()
    {
        if (buttonPrompt == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            UIManager.Instance.HidePromptHint(buttonPrompt.gameObject);
            TopDownCharacterController.Instance.PlayerStats.ApplyPowerUp(powerUpType);
            GameManager.Instance.SelectPowerUpDone();
        }
    }

    public void ActivatePillar(PowerUpType type)
    {
        gameObject.SetActive(true);
        iconSpriteRenderer.sprite = sprites[type];
        powerUpType = type;
    }

    public void DeactivatePillar()
    {
        gameObject.SetActive(false);
        iconSpriteRenderer.sprite = null;
    }
}