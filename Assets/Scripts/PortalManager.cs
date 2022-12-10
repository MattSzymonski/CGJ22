using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public struct CooldownMinMax
{
    public float min, max;
};

public class PortalManager : MonoBehaviour
{
    public int waveMaxNr = 4;
    public int currentWaveNr = 0;
    public CooldownMinMax[] spawnCooldownsMinMaxes;
    public float[] scoreLevelRanges;

    public int currentOpenPortals;
    public int maxCurrentOpenPortals = 2;

    Copernicus copernicus;

    Mighty.MightyTimer spawnPortalTimer;
    MainGameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        copernicus = GameObject.FindGameObjectsWithTag(Utils.COPERNICUS)[0].GetComponent<Copernicus>();
        gameManager = GameObject.Find("GameManager").GetComponent<MainGameManager>();

        float coolDown = Random.Range(spawnCooldownsMinMaxes[currentWaveNr].min, spawnCooldownsMinMaxes[currentWaveNr].max);
        spawnPortalTimer = Mighty.MightyTimersManager.Instance.CreateTimer("PortalSpawnTimer", coolDown, 1f, false, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWaveNr > waveMaxNr)
            return;
        SpawnPortals();
    }

    void SpawnPortals()
    {
        if (spawnPortalTimer.finished)
        {
            // Max 2 concurrent open portals?
            if (currentOpenPortals >= maxCurrentOpenPortals)
            {
                Debug.Log("Too many open portals right now, arm the timer");
                spawnPortalTimer.RestartTimer();
                return;
            }

            if (copernicus.score > copernicus.scoreTarget)
            {
                Debug.Log("Won game, not spawning portals anymore");
                return;
            }

            if (copernicus.score > scoreLevelRanges[currentWaveNr])
            {
                currentWaveNr += 1;
                float coolDown = Random.Range(spawnCooldownsMinMaxes[currentWaveNr].min, spawnCooldownsMinMaxes[currentWaveNr].max);
                Mighty.MightyTimersManager.Instance.RemoveTimer(spawnPortalTimer);
                spawnPortalTimer = Mighty.MightyTimersManager.Instance.CreateTimer("PortalSpawnTimer", coolDown, 1f, false, false);
            }

            // find a room not currently with Big C

            // spawn with a random Time interval depending on the wave
            // wave in turn depends on the progress of research by Copernicus
            foreach (var room in gameManager.rooms)
            {
                if (room == copernicus.currentRoom)
                    continue;

                if (!room.GetComponent<Room>().SpawnPortal())
                    continue;
                else
                {
                    float coolDown = Random.Range(spawnCooldownsMinMaxes[currentWaveNr].min, spawnCooldownsMinMaxes[currentWaveNr].max);
                    Mighty.MightyTimersManager.Instance.RemoveTimer(spawnPortalTimer);
                    spawnPortalTimer = Mighty.MightyTimersManager.Instance.CreateTimer("PortalSpawnTimer", coolDown, 1f, false, false);
                    return;
                }
            }
            Debug.LogError("Could not spawn portals, all slots in all rooms occupied");
        }
    }
}

