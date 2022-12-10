using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject owningRoom;

    [MinMaxSlider(1, 10)]
    public Vector2 spawnCount = new Vector2(1, 10);

    [MinMaxSlider(0, 10)]
    public Vector2 waitBetweenSpawnTimeOffset = new Vector2(0, 10);

    public float maxSpawnOffset = 0.5f;
    public float chargingTime = 1.0f;

    public Mighty.MightyTimer portalTimer;
    public bool loaded;


    void Start()
    {
        portalTimer = Mighty.MightyTimersManager.Instance.CreateTimer("PortalTimer", chargingTime, 1f, false, false);
    }

    void Update()
    {
        if (portalTimer.finished && !loaded) 
        {
            loaded = true;
            StartCoroutine(SpawnEnemies());
        }
    }

    IEnumerator SpawnEnemies() // TODO: wave manager  (not as a counter but in a manager, seeing how many enemies there are etc, be smart)
    {
        int spawnNumber = Random.Range((int)spawnCount.x, (int)spawnCount.y);
        for (int i = 0; i < spawnNumber; i++)
        {
            float wait = Random.Range(waitBetweenSpawnTimeOffset.x, waitBetweenSpawnTimeOffset.y);
            yield return new WaitForSeconds(wait);
            SpawnEnemy();
        }
        StartCoroutine(DieInternal("Self"));
    }

    void SpawnEnemy()
    {
        Vector2 offset = new Vector2(Random.Range(0.0f, maxSpawnOffset), Random.Range(0.0f, maxSpawnOffset));
        Vector2 spawnPoint = Mighty.MightyUtilites.Vec3ToVec2(transform.position) + offset;
        GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(spawnPoint.x, spawnPoint.y, -10.0f), Quaternion.identity);
        MainGameManager.Instance.enemies.Add(newEnemy.GetComponent<Enemy>());
    }

    public void Die()
    {
        owningRoom.GetComponent<Room>().DestroyPortal(gameObject);
        StartCoroutine(DieInternal("Player"));
    }

    IEnumerator DieInternal(string type)
    {
        if (type == "Player") 
        {
            yield return new WaitForSeconds(0.0f);
        }

        if (type == "Self")
        {
            yield return new WaitForSeconds(1.0f);
        }

        GameObject.Destroy(this.gameObject);
        yield return null;
    }

}
