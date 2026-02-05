using System;
using System.Collections;
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
    [SerializeField] TextMeshProUGUI bestTimeText;
    [SerializeField] TextMeshProUGUI nextRoundTimeText;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerHealth playerHealth;

    [SerializeField] GameObject timeLimitReachedObj;
    [SerializeField] GameObject nextRoundTextObj;

    [SerializeField] AudioClip music;
    [SerializeField] AudioClip intro;

    int kills = 0;
    float introAudioTimer;
    float timer = 0.0f;

    static bool playedIntro = false;
    static float bestTime = 0.0f;
    static int round = 1;
    static int killsNeeded = 5;
    float time = 300.0f;
    static int numMookSpawns = 1;

    bool gameOver = false;
    float nextRoundTimer;
    bool goingToNextRound = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeLimitReachedObj.SetActive(false);
        nextRoundTextObj.SetActive(false);

        SoundManager.instance.PlaySound(music, transform, 1.0f, 0.0f);

        if(!playedIntro)
        {
            SoundManager.instance.PlaySound(intro, transform, 1.0f, 0.0f);
            playedIntro = true;
            timer = introAudioTimer;
        }

        introAudioTimer = intro.length;
        roundText.text = round.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (time > 0.0f )
        {
            if(!goingToNextRound)
            {
                time -= Time.deltaTime;
                time = (float)Math.Round(time, 2);
            }
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
        bestTimeText.text = bestTime.ToString();


        timer -= Time.deltaTime;
        if(Input.GetKeyDown(KeyCode.Space) && timer <= 0.0f)
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
            killsNeeded += 2;
            if(time > bestTime)
            {
                bestTime = time;
            }
            PrepareNextRound();
        }
    }

    public int KillsNeeded()
    {
        return killsNeeded;
    }

    public void PrepareNextRound()
    {
        goingToNextRound = true;
        nextRoundTextObj.SetActive(true);
        StartCoroutine(PrepareNextRoundCoroutine(10.0f));
    }

    private IEnumerator PrepareNextRoundCoroutine(float duration)
    {
        float currTime = duration;


        while(currTime > 0.0f)
        {
            currTime -= Time.deltaTime;
            currTime = (float)Math.Round(currTime, 2);
            nextRoundTimeText.text = currTime.ToString();
            yield return null;
        }

        numMookSpawns++;
        SceneManager.LoadScene(0);
    }

    public int GetNumMookSpawns()
    {
        return numMookSpawns;
    }
}
