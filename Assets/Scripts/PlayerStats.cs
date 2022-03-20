using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : EntityStats
{

    PlayerAnimationHandler animationHandler;

    protected override void Start()
    {
        base.Start();

        animationHandler = GetComponentInChildren<PlayerAnimationHandler>();
    }

    private int GetMaxHealthFromHealthLevel()
    {
        maxHealth = healthLevel * 10;
        return maxHealth;
    }

    public override void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthBar.SetCurrentHealth(currentHealth);

        animationHandler.PlayTargetAnimation("TakeDamage02", true);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            animationHandler.PlayTargetAnimation("Death", true);
            //HANDE PLAYER DEATH
        }
    }

}
