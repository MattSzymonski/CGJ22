using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Room : MonoBehaviour
{
    public GameObject[] portalList;
    public GameObject[] slots;
    public List<bool> slotsOccupied;

    public float spawnRadius = 2.0f; // TODO: where should this be stored?

    public GameObject portalPrefab;

    public List<GameObject> enemies;
    // Start is called before the first frame update
    void Start()
    {
        slotsOccupied = new List<bool>();
        portalList = new GameObject[slots.Length];
        for (int i = 0; i < slots.Length; ++i)
        {
            slotsOccupied.Add(false);
        }

        enemies = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool SpawnPortal()
    {
        if (slotsOccupied.All((x) => x == true))
            return false;

        int idx;
        bool spawned = false;

        while (!spawned)
        {
            idx = Random.Range(0, slots.Length);
            if (slotsOccupied[idx])
                continue;

            // find a random position in the slot
            Vector3 spawnPos = slots[idx].transform.position + Mighty.MightyUtilites.Vec2ToVec3(Random.insideUnitCircle * spawnRadius);
            spawnPos += new Vector3(0, 0, -5);
            slotsOccupied[idx] = true;
            var portal = Instantiate(portalPrefab, spawnPos, Quaternion.identity);
            portal.GetComponent<Portal>().owningRoom = gameObject; // TODO: why it nulls here sometimes?
            portalList[idx] = portal;
            Debug.Log("Spawning portal");

            spawned = true;
        }

        return true;
    }
    public void DestroyPortal(GameObject portal)
    {
        int idx = -1;
        for (int i = 0; i < portalList.Length; ++i)
        {
            if (portalList[i] == portal)
            {
                idx = i;
                break;
            }
        }
        if (idx == -1)
        {
            Debug.LogError("Did not find portal!!?");
            return;
        }
        slotsOccupied[idx] = false;
        portalList[idx] = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Utils.COPERNICUS))
        {
            collision.GetComponent<Copernicus>().currentRoom = gameObject;
        }
        else if (collision.CompareTag(Utils.ENEMY))
        {
            if (!enemies.Contains(collision.gameObject))
                enemies.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Utils.COPERNICUS))
        {
            collision.GetComponent<Copernicus>().currentRoom = null;
        }
        else if (collision.CompareTag(Utils.ENEMY))
        {
            if (enemies.Contains(collision.gameObject))
                enemies.Remove(collision.gameObject);
        }
    }

}
