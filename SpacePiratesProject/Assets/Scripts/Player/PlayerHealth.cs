using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100;
    public float healthOnRevive = 25;

    private float currentHealth;
    private BasicController controller;
    private RevivePlayerStation reviveStation;

    

    void Start()
    {
        currentHealth = maxHealth;
        controller = GetComponent<BasicController>();
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
        controller.canMove = false;
        reviveStation.enabled = true;
    }
    public void Revive()
	{
        currentHealth = healthOnRevive;

        controller.canMove = true;
        reviveStation.enabled = false;
    }
}
