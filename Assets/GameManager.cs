using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI killsText;
    [SerializeField] TextMeshProUGUI killsNeededText;
    [SerializeField] TextMeshProUGUI roundText;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] GameObject timeLimitReachedObj;
    [SerializeField] AudioClip music;
    [SerializeField] AudioClip intro;

    int kills = 0;
    float introAudioTimer;
    float timer = 0.0f;

    static bool playedIntro = false;
    static float bestTime = 0.0f;
    static int round = 1;
    static int killsNeeded = 5;
    float time = 120.0f;

    bool gameOver = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeLimitReachedObj.SetActive(false);
        SoundManager.instance.PlaySound(music, transform, 1.0f, 0.0f);
        SoundManager.instance.PlaySound(intro, transform, 1.0f, 0.0f);
        playedIntro = true;
        introAudioTimer = intro.length;
        timer = introAudioTimer;
        roundText.text = round.ToString();

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
        killsNeededText.text = killsNeeded.ToString();

        timer -= Time.deltaTime;
        if(Input.GetKeyDown(KeyCode.Space) && timer < 0.0f)
        {
            SoundManager.instance.PlaySound(music, transform, 1.0f, 0.0f);
            SoundManager.instance.PlaySound(intro, transform, 1.0f, 0.0f);
            timer = introAudioTimer;
        }


    }

    public void AddKill()
    {
        kills++;

        if(kills == killsNeeded)
        {
            round++;
            killsNeeded *= 2;
            if(time > bestTime)
            {
                bestTime = time;
            }
            SceneManager.LoadScene(0);
        }
    }
}
