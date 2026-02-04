using UnityEngine;

public class MookSpawner : MonoBehaviour
{
    [SerializeField] GameObject normalMook;

    float spawnTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnNormalMook();
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;

        if(spawnTimer > 15.0f)
        {
            SpawnNormalMook();
            spawnTimer = 0.0f;
        }
    }

    public void SpawnNormalMook()
    {
        GameObject mookInstance = Instantiate(normalMook, transform.position, Quaternion.identity);
    }
}
