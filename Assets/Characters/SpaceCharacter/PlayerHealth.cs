using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] Slider sliderPlayer;
    [SerializeField] Slider sliderShield;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] GameObject shield;
    [SerializeField] GameObject playerAgain;

    float playerHealth;
    float shieldHealth;

    bool shielded = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerHealth = 1.0f;
        shieldHealth = 1.0f;
        playerAgain.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        sliderPlayer.value = playerHealth;
        sliderShield.value = shieldHealth;
    }

    public bool Shielded()
    {
        return shielded;
    }

    public void TakeDamage(float damageAmount)
    {
        if(shieldHealth > 0.0f)
        {
            shieldHealth -= damageAmount;

            if(shieldHealth < 0.0f)
            {
                shield.SetActive(false);
                shielded = false;
            }
        }
        else
        {
            playerHealth -= damageAmount;
        }

        if(playerHealth <= 0.0f)
        {
            Die();
        }
    }

    public void Die()
    {
        playerMovement.Die();
        playerAgain.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
