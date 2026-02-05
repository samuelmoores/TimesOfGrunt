using UnityEngine;

public class MookSpawner : MonoBehaviour
{
    [SerializeField] GameObject normalMook;
    [SerializeField] int numSpawns;
    [SerializeField] float spawnTime;
    [SerializeField] int startPatrolPointIndex;

    float timer;
    int currNumSpawns = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnNormalMook();
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
