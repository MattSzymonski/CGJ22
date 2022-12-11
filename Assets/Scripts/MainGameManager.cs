using Mighty;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainGameManager : MightyGameManager
{
    public List<Enemy> enemies;


    MightyGameBrain brain;
    private static MainGameManager instance;
    public static MainGameManager Instance { get { return instance; } }

    public Copernicus copernicus;

    public bool notPlaying = true;
    public bool spawningDisabled = false;

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
        notPlaying = MightyGameBrain.Instance.currentGameStateName != "Playing";
        HandleInput();
    }

    public void GameOver()
    {
        // run a small timer for fadeout etc

        GameObject.Find("GameOverText").GetComponent<Text>().text = Utils.LOSE_TEXT;
        MightyTimersManager.Instance.RemoveAllTimers();
        brain.TransitToNextGameState("GameOver");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void Victory()
    {
        GameObject.Find("GameOverText").GetComponent<Text>().text = Utils.WIN_TEXT;
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

        if (Input.GetButtonDown("ControllerAny Start"))
        {
            RestartGame();
        }

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
        //if (exitingGameState == "GameOver") // Transition panel when leaving GameOver state
        //    yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel("TransitionPanel", false, true));

        //if (exitingGameState != "MainMenu")
        //{
        //    yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel("TransitionPanel", false, true));
        //}

        if (exitingGameState == "MainMenu")
        {
            yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel(enteringGameState + "Panel", true, true));
            yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel("TransitionPanel", false, true));
        }
        else
        {
            yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel(enteringGameState + "Panel", true, true));
        }

    }

    // This is called by MightyGameBrain on every game state exit (you decide to handle it or not)
    public override IEnumerator OnGameStateExit(string exitingGameState, string enteringGameState)
    {
        //if (exitingGameState == "GameOver") // Transition panel when leaving GameOver state
        //    yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel("TransitionPanel", true, false));

        //yield return new WaitForSeconds(1);

        if (exitingGameState == "MainMenu")
        {
            yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel("TransitionPanel", true, true));
        }

        //yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel("TransitionPanel", true, true));
        yield return StartCoroutine(MightyUIManager.Instance.ToggleUIPanel(exitingGameState + "Panel", false, true));
    }
}
