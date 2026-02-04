using System;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI killsText;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] GameObject timeLimitReachedObj;

    int kills = 0;
    float time;

    bool gameOver = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        time = 60.0f;
        timeLimitReachedObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (time > 0.0f)
        {
            time -= Time.deltaTime;
            time = (float)Math.Round(time, 2);
            timeText.text = time.ToString();
        }
        else if (!gameOver)
        {
            gameOver = true;
            playerMovement.Die();
            playerHealth.Die();
            timeLimitReachedObj.SetActive(true);

        }

        killsText.text = kills.ToString();
    }

    public void AddKill()
    {
        kills++;
    }
}
