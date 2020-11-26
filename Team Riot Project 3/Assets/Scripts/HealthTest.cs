using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthTest : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public HealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            currentHealth -= 10;
            if(currentHealth <= 0)
            {
                currentHealth = 0;
            }
            healthBar.SetHealth(currentHealth);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            currentHealth += 10;
            if (currentHealth >= 100)
            {
                currentHealth = 100;
            }
            healthBar.SetHealth(currentHealth);
        }
    }
}
