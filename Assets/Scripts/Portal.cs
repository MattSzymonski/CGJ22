using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject owningRoom;

    [MinMaxSlider(0, 10)]
    public Vector2 spawnCount;
    public float maxSpawnOffset = 0.5f;


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void SpawnEnemies()
    {
        Vector2 offset = new Vector2(Random.Range(0.0f, maxSpawnOffset), Random.Range(0.0f, maxSpawnOffset));
        Vector2 spawnPoint = Mighty.MightyUtilites.Vec3ToVec2(transform.position) + offset;
        GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(spawnPoint.x, spawnPoint.y, -9.0f), Quaternion.identity);
        MainGameManager.Instance.enemies.Add(newEnemy.GetComponent<Enemy>());

        int spawnNumber = Random.Range((int)spawnCount.x, (int)spawnCount.y);
        for (int i = 0; i < spawnNumber; i++)
        {

        }
    }

    public void Die()
    {
        owningRoom.GetComponent<Room>().DestroyPortal(gameObject);
        StartCoroutine(DieInternal());
    }

    IEnumerator DieInternal()
    {
        GameObject.Destroy(this.gameObject);
        yield return null;
    }

}
