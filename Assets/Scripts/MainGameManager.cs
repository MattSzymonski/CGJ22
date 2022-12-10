using Mighty;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class MainGameManager : MightyGameManager
{
    public List<Enemy> enemies;


    MightyGameBrain brain;
    private static MainGameManager instance;
    public static MainGameManager Instance { get { return instance; } }

    public Copernicus copernicus;


    public GameObject[] rooms;

    private MightyTimer spawnTimer;

    public float spawnPortalsCooldown = 3.0f;


    void Awake()
    {
        instance = this;
    }


    void Start()
    {
        brain = MightyGameBrain.Instance;
        spawnTimer = MightyTimersManager.Instance.CreateTimer("SpawnPortalsTimer", spawnPortalsCooldown, 1f, false, false);
    }

    void Update()
    {
        HandleInput();
        SpawnPortals();
    }

    public void GameOver()
    {
        // TODO: set loose text in panel
        brain.TransitToNextGameState("GameOver");
    }

    public void Victory()
    {
        // TODO: set win text in panel
        brain.TransitToNextGameState("GameOver");
    }

    void HandleInput()
    {
        //if (Input.GetButtonDown("Escape"))
        //{
        //    MightyAudioManager.Instance.PlaySound("UI_Button_Click");

        //    if (brain.currentGameStateName == "Playing")
        //        brain.TransitToNextGameState("Pause");

        //    if (brain.currentGameStateName == "Pause")
        //        brain.TransitToNextGameState("Playing");

        //    if (brain.currentGameStateName == "Options")
        //        brain.TransitToNextGameState("Pause");
        //}

        //if (Input.GetButtonDown("ControllerAny Start"))
        //{
        //    MightyAudioManager.Instance.PlaySound("UI_Button_Click");

        //    if (brain.currentGameStateName == "Playing")
        //        brain.TransitToNextGameState("Pause");

        //    if (brain.currentGameStateName == "Pause")
        //        brain.TransitToNextGameState("Playing");
        //}
    }

    void SpawnPortals()
    {
        // find a random room without Copernicus
        // find a random not occupied slot
        // spawn a portal
        if (spawnTimer.finished)
        {
            int[] roomsIndexes = new int[rooms.Length];
            for (int i = 0; i < rooms.Length; ++i)
                roomsIndexes[i] = i;

            while (!roomsIndexes.All((x) => x == -1))
            {
                int roomIdx = Random.Range(0, rooms.Length);
                var room = rooms[roomIdx].GetComponent<Room>();

                if (room.slotsOccupied.All((x) => x == true))
                {
                    roomsIndexes[roomIdx] = -1;
                    continue;
                }

                room.SpawnPortal();
                spawnTimer.RestartTimer();
                return;
            }
            Debug.Log("Did not find a free portal spawner");
        }
    }

    // --- MightyGameBrain callbacks ---

    // This is called by MightyGameBrain on every game state enter (you decide to handle it or not)
    public override IEnumerator OnGameStateEnter(string enteringGameState, string exitingGameState)
    {
        if (exitingGameState == "GameOver") // Transition panel when leaving GameOver state
            yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel("TransitionPanel", false, true));

        yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel(enteringGameState + "Panel", true, true));
    }

    // This is called by MightyGameBrain on every game state exit (you decide to handle it or not)
    public override IEnumerator OnGameStateExit(string exitingGameState, string enteringGameState)
    {
        if (exitingGameState == "GameOver") // Transition panel when leaving GameOver state
            yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel("TransitionPanel", true, false));

        yield return new WaitForSeconds(1);

        yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel(exitingGameState + "Panel", false, true));
    }
}
