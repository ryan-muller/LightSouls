using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    Collider damageCollider;

    public int weaponDamage;

    private void Awake()
    {
        damageCollider = GetComponent<Collider>();
        damageCollider.gameObject.SetActive(true);
        damageCollider.isTrigger = true;
        damageCollider.enabled = false;
    }

    public void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    public void DisableDamageCollider()
    {
        damageCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Hittable")
        {
            Debug.Log("HIT!");

            if (collision.TryGetComponent(out EntityStats entityStats))
                {
                entityStats.TakeDamage(weaponDamage);
                }

            //if (playerStats != null)
            //{
            //    playerStats.TakeDamage(weaponDamage);
            //}
        }
    }
}
