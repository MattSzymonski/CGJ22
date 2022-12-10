using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private List<GameObject> portalList;
    public GameObject[] slots;
    public List<bool> slotsOccupied;

    public float spawnRadius = 2.0f; // TODO: where should this be stored?

    public GameObject portalPrefab;

    public List<GameObject> enemies;
    // Start is called before the first frame update
    void Start()
    {
        portalList = new List<GameObject>();
        slotsOccupied = new List<bool>();
        for (int i = 0; i < slots.Length; ++i)
            slotsOccupied.Add(false);

        enemies = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnPortal()
    {
        int idx = Random.Range(0, slots.Length);
        // find a random position in the slot
        Vector3 spawnPos = slots[idx].transform.position + Mighty.MightyUtilites.Vec2ToVec3(Random.insideUnitCircle * spawnRadius);
        slotsOccupied[idx] = true;
        var portal = Instantiate(portalPrefab, spawnPos, Quaternion.identity);
        portal.GetComponent<Portal>().owningRoom = gameObject; // TODO: why it nulls here sometimes?
        portalList.Add(portal);
        Debug.Log("Spawning portal");
    }

    public void DestroyPortal(GameObject portal)
    {
        slotsOccupied[portalList.FindIndex((x) => x == portal)] = false;
        portalList.Remove(portal);
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
