using System;
using System.Collections.Generic;
using UI;

public class PlayerStats
{
    public float Speed = 6;

    public int MaxHealth = 3;
    public int Health;

    public bool HasAttack;
    public bool HasSpell;
    public bool HasSpellPiercing;
    public bool HasDash;

    public int AttackDamage = 1;
    public float AttackCooldown = 0.5f;
    public float AttackRange = 1.5f;

    public float SpellCooldown = 2f;
    public float SpellScale = 1f;
    public int SpellDamage = 1;
    public float SpellSpeed = 10;

    public float DashLength = 0.2f;
    public float DashCooldown = 1;
    public float DashSpeed = 20;

    public void ApplyPowerUp(PowerUpType powerUpType)
    {
        switch (powerUpType)
        {
            case PowerUpType.Attack:
                HasAttack = true;
                UIManager.Instance.ActivateAttackIcon();
                break;
            case PowerUpType.Spell:
                HasSpell = true;
                UIManager.Instance.ActivateSpellIcon();
                break;
            case PowerUpType.Dash:
                HasDash = true;
                UIManager.Instance.ActivateDashIcon();
                break;
            case PowerUpType.SpellPiercing:
                HasSpellPiercing = true;
                break;
            case PowerUpType.AttackDamageUp:
                AttackDamage++;
                break;
            case PowerUpType.SpellDamageUp:
                SpellDamage++;
                SpellScale += 1f;
                break;
            case PowerUpType.HealthUp:
                MaxHealth++;
                UIManager.Instance.UpdateHearts();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(powerUpType), powerUpType, null);
        }
    }

    public List<PowerUpType> GetPossiblePowerUpPool()
    {
        List<PowerUpType> possiblePowerUps = new();
        if (!HasAttack)
        {
            possiblePowerUps.Add(PowerUpType.Attack);
        }
        else
        {
            possiblePowerUps.Add(PowerUpType.AttackDamageUp);
        }

        if (!HasSpell)
        {
            possiblePowerUps.Add(PowerUpType.Spell);
        }
        else
        {
            if (!HasSpellPiercing)
            {
                possiblePowerUps.Add(PowerUpType.SpellPiercing);
            }

            possiblePowerUps.Add(PowerUpType.SpellDamageUp);
        }

        if (!HasDash)
        {
            possiblePowerUps.Add(PowerUpType.Dash);
        }

        possiblePowerUps.Add(PowerUpType.HealthUp);
        return possiblePowerUps;
    }
}