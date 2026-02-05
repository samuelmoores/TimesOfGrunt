using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] Slider sliderPlayer;
    [SerializeField] Slider sliderShield;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] GameObject shield;
    [SerializeField] GameObject playerAgain;
    [SerializeField] AudioClip shieldDownSound;
    [SerializeField] AudioClip shieldDamageSound;
    [SerializeField] AudioClip[] damageSounds;
    [SerializeField] AudioClip deathSound;

    float playerHealth;
    float shieldHealth;

    bool shielded = true;
    bool dead = false;

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
        if (dead)
            return;

        if(shieldHealth > 0.0f)
        {
            shieldHealth -= damageAmount;

            if(shieldHealth <= 0.0f)
            {
                SoundManager.instance.PlaySound(shieldDownSound, transform, 1.0f, 0.0f);
                shield.SetActive(false);
                shielded = false;
            }
            else
            {
                SoundManager.instance.PlaySound(shieldDamageSound, transform, 1.0f, 0.0f);
            }
        }
        else
        {
            playerHealth -= damageAmount;
            int damageSoundIndex = Random.Range(0, damageSounds.Length - 1);
            SoundManager.instance.PlaySound(damageSounds[damageSoundIndex], transform, 1.0f, 0.0f);

        }

        if (playerHealth <= 0.0f)
        {
            Die();
        }
    }

    public void Die()
    {
        SoundManager.instance.PlaySound(deathSound, transform, 1.0f, 0.0f);
        dead = true;
        playerMovement.Die();
        playerAgain.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
