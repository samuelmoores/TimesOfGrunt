using UnityEngine;

public class MookSpawner : MonoBehaviour
{
    [SerializeField] GameObject normalMook;
    [SerializeField] int startPatrolPointIndex;
    [SerializeField] GameManager gm;

    int numSpawns;
    float spawnTime = 5.0f;

    float timer = 0.0f;
    int currNumSpawns = 0;
    int killsNeeded = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnNormalMook();
        numSpawns = gm.GetNumMookSpawns();
        killsNeeded = gm.KillsNeeded();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer > spawnTime && currNumSpawns < numSpawns)
        {
            SpawnNormalMook();
            timer = 0.0f;
        }
    }

    public void SpawnNormalMook()
    {
        GameObject mookInstance = Instantiate(normalMook, transform.position, Quaternion.identity);
        currNumSpawns++;
    }
}
