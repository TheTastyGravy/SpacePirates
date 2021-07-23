using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100;
    public float healthOnRevive = 25;
    public float Health { get => currentHealth; }

    private float currentHealth;
    private PlayerController controller;
    private RevivePlayerStation reviveStation;

    

    void Start()
    {
        currentHealth = maxHealth;
        controller = GetComponent<PlayerController>();
        reviveStation = GetComponent<RevivePlayerStation>();
    }

    public void UpdateHealth(float health)
	{
        // If we are dead, do nothing
        if (currentHealth == 0)
            return;

        // If negitive it is damage, else healing
        if (health < 0)
		{
            currentHealth += health;
            if (currentHealth <= 0)
			{
                currentHealth = 0;
                Die();
			}
		}
		else
		{
            currentHealth += health;
            if (currentHealth >= maxHealth)
                currentHealth = maxHealth;
        }
	}


    private void Die()
	{
        controller.enabled = false;
        reviveStation.SetIsUsable(true);
    }
    public void Revive()
	{
        currentHealth = healthOnRevive;

        controller.enabled = true;
        reviveStation.SetIsUsable(false);
    }
}
