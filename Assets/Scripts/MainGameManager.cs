using Mighty;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainGameManager : MightyGameManager
{
    public List<Enemy> enemies;


    MightyGameBrain brain;
    private static MainGameManager instance;
    public static MainGameManager Instance { get { return instance; } }

    public Copernicus copernicus;


    public GameObject[] rooms;

    void Awake()
    {
        instance = this;
    }


    void Start()
    {
        brain = MightyGameBrain.Instance;
    }

    void Update()
    {
        HandleInput();
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
