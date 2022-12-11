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
    public int maxCurrentOpenPortals = 4; // they should be configured per each wave

    Copernicus copernicus;

    private static PortalManager instance;
    public static PortalManager Instance { get { return instance; } }

    Mighty.MightyTimer spawnPortalTimer;
    // Start is called before the first frame update
    void Start()
    {

        instance = this;

        copernicus = GameObject.FindGameObjectsWithTag(Utils.COPERNICUS)[0].GetComponent<Copernicus>();

        float coolDown = Random.Range(spawnCooldownsMinMaxes[currentWaveNr].min, spawnCooldownsMinMaxes[currentWaveNr].max);
        spawnPortalTimer = Mighty.MightyTimersManager.Instance.CreateTimer("PortalSpawnTimer", coolDown, 1f, false, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWaveNr >= waveMaxNr)
            return;
        if (MainGameManager.Instance.notPlaying)
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
                if (currentWaveNr >= waveMaxNr)
                    return;

                float coolDown = Random.Range(spawnCooldownsMinMaxes[currentWaveNr].min, spawnCooldownsMinMaxes[currentWaveNr].max);
                Mighty.MightyTimersManager.Instance.RemoveTimer(spawnPortalTimer);
                spawnPortalTimer = Mighty.MightyTimersManager.Instance.CreateTimer("PortalSpawnTimer", coolDown, 1f, false, false);
            }

            // find a room not currently with Big C

            // spawn with a random Time interval depending on the wave
            // wave in turn depends on the progress of research by Copernicus
            bool spawned = false;
            int spawnTries = 0;
            while (!spawned)
            {
                if (spawnTries >= MainGameManager.Instance.rooms.Length)
                    break;

                int roomIdx = Random.Range(0, MainGameManager.Instance.rooms.Length);
                spawnTries++;
                if (MainGameManager.Instance.rooms[roomIdx] == copernicus.currentRoom)
                    continue;

                if (!MainGameManager.Instance.rooms[roomIdx].GetComponent<Room>().SpawnPortal())
                    continue;
                else
                {
                    ++currentOpenPortals;
                    float coolDown = Random.Range(spawnCooldownsMinMaxes[currentWaveNr].min, spawnCooldownsMinMaxes[currentWaveNr].max);
                    Mighty.MightyTimersManager.Instance.RemoveTimer(spawnPortalTimer);
                    spawnPortalTimer = Mighty.MightyTimersManager.Instance.CreateTimer("PortalSpawnTimer", coolDown, 1f, false, false);
                    spawned = true;
                }
            }
            if (!spawned)
                Debug.LogError("Could not spawn portals, all slots in all rooms occupied");
        }
    }
}

