using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : EntityStats
{
    Animator animator;

    protected override void Start()
    {
        base.Start();

        animator = GetComponentInChildren<Animator>();
    }

    public override void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthBar.SetCurrentHealth(currentHealth);

        animator.Play("TakeDamage02");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            animator.Play("Death");
            //HANDE PLAYER DEATH
        }
    }
}
